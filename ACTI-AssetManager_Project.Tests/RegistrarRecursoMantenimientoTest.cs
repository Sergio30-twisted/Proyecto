using ACTI_AssetManager_Project.Application.Data;
using ACTI_AssetManager_Project.Domain.Entities;
using ACTI_AssetManager_Project.Domain.Enums;
using ACTI_AssetManager_Project.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ACTI_AssetManager_Project.Tests.Integration
{
    /// Pruebas de integración para el módulo: Registrar Recurso en Mantenimiento
    public class RegistrarRecursoMantenimientoTests : IDisposable
    {
        //  Contexto e infraestructura 
        private readonly AM_DBContext _context;
        private readonly RecursoRepository _repository;

        //  Datos compartidos por todos los tests 
        private readonly CategoriaRecurso _categoriaHardware;
        private readonly TipoRecurso _tipoHardware;
        private readonly EstadoRecurso _estadoDisponible;
        private readonly EstadoRecurso _estadoMantenimiento;

        // Constructor
        public RegistrarRecursoMantenimientoTests()
        {
            var options = new DbContextOptionsBuilder<AM_DBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AM_DBContext(options);
            _repository = new RecursoRepository(_context);

            _categoriaHardware = new CategoriaRecurso { IdCategoria = 1, Nombre = "Hardware" };

            _tipoHardware = new TipoRecurso
            {
                IdTipoRecurso = 1,
                NombreTipoRecurso = "Laptop",
                Descripcion = "Computadoras portátiles",
                Eliminado = false,
                FechaHoraCambio = DateTime.Now,
                IdCategoria = 1,
                Categoria = _categoriaHardware
            };

            _estadoDisponible = new EstadoRecurso
            {
                IdEstado = (int)EstadoRecursoEnum.Disponible,
                NombreEstado = "Disponible"
            };

            _estadoMantenimiento = new EstadoRecurso
            {
                IdEstado = (int)EstadoRecursoEnum.Mantenimiento,
                NombreEstado = "Mantenimiento"
            };

            _context.CategoriaRecursos.Add(_categoriaHardware);
            _context.TiposRecurso.Add(_tipoHardware);
            _context.EstadosRecurso.AddRange(_estadoDisponible, _estadoMantenimiento);
            _context.SaveChanges();
        }

        // Limpieza
        public void Dispose() => _context.Dispose();

        [Fact]
        public async Task RegistrarRecurso_ConEstadoMantenimiento_DebeGuardarseEnBD()
        {
            // Arrange
            var recurso = new Recurso
            {
                CodigoInterno = "LAP-001",
                IdTipoRecurso = _tipoHardware.IdTipoRecurso,
                IdEstado = (int)EstadoRecursoEnum.Mantenimiento,
                FechaAdquisicion = DateTime.Today,
                Eliminado = false,
                FechaHoraCambio_Creacion_Recurso = DateTime.Now,
                IdUsuarioCambio = "admin01"
            };

            // Act
            await _repository.AddAsync(recurso);

            // Assert
            var guardado = await _context.Recursos.FindAsync(recurso.IdRecurso);
            Assert.NotNull(guardado);
            Assert.Equal((int)EstadoRecursoEnum.Mantenimiento, guardado.IdEstado);
            Assert.Equal("LAP-001", guardado.CodigoInterno);
        }

        // Cambiar estado de Disponible a Mantenimiento 
        [Fact]
        public async Task ActualizarRecurso_DeDisponibleAMantenimiento_DebeReflejarNuevoEstado()
        {
            // Arrange – recurso en estado Disponible
            var recurso = new Recurso
            {
                CodigoInterno = "LAP-002",
                IdTipoRecurso = _tipoHardware.IdTipoRecurso,
                IdEstado = (int)EstadoRecursoEnum.Disponible,
                FechaAdquisicion = DateTime.Today,
                Eliminado = false,
                FechaHoraCambio_Creacion_Recurso = DateTime.Now,
                IdUsuarioCambio = "admin01"
            };
            await _repository.AddAsync(recurso);

            // Act – se actualiza el estado a Mantenimiento
            recurso.IdEstado = (int)EstadoRecursoEnum.Mantenimiento;
            recurso.FechaHoraCambio_Creacion_Recurso = DateTime.Now;
            await _repository.UpdateAsync(recurso);

            // Assert
            var actualizado = await _repository.GetByIdAsync(recurso.IdRecurso);
            Assert.NotNull(actualizado);
            Assert.Equal((int)EstadoRecursoEnum.Mantenimiento, actualizado.IdEstado);
        }

        // ObtenerEstadoMantenimientoRep solo devuelve los correctos
        // El método del repositorio filtra por IdEstado == 6 y Eliminado == false
        [Fact]
        public async Task ObtenerEstadoMantenimientoRep_DebeRetornarSoloRecursosEnMantenimiento()
        {
            // Arrange – dos en mantenimiento, uno disponible, uno eliminado en mantenimiento
            var enMantenimiento1 = new Recurso
            {
                CodigoInterno = "LAP-010",
                IdTipoRecurso = _tipoHardware.IdTipoRecurso,
                IdEstado = (int)EstadoRecursoEnum.Mantenimiento,
                FechaAdquisicion = DateTime.Today,
                Eliminado = false,
                FechaHoraCambio_Creacion_Recurso = DateTime.Now,
                IdUsuarioCambio = "admin01"
            };
            var enMantenimiento2 = new Recurso
            {
                CodigoInterno = "LAP-011",
                IdTipoRecurso = _tipoHardware.IdTipoRecurso,
                IdEstado = (int)EstadoRecursoEnum.Mantenimiento,
                FechaAdquisicion = DateTime.Today,
                Eliminado = false,
                FechaHoraCambio_Creacion_Recurso = DateTime.Now,
                IdUsuarioCambio = "admin01"
            };
            var disponible = new Recurso
            {
                CodigoInterno = "LAP-012",
                IdTipoRecurso = _tipoHardware.IdTipoRecurso,
                IdEstado = (int)EstadoRecursoEnum.Disponible,
                FechaAdquisicion = DateTime.Today,
                Eliminado = false,
                FechaHoraCambio_Creacion_Recurso = DateTime.Now,
                IdUsuarioCambio = "admin01"
            };
            var eliminadoEnMantenimiento = new Recurso
            {
                CodigoInterno = "LAP-013",
                IdTipoRecurso = _tipoHardware.IdTipoRecurso,
                IdEstado = (int)EstadoRecursoEnum.Mantenimiento,
                FechaAdquisicion = DateTime.Today,
                Eliminado = true,   
                FechaHoraCambio_Creacion_Recurso = DateTime.Now,
                IdUsuarioCambio = "admin01"
            };

            _context.Recursos.AddRange(enMantenimiento1, enMantenimiento2, disponible, eliminadoEnMantenimiento);
            await _context.SaveChangesAsync();

            // Act
            var resultado = await _repository.ObtenerEstadoMantenimientoRep();

            // Assert
            Assert.Equal(2, resultado.Count());
            Assert.All(resultado, r => Assert.Equal((int)EstadoRecursoEnum.Mantenimiento, r.IdEstado));
            Assert.All(resultado, r => Assert.False(r.Eliminado));
        }

        // Un recurso en Mantenimiento no aparece en GetAllAsync si está marcado como Eliminado
        [Fact]
        public async Task GetAllAsync_NoDebeIncluirRecursosEliminadosAunqueEstenEnMantenimiento()
        {
            // Arrange
            var recursoEliminado = new Recurso
            {
                CodigoInterno = "LAP-099",
                IdTipoRecurso = _tipoHardware.IdTipoRecurso,
                IdEstado = (int)EstadoRecursoEnum.Mantenimiento,
                FechaAdquisicion = DateTime.Today,
                Eliminado = true,
                FechaHoraCambio_Creacion_Recurso = DateTime.Now,
                IdUsuarioCambio = "admin01"
            };
            _context.Recursos.Add(recursoEliminado);
            await _context.SaveChangesAsync();

            // Act
            var todos = await _repository.GetAllAsync();

            // Assert
            Assert.DoesNotContain(todos, r => r.CodigoInterno == "LAP-099");
        }

        // SoftDelete de un recurso en Mantenimiento  ya no aparece
        [Fact]
        public async Task SoftDelete_RecursoEnMantenimiento_NoDebeAparecer()
        {
            // Arrange
            var recurso = new Recurso
            {
                CodigoInterno = "LAP-050",
                IdTipoRecurso = _tipoHardware.IdTipoRecurso,
                IdEstado = (int)EstadoRecursoEnum.Mantenimiento,
                FechaAdquisicion = DateTime.Today,
                Eliminado = false,
                FechaHoraCambio_Creacion_Recurso = DateTime.Now,
                IdUsuarioCambio = "admin01"
            };
            _context.Recursos.Add(recurso);
            await _context.SaveChangesAsync();

            // Act
            await _repository.SoftDeleteAsync(recurso.IdRecurso);

            // Assert – no aparece en la lista de mantenimiento ni en GetAll
            var mantenimiento = await _repository.ObtenerEstadoMantenimientoRep();
            Assert.DoesNotContain(mantenimiento, r => r.IdRecurso == recurso.IdRecurso);

            var todos = await _repository.GetAllAsync();
            Assert.DoesNotContain(todos, r => r.IdRecurso == recurso.IdRecurso);
        }

        // GetByIdAsync devuelve el recurso en Mantenimiento correctamente
        [Fact]
        public async Task GetByIdAsync_RecursoEnMantenimiento_DebeRetornarloConRelaciones()
        {
            // Arrange
            var recurso = new Recurso
            {
                CodigoInterno = "LAP-060",
                IdTipoRecurso = _tipoHardware.IdTipoRecurso,
                IdEstado = (int)EstadoRecursoEnum.Mantenimiento,
                FechaAdquisicion = DateTime.Today.AddMonths(-3),
                Eliminado = false,
                FechaHoraCambio_Creacion_Recurso = DateTime.Now,
                IdUsuarioCambio = "admin01"
            };
            _context.Recursos.Add(recurso);
            await _context.SaveChangesAsync();

            // Act
            var encontrado = await _repository.GetByIdAsync(recurso.IdRecurso);

            // Assert
            Assert.NotNull(encontrado);
            Assert.Equal("LAP-060", encontrado.CodigoInterno);
            Assert.Equal((int)EstadoRecursoEnum.Mantenimiento, encontrado.IdEstado);
            Assert.NotNull(encontrado.TipoRecurso);           
            Assert.Equal("Laptop", encontrado.TipoRecurso.NombreTipoRecurso);
        }

        // Un recurso en Mantenimiento aparece en el filtro correcto
        [Fact]
        public async Task GetRecursosParaPaginadoAsync_FiltrandoPorMantenimiento_DebeRetornarSolosEllos()
        {
            // Arrange
            _context.Recursos.AddRange(
                new Recurso
                {
                    CodigoInterno = "LAP-070",
                    IdTipoRecurso = _tipoHardware.IdTipoRecurso,
                    IdEstado = (int)EstadoRecursoEnum.Mantenimiento,
                    FechaAdquisicion = DateTime.Today,
                    Eliminado = false,
                    FechaHoraCambio_Creacion_Recurso = DateTime.Now,
                    IdUsuarioCambio = "admin01"
                },
                new Recurso
                {
                    CodigoInterno = "LAP-071",
                    IdTipoRecurso = _tipoHardware.IdTipoRecurso,
                    IdEstado = (int)EstadoRecursoEnum.Disponible,
                    FechaAdquisicion = DateTime.Today,
                    Eliminado = false,
                    FechaHoraCambio_Creacion_Recurso = DateTime.Now,
                    IdUsuarioCambio = "admin01"
                }
            );
            await _context.SaveChangesAsync();

            // Act – filtroEstado = 6 (Mantenimiento)
            var (recursos, total) = await _repository.GetRecursosParaPaginadoAsync(
                filtroCodigo: null,
                filtroTipo: null,
                filtroEstado: (int)EstadoRecursoEnum.Mantenimiento,
                pagina: 1,
                registrosPorPagina: 10
            );

            // Assert
            Assert.Equal(1, total);
            Assert.Single(recursos);
            Assert.Equal("LAP-070", recursos.First().CodigoInterno);
        }

        // Verifica que el sistema no impide registros con mismo código si
        [Fact]
        public async Task RegistrarDosRecursos_ConMismoCodigoYEstadoDiferente_DebenExistirIndependientemente()
        {
            // Arrange
            var recurso1 = new Recurso
            {
                CodigoInterno = "LAP-DUP",
                IdTipoRecurso = _tipoHardware.IdTipoRecurso,
                IdEstado = (int)EstadoRecursoEnum.Disponible,
                FechaAdquisicion = DateTime.Today,
                Eliminado = false,
                FechaHoraCambio_Creacion_Recurso = DateTime.Now,
                IdUsuarioCambio = "admin01"
            };
            var recurso2 = new Recurso
            {
                CodigoInterno = "LAP-DUP",
                IdTipoRecurso = _tipoHardware.IdTipoRecurso,
                IdEstado = (int)EstadoRecursoEnum.Mantenimiento,
                FechaAdquisicion = DateTime.Today,
                Eliminado = false,
                FechaHoraCambio_Creacion_Recurso = DateTime.Now,
                IdUsuarioCambio = "admin01"
            };

            // Act
            await _repository.AddAsync(recurso1);
            await _repository.AddAsync(recurso2);

            // Assert – los dos existen con IDs distintos
            Assert.NotEqual(recurso1.IdRecurso, recurso2.IdRecurso);
            var todos = await _repository.GetAllAsync();
            Assert.Equal(2, todos.Count(r => r.CodigoInterno == "LAP-DUP"));
        }
    }
}