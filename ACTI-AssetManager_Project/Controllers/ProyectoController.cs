using ACTI_AssetManager_Project.Application.DTOs;
using ACTI_AssetManager_Project.Application.Interfaces;
using ACTI_AssetManager_Project.Models.Proyecto;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ACTI_AssetManager_Project.Controllers
{
    public class ProyectoController : Controller
    {
        private readonly IProyectoService _proyectoService;

        public ProyectoController(IProyectoService proyectoService)
        {
            _proyectoService = proyectoService;
        }

        public async Task<IActionResult> Index()
        {
            var proyectosEntidad = await _proyectoService.ObtenerTodosLosProyectosAsync();

            // 2. CREAR E INSTANCIAR el ViewModel (Esto es lo que te falta)
            var viewModel = new ProyectoViewModel
            {
                TotalProyectos = proyectosEntidad.Count(),
                // Mapeamos las entidades a los items del ViewModel
                Proyectos = proyectosEntidad.Select(p => new ProyectoItemViewModel
                {
                    IdProyecto = p.IdProyecto,
                    NombreProyecto = p.NombreProyecto,
                    Descripcion = p.Descripcion,
                }).ToList() // Importante convertirlo a lista
            };

            // 3. PASAR EL MODELO A LA VISTA
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromForm] ProyectoDto dto)
        {
            if (!ModelState.IsValid) return Json(new { success = false, message = "Datos inválidos" });

            // Obtenemos el ID del usuario logueado (matrícula/ID)
            string? idUsuarioCambio = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var resultado = await _proyectoService.RegistrarProyectoAsync(dto, idUsuarioCambio);

            if (resultado)
                return Json(new { success = true, message = "Proyecto guardado con éxito" });

            return Json(new { success = false, message = "Error al guardar en la base de datos" });
        }

    }
}
