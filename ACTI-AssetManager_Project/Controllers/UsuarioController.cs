using ACTI_AssetManager_Project.Application.DTOs;
using ACTI_AssetManager_Project.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ACTI_AssetManager_Project.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        private string? ObtenerIdUsuarioActual()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromForm] AgregarUsuarioDto dto)
        {
            if (!ModelState.IsValid) return BadRequest("Datos inválidos");

            // Obtenemos el ID del admin que está operando
            string? idAdmin = ObtenerIdUsuarioActual();

            if (string.IsNullOrEmpty(idAdmin))
                return Unauthorized("No se pudo identificar al administrador actual");

            // Pasamos el DTO y el ID del admin al servicio
            var resultado = await _usuarioService.RegistrarUsuarioAsync(dto, idAdmin);

            if (resultado)
                return Json(new { success = true, message = "Usuario registrado correctamente" });

            return Json(new { success = false, message = "Error al registrar: El ID de usuario ya existe" });
        }
    }
}
