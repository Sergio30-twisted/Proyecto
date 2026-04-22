using ACTI_AssetManager_Project.Application.DTOs;
using ACTI_AssetManager_Project.Application.Interfaces;
using ACTI_AssetManager_Project.Application.Services;
using ACTI_AssetManager_Project.Domain.Entities;
using ACTI_AssetManager_Project.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Tests
{
    public class AsignarRecursoTest
    {
        private readonly Mock<IAsignacionRepository> _repoMock;
        private readonly AsignacionService _service;

        public AsignarRecursoTest()
        {
            // Inicializamos el Mock y el Servicio
            _repoMock = new Mock<IAsignacionRepository>();
            _service = new AsignacionService(_repoMock.Object);
        }

        [Fact]
        public async Task CrearAsignacion_CuandoDatosSonValidos_RetornaTrueYGuarda()
        {
            // ARRANGE
            var dto = new AsignacionDto
            {
                IdProyecto = 1,
                RecursosIds = new List<int> { 101 },
                UsuariosIds = new List<string> { "User01" },
                IdUsuarioRegistro = "Admin"
            };
            _repoMock.Setup(r => r.CrearAsignacionAsync(It.IsAny<Asignacion>())).ReturnsAsync(true);

            // ACT
            var resultado = await _service.CrearAsignacionAsync(dto);

            // ASSERT
            Assert.True(resultado);
            _repoMock.Verify(r => r.CrearAsignacionAsync(It.IsAny<Asignacion>()), Times.Once);
        }

        [Fact]
        public async Task CrearAsignacion_CuandoNoHayRecursos_RetornaFalseYNoGuarda()
        {
            // ARRANGE
            var dto = new AsignacionDto
            {
                RecursosIds = new List<int>() // Lista vacía para disparar el IF
            };

            // ACT
            var resultado = await _service.CrearAsignacionAsync(dto);

            // ASSERT
            // Aquí la prueba PASA si el resultado es falso y el repo no se tocó
            Assert.False(resultado);
            _repoMock.Verify(r => r.CrearAsignacionAsync(It.IsAny<Asignacion>()), Times.Never);
        }



    }
}
