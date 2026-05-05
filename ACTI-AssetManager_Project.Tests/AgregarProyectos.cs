using ACTI_AssetManager_Project.Application.DTOs;
using ACTI_AssetManager_Project.Application.Services;
using ACTI_AssetManager_Project.Domain.Entities;
using ACTI_AssetManager_Project.Domain.Interfaces;
using Moq;
using Xunit;

namespace ACTI_AssetManager_Project.Tests.Services
{
    public class ProyectoServiceTests
    {
        private readonly Mock<IProyectoRepository> _repoMock;
        private readonly ProyectoService _service;

        public ProyectoServiceTests()
        {
            _repoMock = new Mock<IProyectoRepository>();
            _service = new ProyectoService(_repoMock.Object);
        }

        #region Pruebas de Obtención

        [Fact]
        public async Task ObtenerTodosLosProyectosAsync_DebeRetornarListaDeDtos()
        {
            // Arrange
            var proyectosFake = new List<Proyecto>
            {
                new Proyecto { IdProyecto = 1, NombreProyecto = "Proyecto A" },
                new Proyecto { IdProyecto = 2, NombreProyecto = "Proyecto B" }
            };

            /*
             Aquí decimos "Cuando el servicio llame al método ObtenerTodosAsync 
             del repositorio, no vayas a la base de datos; simplemente devuélvele 
             la lista proyectosFake que acabo de crear".
             */
            _repoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(proyectosFake);

            // Act
            var resultado = await _service.ObtenerTodosLosProyectosAsync();

            // Assert
            Assert.Equal(2, resultado.Count());
            Assert.Contains(resultado, p => p.NombreProyecto == "Proyecto A");
        }

        [Fact]
        public async Task ObtenerPorIdAsync_SiEstaEliminado_DebeRetornarNull()
        {
            // Arrange
            var proyectoEliminado = new Proyecto { IdProyecto = 1, ELIMINADO = true };
            _repoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(proyectoEliminado);

            // Act
            var resultado = await _service.ObtenerPorIdAsync(1);

            // Assert
            Assert.Null(resultado);
        }

        #endregion

        #region Pruebas de Registro

        [Fact]
        public async Task RegistrarProyectoAsync_DebeMapearCorrectamenteYGuardar()
        {
            // Arrange
            var dto = new ProyectoDto { NombreProyecto = "Nuevo", Descripcion = "Desc" };

            //Crea el objeto que finge ser el repositorio
            //Cuando alquien te llame y te pase cualquier proyecto, tu limitate a responder que si.
            _repoMock.Setup(r => r.AgregarAsync(It.IsAny<Proyecto>())).ReturnsAsync(true);

            // Act
            var resultado = await _service.RegistrarProyectoAsync(dto, "admin-id");

            // Assert
            Assert.True(resultado);
            //Aquí decimos Un momento, a mí me configuraron para recibir un Proyecto, y ya se valida
            _repoMock.Verify(r => r.AgregarAsync(It.Is<Proyecto>(p =>
                p.NombreProyecto == dto.NombreProyecto &&
                p.ELIMINADO == false
            )), Times.Once);
        }

        #endregion

        #region Pruebas de Actualización y Eliminación

        [Fact]
        public async Task ActualizarProyectoAsync_SiExiste_DebeActualizarCamposEspecificos()
        {
            // Arrange
            var proyectoExistente = new Proyecto { IdProyecto = 10, NombreProyecto = "Viejo", Descripcion = "Vieja" };
            var dtoEdicion = new ProyectoDto { IdProyecto = 10, NombreProyecto = "Nuevo Nombre", Descripcion = "Nueva Desc" };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(10)).ReturnsAsync(proyectoExistente);
            _repoMock.Setup(r => r.ActualizarAsync(It.IsAny<Proyecto>())).ReturnsAsync(true);

            // Act
            var resultado = await _service.ActualizarProyectoAsync(dtoEdicion);

            // Assert
            Assert.True(resultado);
            /*
             Con el verirfy comprobamos si se intento llamar al metodo
             y corrobora si los valores que queremos actualizar fueron los que se le pasaron al metodo actualizar del repositorio

            El time.once dice Debe ocurrir exactamente 1 vez. 
             */
            _repoMock.Verify(r => r.ActualizarAsync(It.Is<Proyecto>(p =>
                p.NombreProyecto == "Nuevo Nombre" &&
                p.Descripcion == "Nueva Desc" &&
                p.IdProyecto == 10
            )), Times.Once);
        }

        [Fact]
        public async Task EliminarProyectoAsync_DebeCambiarEstadoA_ELIMINADO()
        {
            // Arrange
            var proyecto = new Proyecto { IdProyecto = 5, ELIMINADO = false };
            _repoMock.Setup(r => r.ObtenerPorIdAsync(5)).ReturnsAsync(proyecto);
            _repoMock.Setup(r => r.ActualizarAsync(It.IsAny<Proyecto>())).ReturnsAsync(true);

            // Act
            var resultado = await _service.EliminarProyectoAsync(5);

            // Assert
            Assert.True(resultado);
            // Verificamos que se llamó a Actualizar con el flag ELIMINADO en true
            _repoMock.Verify(r => r.ActualizarAsync(It.Is<Proyecto>(p =>
                p.IdProyecto == 5 &&
                p.ELIMINADO == true
            )), Times.Once);
        }

        [Fact]
        public async Task EliminarProyectoAsync_SiNoExiste_DebeRetornarFalse()
        {
            // Arrange
            _repoMock.Setup(r => r.ObtenerPorIdAsync(It.IsAny<int>())).ReturnsAsync((Proyecto)null);

            // Act
            var resultado = await _service.EliminarProyectoAsync(99);

            // Assert
            Assert.False(resultado);
            _repoMock.Verify(r => r.ActualizarAsync(It.IsAny<Proyecto>()), Times.Never);
        }

        #endregion
    }
}