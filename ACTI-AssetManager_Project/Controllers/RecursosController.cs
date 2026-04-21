using ACTI_AssetManager_Project.Application.DTOs;
using ACTI_AssetManager_Project.Application.Interfaces;
using ACTI_AssetManager_Project.Application.Services;
using ACTI_AssetManager_Project.Domain.Entities;
using ACTI_AssetManager_Project.Models;
using ACTI_AssetManager_Project.Models.Recursos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ACTI_AssetManager_Project.Controllers
{
    [Authorize]
    public class RecursosController : Controller
    {
        private readonly IRecursoService _recursoService;
        private readonly IResponsableRecursoService _responsableRecursoService;

        public RecursosController(IRecursoService recursoService)
        {
            _recursoService = recursoService;
        }

        // Helper: obtiene el idUsuario real del JWT (no el nombre)
        // En AuthService se guarda ClaimTypes.NameIdentifier = user.IdUsuario
        private string? ObtenerIdUsuarioActual()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // GET /Recursos
        public async Task<IActionResult> Index(string? filtroCodigo, int? filtroTipo, int? filtroEstado, int pagina = 1)
        {
            //Paginado
            int registrosPorPagina = 10;
            var (recursosQuery, totalRegistros) = await _recursoService.ObtenerRecursosPaginadosAsync(
             filtroCodigo, filtroTipo, filtroEstado, pagina, registrosPorPagina);

            // 1. Obtener datos base
            var recursosAsignados = await _recursoService.ObtenerRecursos_YaAsignadosAsync();

            var tipos = await _recursoService.ObtenerTiposAsync();
            var estados = await _recursoService.ObtenerEstadosAsync();
            var responsablesDto = await _recursoService.ObtenerResponsablesParaDropdownAsync();
            var listaResponsables = responsablesDto.Select(u => new SelectListItem(u.NombreCompleto, u.IdUsuario.ToString())).ToList();
            var infoMantenimiento = estados.FirstOrDefault(e => e.IdEstado == 6);
            var categorias = await _recursoService.ObtenerCategoriasAsync();


            ViewBag.Categorias = categorias.Select(c => new SelectListItem
            {
                Value = c.IdCategoria.ToString(),
                Text = c.Nombre
            }).ToList();

            //ViewBag.TiposConCategoria = tipos.Select(t => new {
            //    Id = t.IdTipoRecurso.ToString(),
            //    Nombre = t.NombreTipoRecurso,
            //    Cat = t.IdCategoria.ToString() // Enviamos la categoría como un simple texto
            //}).ToList();

            // 5. ENSAMBLAR EL VIEWMODEL
            var vm = new RecursoListViewModel
            {
                PaginaActual = pagina,
                TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)registrosPorPagina),
                FiltroCodigo = filtroCodigo,
                FiltroTipo = filtroTipo,
                FiltroEstado = filtroEstado,
                Tipos = tipos.Select(t => new SelectListItem(t.NombreTipoRecurso, t.IdTipoRecurso.ToString())).ToList(),
                Estados = estados.Select(e => new SelectListItem(e.NombreEstado, e.IdEstado.ToString())).ToList(),

                // AQUÍ ASIGNAMOS LA LISTA
                Responsables = listaResponsables,

                TiposConCategoria = tipos.Select(t => new SelectListItem
                {
                    Value = t.IdTipoRecurso.ToString(),
                    Text = t.NombreTipoRecurso,
                    Group = new SelectListGroup { Name = t.IdCategoria.ToString() }
                }).ToList(),

                Recursos = recursosQuery.Select(r =>
                {

                    int estadoCalculado = _recursoService.DeterminarEstadoFinal(r.IdEstado, r.IdUsuarioResponsable, r.IdTipoRecurso);
                    string? nombreFinal = (estadoCalculado == 6)
                    ? infoMantenimiento?.NombreEstado
                    : estados.FirstOrDefault(e => e.IdEstado == estadoCalculado)?.NombreEstado;

                    var entidadEstado = estados.FirstOrDefault(e => e.IdEstado == estadoCalculado);

                    var dto = new RecursoDto
                    {
                        IdRecurso = r.IdRecurso,
                        IdTipoRecurso = r.IdTipoRecurso,
                        CodigoInterno = r.CodigoInterno,
                        FechaAdquisicion = r.FechaAdquisicion,
                        Vigencia = r.Vigencia,
                        NombreTipoRecurso = r.TipoRecurso?.NombreTipoRecurso,
                        IdEstado = estadoCalculado,
                        NombreEstado = entidadEstado.NombreEstado,
                        IdUsuarioResponsable = r.IdUsuarioResponsable,
                        NombreCompletoResponsable = r.UsuarioResponsable?.NombreCompleto
                    };
                    if (string.IsNullOrEmpty(dto.IdUsuarioResponsable))
                    {
                        System.Diagnostics.Debug.WriteLine($"Alerta: El recurso {r.IdRecurso} no tiene ID de responsable en el mapeo.");
                    }
                    return new RecursoItemViewModel(dto, dto.NombreEstado);
                })
                .OrderBy(v => v.IdEstado == 5 ? 1 : v.IdEstado == 4 ? 2 : 3) // Orden visual de la página
                .ToList()
            };

            return View(vm);

        }


        [HttpGet] // Este responde al clic del enlace <a>
        public async Task<IActionResult> GetTipoRecurso()
        {
            var listaTipos = await _recursoService.ObtenerTiposAsync();

            // 2. Crear el ViewModel que espera la vista "Form"
            var model = new RecursoFormViewModel
            {
                // 3. Llenar la lista que creamos en el ViewModel con la categoría incluida
                TiposConCategoria = listaTipos.Select(t => new SelectListItem
                {
                    Value = t.IdTipoRecurso.ToString(),
                    Text = t.NombreTipoRecurso,
                    // Guardamos la categoría aquí para que el JS la pueda leer del HTML
                    Group = new SelectListGroup { Name = t.IdCategoria.ToString() }
                }).ToList()
            };

            return View("Form", model);
        }




        [HttpPost]
        public async Task<IActionResult> Liberar(int idRecurso)
        {
            var resultado = await _recursoService.LiberarRecursoServ(idRecurso);

            if (resultado)
            {
                TempData["Success"] = "El recurso ha sido liberado correctamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo liberar el recurso.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST /Recursos/Crear
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(RecursoFormViewModel model)
        {
            var estado = model.IdEstado;

            if (estado == null)
            {
                // 1 y 2 son Hardware (Equipo de cómputo / Servidores)
                estado = (model.IdTipoRecurso == 1 || model.IdTipoRecurso == 2) ? 5 : 1;
            }

            if (!ModelState.IsValid)
            {
                await RellenarDropdownsAsync(model);
                return View("Form", model);
            }

            try
            {
                var recurso = new Recurso
                {
                    CodigoInterno = model.CodigoInterno.Trim().ToUpper(),
                    IdTipoRecurso = model.IdTipoRecurso,
                    IdEstado = estado.Value,
                    FechaAdquisicion = model.FechaAdquisicion,
                    Vigencia = model.Vigencia,
                    CapacidadUso = model.CapacidadUso,
                    IdUsuarioCambio = ObtenerIdUsuarioActual(),
                    IdUsuarioResponsable = null
                };

                await _recursoService.CrearAsync(recurso);
                TempData["Exito"] = $"Recurso '{recurso.CodigoInterno}' creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Esto captura el mensaje real de SQL o EF
                var mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                ModelState.AddModelError("Error", $"No se pudo guardar: {mensajeError}");

                // IMPORTANTE: Aquí debes recargar los dropdowns antes de volver a la vista
                await RellenarDropdownsAsync(model);
                return View("Form", model);
            }
        }

        // GET /Recursos/Editar/{id}
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var recurso = await _recursoService.ObtenerPorIdAsync(id);
            if (recurso is null) return NotFound();

            var form = await ConstruirFormVmAsync(recurso);

            var Mostraresponsables = await _recursoService.ObtenerResponsablesParaDropdownAsync();

            // 2. Los convertimos a SelectListItem (esto quita el error de RuntimeBinderException)
            ViewBag.Usuarios = Mostraresponsables.Select(r => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = r.IdUsuario.ToString(), // El ID que se guardará
                Text = r.NombreCompleto   // El nombre que el usuario verá
            }).ToList();

            var listaTipos = await _recursoService.ObtenerTiposAsync();
            form.TiposConCategoria = listaTipos.Select(t => new SelectListItem
            {
                Value = t.IdTipoRecurso.ToString(),
                Text = t.NombreTipoRecurso,
                // No olvides el Group para que el JavaScript funcione
                Group = new SelectListGroup { Name = t.IdCategoria.ToString() }
            }).ToList();

            ViewBag.Tipos = form.TiposConCategoria;

            return View("Form", form);
        }

        // Helpers privados
        private async Task<RecursoFormViewModel> ConstruirFormVmAsync(Recurso? recurso)
        {
            var responsable = await _recursoService.ObtenerResponsablesParaDropdownAsync();
            var tipos = await _recursoService.ObtenerTiposAsync();
            var estados = await _recursoService.ObtenerEstadosAsync();

            var listaResponsables = responsable.Select(r => new SelectListItem
            {
                Value = r.IdUsuario.ToString(),
                Text = r.NombreCompleto
            }).ToList();

            return new RecursoFormViewModel
            {
                IdRecurso = recurso?.IdRecurso ?? 0,
                CodigoInterno = recurso?.CodigoInterno ?? string.Empty,
                IdTipoRecurso = recurso?.IdTipoRecurso ?? 0,
                IdEstado = recurso?.IdEstado ?? 1,
                FechaAdquisicion = recurso?.FechaAdquisicion ?? DateTime.Today,
                Vigencia = recurso?.Vigencia,
                IdUsuarioResponsable = recurso?.IdUsuarioResponsable,
                Tipos = tipos.Select(t => new SelectListItem(t.NombreTipoRecurso, t.IdTipoRecurso.ToString())),
                Estados = estados.Select(e => new SelectListItem(e.NombreEstado, e.IdEstado.ToString())),
                ListaResponsables = listaResponsables
            };
        }

        // POST /Recursos/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(RecursoFormViewModel model)
        {
            var estadoSeleccionado = model.IdEstado;

            if (!ModelState.IsValid)
            {
                await RellenarDropdownsAsync(model);
                var responsables = await _recursoService.ObtenerResponsablesParaDropdownAsync();

                ViewBag.Usuarios = responsables.Select(r => new SelectListItem
                {
                    Value = r.IdUsuario.ToString(),
                    Text = r.NombreCompleto

                }).ToList();

                return View("Form", model);
            }

            try
            {
                var recurso = new Recurso
                {
                    IdRecurso = model.IdRecurso,
                    CodigoInterno = model.CodigoInterno.Trim().ToUpper(),
                    IdTipoRecurso = model.IdTipoRecurso,
                    IdEstado = model.IdEstado ?? 1,
                    FechaAdquisicion = model.FechaAdquisicion,
                    Vigencia = model.Vigencia,
                    IdUsuarioCambio = ObtenerIdUsuarioActual(),
                    IdUsuarioResponsable = model.IdUsuarioResponsable

                };

                System.Diagnostics.Debug.WriteLine($"CAMBIO: El nuevo responsable es {recurso.IdUsuarioResponsable}");
                await _recursoService.ActualizarAsync(recurso);
                TempData["Exito"] = $"Recurso '{recurso.CodigoInterno}' actualizado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var mensajeError = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("Error", ex.Message);
                ModelState.AddModelError("Error", $"No se pudo guardar: {mensajeError}");
                await RellenarDropdownsAsync(model);
                return View("Form", model);
            }
        }

        // POST /Recursos/Eliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                await _recursoService.EliminarAsync(id);
                TempData["Exito"] = "Recurso eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> EliminarTipoRecurso(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "ID de tipo no válido." });
            }

            try
            {
                // Llamamos al servicio para ejecutar la lógica de eliminación
                bool eliminado = await _recursoService.EliminarTipoRecursoAsync(id);

                if (eliminado)
                {
                    return Json(new { success = true, message = "El tipo de recurso ha sido eliminado." });
                }
                else
                {
                    // El servicio devuelve false si el tipo está en uso o no existe
                    return Json(new { success = false, message = "No se puede eliminar: el tipo está asociado a recursos existentes o no se encontró." });
                }
            }
            catch (Exception ex)
            {
                // Log del error para depuración
                System.Diagnostics.Debug.WriteLine($"Error al eliminar tipo: {ex.Message}");
                return Json(new { success = false, message = "Ocurrió un error interno en el servidor." });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PonerEnMantenimiento(int idRecurso)
        {
            // 1. Llamamos al servicio para cambiar el estado a 6
            bool exito = await _recursoService.PonerEnMantenimientoAsync(idRecurso);

            if (exito)
            {
                TempData["Success"] = "El recurso ahora está en mantenimiento.";
            }
            else
            {
                TempData["Error"] = "No se pudo actualizar el estado del recurso.";
            }

            // 2. Redirigimos al Index para ver el cambio
            return RedirectToAction(nameof(Index));
        }



        private async Task RellenarDropdownsAsync(RecursoFormViewModel model)
        {
            var tipos = await _recursoService.ObtenerTiposAsync();
            var estados = await _recursoService.ObtenerEstadosAsync();
            var responsables = await _recursoService.ObtenerResponsablesParaDropdownAsync();

            // Llenamos el ViewBag que la vista espera
            ViewBag.Tipos = new SelectList(tipos, "IdTipoRecurso", "NombreTipoRecurso", model.IdTipoRecurso);
            ViewBag.Estados = new SelectList(estados, "IdEstado", "NombreEstado", model.IdEstado);

            ViewBag.Responsables = responsables.Select(r => new SelectListItem
            {
                Value = r.IdUsuario.ToString(),
                Text = r.NombreCompleto
            }).ToList();
        }

        [HttpPost]
        public async Task<IActionResult> AsignarResponsable([FromForm] int idRecurso, [FromForm] string idUsuario)
        {
            string idLogueado = ObtenerIdUsuarioActual();

            // 1. Solo ejecutamos la acción de guardado
            var exito = await _recursoService.AsignarResponsableAsync(idRecurso, idUsuario,idLogueado);

            if (exito)
            {
                // 2. Respondemos con un simple "Ok". 
                // No necesitamos buscar nombres ni devolver JSON complejo.
                return Ok();
            }

            return BadRequest("No se pudo asignar el responsable.");
        }

        /*ENDPOINT CREAR TIPO RECURSO*/

        [HttpPost]
        public async Task<IActionResult> CrearTipoRecurso(CrearTipoRecursoDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            // Llamamos al service
            var resultado = await _recursoService.RegistrarNuevoTipoAsync(dto);

            if (resultado == null)
            {
                TempData["Error"] = "El nombre del recurso ya existe.";
                return RedirectToAction("Index");
            }

            var categorias = await _recursoService.ObtenerCategoriasAsync();

            ViewBag.Categorias = categorias.Select(c => new SelectListItem
            {
                Value = c.IdCategoria.ToString(),
                Text = c.Nombre
            }).ToList();

            // Puedes pasar este viewModel a una vista parcial o refrescar la lista
            return RedirectToAction("Index");
        }


        /*ENDPOINT PARA OBTENER LOS DATOS DE TIPO RECURSO*/
        [HttpGet]
        public async Task<IActionResult> GetDatosTipoRecurso(int id)
        {
            // 1. Llamamos al service (que crearemos en el siguiente paso)
            var resultado = await _recursoService.ObtenerTipoRecursoPorIdAsync(id);

            // 2. Si el service no encuentra nada, mandamos un error 404
            if (resultado == null)
            {
                return NotFound(new { message = "No se encontró el tipo de recurso solicitado." });
            }

            // 3. Respondemos con los datos en formato JSON
            // Esto es lo que el JavaScript recibirá en el 'data' del fetch
            return Json(new
            {
                id = resultado.IdTipoRecurso,
                nombre = resultado.NombreTipoRecurso,
                idCategoria = resultado.IdCategoria
            });
        }


        /*ENDPOINT PARA EDITAR LOS DATOS NUEVOS  QUE INGRESAMOS DE TIPO RECURSO*/
        [HttpPost]
        public async Task<IActionResult> EditarTipoRecurso([FromBody] TipoRecursoDto modelo)
        {
            if (modelo == null || string.IsNullOrEmpty(modelo.Nombre))
                return BadRequest("Datos inválidos");

            // Llamamos al service para actualizar
            bool exito = await _recursoService.ActualizarTipoRecursoAsync(modelo);

            if (exito) return Ok();

            return StatusCode(500, "No se pudo actualizar en la base de datos");
        }

    }
}
