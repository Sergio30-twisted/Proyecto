using Moq;
using ACTI_AssetManager_Project.Application.DTOs;
using ACTI_AssetManager_Project.Application.Services;
using ACTI_AssetManager_Project.Domain.Entities;
using ACTI_AssetManager_Project.Domain.Interfaces;
using ACTI_AssetManager_Project.Application.Interfaces;

namespace ACTI_AssetManager_Project.Tests
{
    public class CrearRecursoTests
    {
        private readonly Mock<IRecursoRepository> _repoMock;
        private readonly Mock<IResponsableRecursoService> _responsableServiceMock;
        private readonly Mock<ITipoRecursoRepository> _tipoRecursoRepoMock;
        private readonly RecursoService _service;

        public CrearRecursoTests()
        {
            _repoMock = new Mock<IRecursoRepository>();
            _responsableServiceMock = new Mock<IResponsableRecursoService>();
            _tipoRecursoRepoMock = new Mock<ITipoRecursoRepository>();

            _service = new RecursoService(
                _repoMock.Object,
                _responsableServiceMock.Object,
                _tipoRecursoRepoMock.Object
            );
        }

        // CASO 1 – Creación exitosa
        [Fact]
        public async Task CrearAsync_ConDatosValidos_GuardaRecursoCorrectamente()
        {
            var tipoRecurso = new TipoRecurso
            {
                IdTipoRecurso = 3,
                NombreTipoRecurso = "Licencia",
                IdCategoria = 2
            };

            _tipoRecursoRepoMock
                .Setup(r => r.GetByIdAsync(3))
                .ReturnsAsync(tipoRecurso);

            _repoMock
                .Setup(r => r.AddAsync(It.IsAny<Recurso>()))
                .Returns(Task.CompletedTask);

            var recurso = new Recurso
            {
                CodigoInterno = "LIC-001",
                IdTipoRecurso = 3,
                FechaAdquisicion = new DateTime(2024, 1, 1),
                Vigencia = new DateTime(2025, 12, 31),
                IdUsuarioCambio = "USR001"
            };

            await _service.CrearAsync(recurso);

            _repoMock.Verify(r => r.AddAsync(It.IsAny<Recurso>()), Times.Once);
        }

        // CASO 2 – Campos obligatorios vacíos
        [Fact]
        public void RecursoFormViewModel_SinCodigoInterno_EsInvalido()
        {
            var viewModel = new RecursoFormViewModel_Test
            {
                CodigoInterno = "",
                IdTipoRecurso = 0,
                FechaAdquisicion = DateTime.Today
            };

            var resultados = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var contexto = new System.ComponentModel.DataAnnotations.ValidationContext(viewModel);
            bool esValido = System.ComponentModel.DataAnnotations.Validator
                                .TryValidateObject(viewModel, contexto, resultados, true);

            Assert.False(esValido);
            Assert.Contains(resultados,
                r => r.MemberNames.Contains(nameof(RecursoFormViewModel_Test.CodigoInterno)));
            Assert.Contains(resultados,
                r => r.MemberNames.Contains(nameof(RecursoFormViewModel_Test.IdTipoRecurso)));
        }

        // CASO 3 – Registro duplicado
        [Fact]
        public async Task CrearAsync_ConCodigoDuplicado_LanzaExcepcion()
        {
            var tipoRecurso = new TipoRecurso
            {
                IdTipoRecurso = 1,
                NombreTipoRecurso = "Laptop",
                IdCategoria = 1
            };

            _tipoRecursoRepoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(tipoRecurso);

            _repoMock
                .Setup(r => r.AddAsync(It.IsAny<Recurso>()))
                .ThrowsAsync(new InvalidOperationException(
                    "Violation of UNIQUE KEY constraint. Duplicate entry for 'CodigoInterno'."));

            var recursoDuplicado = new Recurso
            {
                CodigoInterno = "LAP-001",
                IdTipoRecurso = 1,
                FechaAdquisicion = DateTime.Today,
                IdUsuarioCambio = "USR001"
            };

            var excepcion = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CrearAsync(recursoDuplicado));

            Assert.Contains("Duplicate entry", excepcion.Message);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Recurso>()), Times.Once);
        }

        // CASO 4a – Formato de datos: vigencia menor que adquisición
        [Fact]
        public async Task CrearAsync_VigenciaMenorQueAdquisicion_LanzaInvalidOperationException()
        {
            var tipoRecurso = new TipoRecurso
            {
                IdTipoRecurso = 3,
                NombreTipoRecurso = "Licencia",
                IdCategoria = 2
            };

            _tipoRecursoRepoMock
                .Setup(r => r.GetByIdAsync(3))
                .ReturnsAsync(tipoRecurso);

            var recursoFechaInvalida = new Recurso
            {
                CodigoInterno = "LIC-ERR-01",
                IdTipoRecurso = 3,
                FechaAdquisicion = new DateTime(2025, 6, 1),
                Vigencia = new DateTime(2024, 1, 1),   
                IdUsuarioCambio = "USR001"
            };

            var excepcion = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CrearAsync(recursoFechaInvalida));

            Assert.Contains("vigencia no puede ser menor", excepcion.Message);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Recurso>()), Times.Never);
        }

        // CASO 4b – Formato de datos: código con longitud inválida
        [Fact]
        public void RecursoFormViewModel_CodigoConMasDe50Caracteres_EsInvalido()
        {
            var viewModel = new RecursoFormViewModel_Test
            {
                CodigoInterno = new string('@', 51),
                IdTipoRecurso = 1,
                FechaAdquisicion = DateTime.Today
            };

            var resultados = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var contexto = new System.ComponentModel.DataAnnotations.ValidationContext(viewModel);
            bool esValido = System.ComponentModel.DataAnnotations.Validator
                                .TryValidateObject(viewModel, contexto, resultados, true);

            Assert.False(esValido);
            Assert.Contains(resultados,
                r => r.MemberNames.Contains(nameof(RecursoFormViewModel_Test.CodigoInterno)));
        }
    }

    internal class RecursoFormViewModel_Test
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El código interno es obligatorio.")]
        [System.ComponentModel.DataAnnotations.StringLength(50, ErrorMessage = "Máximo 50 caracteres.")]
        public string CodigoInterno { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Selecciona el tipo de recurso.")]
        [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Selecciona el tipo de recurso.")]
        public int IdTipoRecurso { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "La fecha de adquisición es obligatoria.")]
        public DateTime FechaAdquisicion { get; set; } = DateTime.Today;

        public DateTime? Vigencia { get; set; }
    }
}