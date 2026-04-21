using ACTI_AssetManager_Project.Application.Data;
using ACTI_AssetManager_Project.Application.Interfaces;
using ACTI_AssetManager_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ACTI_AssetManager_Project.Controllers
{
    [AllowAnonymous]
    //"AllowAnonymous dice que cualquiera puede entrar "
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authservice)
        {   
            _authService = authservice;
        }

        public IActionResult Index()
        {
            return View();
        }
       
        [HttpPost]
        public async Task<IActionResult> Login(AuthViewModel model)
        {
            if (!ModelState.IsValid) return View("Index", model);

            try
            {
                // El servicio valida y nos devuelve el token con el usuario ya incrustado
                var token = _authService.ValidarYGenerarToken(model.Username, model.Password);

                // Guardamos el token en la cookie segura
                Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // En IIS con SSL esto es vital
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(8)
                });

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Si el servicio lanzó "Usuario no existe", aquí lo atrapamos
                ModelState.AddModelError("AuthError", ex.Message);
                return View("Index", model);
            }
        }

        public IActionResult Logout()
        {
            // Borra la cookie del navegador
            Response.Cookies.Delete("AuthToken");

            // Redirige al Index de este mismo controlador (Auth)
            return RedirectToAction("Index", "Auth");
        }
    }
}
