using ACTI_AssetManager_Project.Application.DTOs;
using ACTI_AssetManager_Project.Application.Services;
using ACTI_AssetManager_Project.Domain.Entities;
using ACTI_AssetManager_Project.Domain.Interfaces;
using Moq;
using Xunit;

namespace ACTI_AssetManager_Project.Tests.Services
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepository> _repositoryMock;
        private readonly UsuarioService _service;

        public UsuarioServiceTests()
        {
            // Creamos el mock del repositorio
            _repositoryMock = new Mock<IUsuarioRepository>();
            // Inyectamos el mock en el servicio
            _service = new UsuarioService(_repositoryMock.Object);
        }

        [Fact]
        public async Task RegistrarUsuarioAsync_CuandoEsAdmin_DebeMapearCorrectamenteYRetornarTrue()
        {
            // Arrange (Preparar)
            var dto = new AgregarUsuarioDto
            {
                IdUsuario = "U001",
                Nombre = "Admin Test",
                Email = "admin@test.com",
                Password = "Password123",
                Rol = "Admin"
            };
            string idAdminCreador = "ADMIN_SISTEMA";

            _repositoryMock.Setup(repo => repo.AgregarAsync(It.IsAny<Usuario>()))
                           .ReturnsAsync(true);

            // Act (Actuar)
            var resultado = await _service.RegistrarUsuarioAsync(dto, idAdminCreador);

            // Assert (Verificar)
            Assert.True(resultado);
            _repositoryMock.Verify(repo => repo.AgregarAsync(It.Is<Usuario>(u =>
                u.IdUsuario == dto.IdUsuario &&
                u.SuperUsuario == true &&
                u.IdTipoUsuario == 1 &&
                u.IdUsuarioCambio == idAdminCreador &&
                !string.IsNullOrEmpty(u.PasswordHash)
            )), Times.Once);
        }

        [Fact]
        public async Task RegistrarUsuarioAsync_CuandoEsUsuarioNormal_DebeAsignarRolCorrecto()
        {
            // Arrange
            var dto = new AgregarUsuarioDto
            {
                IdUsuario = "U002",
                Nombre = "User Test",
                Email = "user@test.com",
                Password = "Password123",
                Rol = "User" // No es Admin
            };

            _repositoryMock.Setup(repo => repo.AgregarAsync(It.IsAny<Usuario>()))
                           .ReturnsAsync(true);

            // Act
            await _service.RegistrarUsuarioAsync(dto, "ADMIN01");

            // Assert
            _repositoryMock.Verify(repo => repo.AgregarAsync(It.Is<Usuario>(u =>
                u.SuperUsuario == false &&
                u.IdTipoUsuario == 2
            )), Times.Once);
        }

        [Fact]        
        public async Task RegistrarUsuarioAsync_LaContrasenaDebeEstarHasheada()
        {
            // Arrange
            var dto = new AgregarUsuarioDto
            {
                IdUsuario = "U003",
                Nombre = "Hash Test",
                Email = "hash@test.com",
                Password = "MiPasswordSeguro",
                Rol = "User"
            };

            Usuario usuarioCapturado = null;

            // Configuramos el mock para que "guarde" el usuario que recibe
            _repositoryMock.Setup(repo => repo.AgregarAsync(It.IsAny<Usuario>()))
                           .Callback<Usuario>(u => usuarioCapturado = u) // Captura
                           .ReturnsAsync(true);

            // Act
            await _service.RegistrarUsuarioAsync(dto, "ADMIN01");

            // Assert
            Assert.NotNull(usuarioCapturado);
            Assert.NotEqual(dto.Password, usuarioCapturado.PasswordHash);

            // Ahora sí verificamos el Hash con BCrypt fuera del Mock
            bool esValido = BCrypt.Net.BCrypt.Verify(dto.Password, usuarioCapturado.PasswordHash);
            Assert.True(esValido, "El hash generado no coincide con la contraseña original.");
        }



        [Fact]
        public async Task RegistrarUsuarioAsync_SiRepositorioFalla_DebeRetornarFalse()
        {
            // Arrange
            var dto = new AgregarUsuarioDto { Password = "123", Rol = "User" };
            _repositoryMock.Setup(repo => repo.AgregarAsync(It.IsAny<Usuario>()))
                           .ReturnsAsync(false);

            // Act
            var resultado = await _service.RegistrarUsuarioAsync(dto, "ADMIN01");

            // Assert
            Assert.False(resultado);
        }
    }
}