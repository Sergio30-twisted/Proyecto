using Moq;
using ACTI_AssetManager_Project.Application.Services;
using ACTI_AssetManager_Project.Application.Data;
using ACTI_AssetManager_Project.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ACTI_AssetManager_Project.Tests
{
    /// <summary>
    /// Pruebas de funcionalidad para el Login (casos felices y tristes).
    /// Cubre: credenciales válidas, contraseña incorrecta, usuario inexistente y campos vacíos.
    /// </summary>
    public class LoginTests
    {
        // ──────────────────────────────────────────────
        // Helpers compartidos por todos los tests
        // ──────────────────────────────────────────────

        private static AM_DBContext CrearContextoConUsuarios(params Usuario[] usuarios)
        {
            var opciones = new DbContextOptionsBuilder<AM_DBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var contexto = new AM_DBContext(opciones);
            contexto.Usuarios.AddRange(usuarios);
            contexto.SaveChanges();
            return contexto;
        }

        private static IConfiguration CrearConfiguracion()
        {
            var config = new Mock<IConfiguration>();
            config.Setup(c => c["JwtSettings:Key"])
                  .Returns("k8#Mv2@rL5!pX9$zQ4*nJ0^bF7&wT1+sD3");
            config.Setup(c => c["JwtSettings:Issuer"])
                  .Returns("ACTI-AssetManager-Prod");
            config.Setup(c => c["JwtSettings:Audience"])
                  .Returns("ACTI-AssetManager-WebUI");
            return config.Object;
        }

        private static string HashearPassword(string pwd)
            => BCrypt.Net.BCrypt.HashPassword(pwd);


        // ══════════════════════════════════════════════════════════════════════
        // CASO 1 – CREDENCIALES VÁLIDAS
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public void ValidarYGenerarToken_CredencialesCorrectas_RetornaTokenJwt()
        {
            const string username = "Juan Pablo";
            const string clave = "Clave@123";

            var usuario = new Usuario
            {
                IdUsuario = "USR001",
                NombreCompleto = username,
                PasswordHash = HashearPassword(clave),
                Eliminado = false,
                IdUsuarioCambio = "USR_TEST",
                FechaHoraCambio = DateTime.Now
            };

            var contexto = CrearContextoConUsuarios(usuario);
            var servicio = new AuthService(CrearConfiguracion(), contexto);

            var token = servicio.ValidarYGenerarToken(username, clave);

            Assert.NotNull(token);
            Assert.NotEmpty(token);
            Assert.Equal(2, token.Count(c => c == '.'));
        }


        // ══════════════════════════════════════════════════════════════════════
        // CASO 2 – CONTRASEÑA INCORRECTA
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public void ValidarYGenerarToken_ConPasswordIncorrecto_LanzaExcepcion()
        {
            const string username = "Juan Pablo";

            var usuario = new Usuario
            {
                IdUsuario = "USR001",
                NombreCompleto = username,
                PasswordHash = HashearPassword("PasswordCorrecto@1"),
                Eliminado = false,
                IdUsuarioCambio = "USR_TEST",
                FechaHoraCambio = DateTime.Now
            };

            var contexto = CrearContextoConUsuarios(usuario);
            var servicio = new AuthService(CrearConfiguracion(), contexto);

            var ex = Assert.Throws<Exception>(
                () => servicio.ValidarYGenerarToken(username, "PasswordIncorrecto@1"));

            Assert.NotNull(ex.Message);
            Assert.NotEmpty(ex.Message);
        }

        [Fact]
        public void ValidarYGenerarToken_ConPasswordIncorrecto_MensajeEsSeguro()
        {
            const string username = "Juan Pablo";

            var usuario = new Usuario
            {
                IdUsuario = "USR001",
                NombreCompleto = username,
                PasswordHash = HashearPassword("CorrectPass1!"),
                Eliminado = false,
                IdUsuarioCambio = "USR_TEST",
                FechaHoraCambio = DateTime.Now
            };

            var contexto = CrearContextoConUsuarios(usuario);
            var servicio = new AuthService(CrearConfiguracion(), contexto);

            var ex = Assert.Throws<Exception>(
                () => servicio.ValidarYGenerarToken(username, "WrongPass1!"));

            Assert.DoesNotContain("usuario existe", ex.Message, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Usuario no existe", ex.Message, StringComparison.OrdinalIgnoreCase);
        }


        // ══════════════════════════════════════════════════════════════════════
        // CASO 3 – USUARIO INEXISTENTE
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public void ValidarYGenerarToken_UsuarioInexistente_LanzaExcepcion()
        {
            var contexto = CrearContextoConUsuarios();
            var servicio = new AuthService(CrearConfiguracion(), contexto);

            var ex = Assert.Throws<Exception>(
                () => servicio.ValidarYGenerarToken("UsuarioQueNoExiste", "cualquierPassword"));

            Assert.NotNull(ex.Message);
        }

        [Fact]
        public void ValidarYGenerarToken_UsuarioEliminado_LanzaExcepcion()
        {
            const string username = "Carlos Eliminado";

            var usuarioEliminado = new Usuario
            {
                IdUsuario = "USR999",
                NombreCompleto = username,
                PasswordHash = HashearPassword("Pass@123"),
                Eliminado = true,
                IdUsuarioCambio = "USR_TEST",
                FechaHoraCambio = DateTime.Now
            };

            var contexto = CrearContextoConUsuarios(usuarioEliminado);
            var servicio = new AuthService(CrearConfiguracion(), contexto);

            Assert.Throws<Exception>(
                () => servicio.ValidarYGenerarToken(username, "Pass@123"));
        }


        // ══════════════════════════════════════════════════════════════════════
        // CASO 4 – CAMPOS VACÍOS
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public void ValidarYGenerarToken_UsernameVacio_LanzaExcepcion()
        {
            var contexto = CrearContextoConUsuarios();
            var servicio = new AuthService(CrearConfiguracion(), contexto);

            Assert.Throws<Exception>(
                () => servicio.ValidarYGenerarToken("", "cualquierPassword"));
        }

        [Fact]
        public void ValidarYGenerarToken_PasswordVacio_LanzaExcepcion()
        {
            const string username = "Juan Pablo";

            var usuario = new Usuario
            {
                IdUsuario = "USR001",
                NombreCompleto = username,
                PasswordHash = HashearPassword("Pass@123"),
                Eliminado = false,
                IdUsuarioCambio = "USR_TEST",
                FechaHoraCambio = DateTime.Now
            };

            var contexto = CrearContextoConUsuarios(usuario);
            var servicio = new AuthService(CrearConfiguracion(), contexto);

            Assert.Throws<Exception>(
                () => servicio.ValidarYGenerarToken(username, ""));
        }

        [Fact]
        public void AuthViewModel_CamposVacios_FallaValidacionDeAnotaciones()
        {
            var modelo = new AuthViewModel_Test
            {
                Username = "",
                Password = ""
            };

            var resultados = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var contexto = new System.ComponentModel.DataAnnotations.ValidationContext(modelo);

            bool esValido = System.ComponentModel.DataAnnotations.Validator
                                .TryValidateObject(modelo, contexto, resultados, true);

            Assert.False(esValido);
            Assert.Contains(resultados,
                r => r.MemberNames.Contains(nameof(AuthViewModel_Test.Username)));
            Assert.Contains(resultados,
                r => r.MemberNames.Contains(nameof(AuthViewModel_Test.Password)));
        }
    }

    internal class AuthViewModel_Test
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El usuario es obligatorio.")]
        public string Username { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; } = string.Empty;
    }
}