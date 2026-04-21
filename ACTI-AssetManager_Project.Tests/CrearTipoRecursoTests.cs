using ACTI_AssetManager_Project.Application.DTOs;
using ACTI_AssetManager_Project.Application.Interfaces;
using ACTI_AssetManager_Project.Application.Services;
using ACTI_AssetManager_Project.Domain.Entities;
using ACTI_AssetManager_Project.Domain.Interfaces;
using Moq;
using Xunit;

namespace ACTI_AssetManager_Project.Tests
{
    public class CrearTipoRecursoTests
    {
        private readonly Mock<IRecursoRepository> _repoMock;
        private readonly RecursoService _service;

        public CrearTipoRecursoTests()
        {
            _repoMock = new Mock<IRecursoRepository>();

            // RecursoService requiere IRecursoRepository e ITipoRecursoRepository
            _service = new RecursoService(
                _repoMock.Object,
                new Mock<IResponsableRecursoService>().Object,
                new Mock<ITipoRecursoRepository>().Object
                
            );
        }

        //CASO 1: Registro Exitoso
        [Fact]
        public async Task RegistrarNuevoTipo_NombreUnico_RetornaTipoRecurso()
        {
            _repoMock.Setup(r => r.ExisteTipoNombreAsync("Electrónica")).ReturnsAsync(false);
            _repoMock.Setup(r => r.GuardarTipoRecursoAsync(It.IsAny<TipoRecurso>()))
                     .Returns(Task.CompletedTask);

            var dto = new CrearTipoRecursoDto { Nombre = "Electrónica", IdCategoria = 1 };
            var resultado = await _service.RegistrarNuevoTipoAsync(dto);

            Assert.NotNull(resultado);
            Assert.Equal("Electrónica", resultado.NombreTipoRecurso);
        }

        //CASO 2: Campo Obligatorio nombre vacío
        [Fact]
        public async Task RegistrarNuevoTipo_NombreVacio_RetornaNull()
        {
            var dto = new CrearTipoRecursoDto { Nombre = "", IdCategoria = 1 };
            var resultado = await _service.RegistrarNuevoTipoAsync(dto);

            Assert.Null(resultado);
        }

        //CASO 3: Evitar Duplicados
        [Fact]
        public async Task RegistrarNuevoTipo_NombreDuplicado_RetornaNull()
        {
            _repoMock.Setup(r => r.ExisteTipoNombreAsync("Mobiliario")).ReturnsAsync(true);

            var dto = new CrearTipoRecursoDto { Nombre = "Mobiliario", IdCategoria = 1 };
            var resultado = await _service.RegistrarNuevoTipoAsync(dto);

            Assert.Null(resultado);
        }

        //CASO 4: Límite de Caracteres más de 50
        [Fact]
        public async Task RegistrarNuevoTipo_NombreMuyLargo_RetornaNull()
        {
            var nombreLargo = new string('A', 51); // 51 caracteres
            var dto = new CrearTipoRecursoDto { Nombre = nombreLargo, IdCategoria = 1 };
            var resultado = await _service.RegistrarNuevoTipoAsync(dto);

            Assert.Null(resultado);
        }

        //CASO 5: Espacios en Blanco
        [Fact]
        public async Task RegistrarNuevoTipo_SoloEspacios_RetornaNull()
        {
            var dto = new CrearTipoRecursoDto { Nombre = "     ", IdCategoria = 1 };
            var resultado = await _service.RegistrarNuevoTipoAsync(dto);

            Assert.Null(resultado);
        }
    }
}
