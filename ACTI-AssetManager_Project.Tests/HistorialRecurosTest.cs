using ACTI_AssetManager_Project.Application.DTOs;
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
    public class HistorialRecurosTest
    {
        private readonly Mock<IAsignacionRepository> _repoMock;
        private readonly AsignacionService _service;

        public HistorialRecurosTest()
        {
            // Inicializamos el Mock y el Servicio
            _repoMock = new Mock<IAsignacionRepository>();
            _service = new AsignacionService(_repoMock.Object);
        }


        [Fact]
        public async Task ObtenerTodasLasAsignaciones_NoEliminadas()
        {
            // ARRANGE
            // Creamos datos de prueba usando la Entidad de Dominio (lo que devuelve el Repo)
            var asignacionesDB = new List<Asignacion>
    {
        new Asignacion
        {
            IdAsignacion = 1,
            Eliminado = true,
            FechaInicio = DateTime.Now.AddDays(-1)
        },
        new Asignacion
        {
            IdAsignacion = 2,
            Eliminado = false,
            FechaInicio = DateTime.Now
        },new Asignacion
        {
            IdAsignacion = 3,
            Eliminado = false,
            FechaInicio = DateTime.Now
        }
    };

            // Configuramos el Mock para que devuelva la lista de Entidades
            _repoMock.Setup(r => r.ObtenerTodasAsync()).ReturnsAsync(asignacionesDB);

            var resultado = await _service.ObtenerTodasLasAsignacionesAsync();

            // ASSERT
            Assert.NotNull(resultado);

            // CAMBIO 1: Esperamos solo 1 (el activo)
            Assert.Equal(2, resultado.Count); 

            // CAMBIO 2: Verificamos que el que llegó NO esté eliminado
            Assert.All(resultado, x => Assert.False(x.Eliminado));
           
        }
    }
}
