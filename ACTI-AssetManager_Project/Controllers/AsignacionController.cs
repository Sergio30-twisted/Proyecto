using ACTI_AssetManager_Project.Application.DTOs;
using ACTI_AssetManager_Project.Application.Interfaces;
using ACTI_AssetManager_Project.Application.Services;
using ACTI_AssetManager_Project.Domain.Entities;
using ACTI_AssetManager_Project.Models;
using ACTI_AssetManager_Project.Models.Asignacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ACTI_AssetManager_Project.Controllers
{
    [Authorize]
   
    public class AsignacionController : Controller
    {
        private readonly IAsignacionService _asigService;
        private readonly IRecursoService _recursoService;
        private readonly IProyectoService _proyectoService;
        private readonly IResponsableRecursoService _responsableService;

        public AsignacionController(IAsignacionService asigService, IRecursoService recursoService, IProyectoService proyectoService, IResponsableRecursoService responsableService)
        {
            _asigService = asigService;
            _recursoService = recursoService;
            _proyectoService = proyectoService;
            _responsableService = responsableService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var viewModel = new AsignacionViewModel();

            // Opcional: Si quieres que el select cargue desde el inicio sin esperar al JS:
            var recursos = await _recursoService.ObtenerRecursosDisponiblesAsync();
            viewModel.RecursosDisponibles = recursos.Select(r => new SelectListItem
            {
                Value = r.IdRecurso.ToString(),
                Text = r.CodigoInterno
            }).ToList();

            var responsables = await _recursoService.ObtenerResponsablesParaDropdownAsync();
            viewModel.Responsables = responsables.Select(r => new SelectListItem
            {
                Value = r.IdUsuario.ToString(),
                Text = r.NombreCompleto
            }).ToList();

            viewModel.AsignacionesRealizadas = await _asigService.ObtenerTodasLasAsignacionesAsync();


            return View(viewModel);

        }

        private string? ObtenerIdUsuarioActual()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private async Task RecargarListas(AsignacionViewModel model)
        {
            var recursos = await _recursoService.ObtenerRecursosDisponiblesAsync();
            model.RecursosDisponibles = recursos.Select(r => new SelectListItem
            {
                Value = r.IdRecurso.ToString(),
                Text = r.CodigoInterno
            }).ToList();

            var responsables = await _recursoService.ObtenerResponsablesParaDropdownAsync();
            model.Responsables = responsables.Select(r => new SelectListItem
            {
                Value = r.IdUsuario.ToString(),
                Text = r.NombreCompleto
            }).ToList();
        }


        [HttpPost]
        public async Task<IActionResult> CrearAsignacion(AsignacionViewModel model)
        {
            // 1. Validación de anotaciones del modelo
            if (!ModelState.IsValid)
            {
                // Importante: Si tu vista necesita cargar selects nuevamente, hazlo aquí
                await RecargarListas(model);
                return View("Index", model);
            }

            string IdUsuarioActual = ObtenerIdUsuarioActual();

            try
            {
                // 2. Mapeo Manual: Convertimos el ViewModel al DTO
                // Aquí ya no usamos _context.Database.BeginTransaction
                var asignacionDto = new AsignacionDto
                {
                    IdProyecto = null, // Usa el valor del modelo, no null
                    FechaInicio = model.FechaInicio,
                    FechaFin = model.FechaFin,
                    IdUsuarioRegistro = IdUsuarioActual, // Reemplazar por el ID del usuario logueado (ej. User.GetId())

                    // Enviamos las dos listas que vienen de los inputs hidden de tus cartas
                    UsuariosIds = model.UsuariosSeleccionadosIds ?? new List<string>(),
                    RecursosIds = model.RecursosSeleccionadosIds ?? new List<int>()
                };

                // 3. Llamada única al servicio
                var resultado = await _asigService.CrearAsignacionAsync(asignacionDto);

                if (resultado)
                {
                    TempData["Success"] = "Asignación creada con éxito.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "No se pudo guardar la asignación. Revisa la disponibilidad de los recursos.");
            }
            catch (Exception ex)
            {
                // 4. Manejo de errores detallado
                var mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                ModelState.AddModelError("", "Error en el servidor: " + mensajeError);
            }

            // Si algo falló, regresamos a la vista con el modelo para no perder los datos
            await RecargarListas(model);
            return View("Index", model);

        }

        [HttpGet]
        [Route("Asignacion/ObtenerDetallesResponsables")]
        public async Task<IActionResult> ObtenerDetallesPorAsignacionAsync(int idAsignacion)
        {
            try
            {
                var detalles = await _asigService.ObtenerDetallesPorAsignacionAsync(idAsignacion);
                return Json(detalles); // Retorna el JSON limpio
            }
            catch (Exception ex)
            {
                // Esto evita que el JS reciba un HTML de error y vea el "Token S"
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarLogica(int id)
        {
            try
            {
                var resultado = await _asigService.EliminarAsignacionLogicaAsync(id);
                if (resultado)
                {
                    return Json(new { success = true, message = "Asignación eliminada correctamente." });
                }
                return Json(new { success = false, message = "No se pudo encontrar la asignación." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerInformacionAsignacion(int id)
        {
            var data = await _asigService.GetAsignacionDetalleJsonAsync(id);

            if (data == null) return NotFound(new { message = "No se encontró la asignación" });

            return Json(data);
        }

    }
}
