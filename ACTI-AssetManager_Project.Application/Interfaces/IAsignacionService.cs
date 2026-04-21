using ACTI_AssetManager_Project.Application.DTOs;
using ACTI_AssetManager_Project.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Application.Interfaces
{
    public interface IAsignacionService
    {
        //Task<IEnumerable<AsignacionDto>> ObtenerAsignacionesVigentesAsync();

        Task<bool> CrearAsignacionAsync(AsignacionDto model);

        Task<List<AsignacionRealizadaDto>> ObtenerTodasLasAsignacionesAsync();

        Task<List<DetalleResponsableDto>> ObtenerDetallesPorAsignacionAsync(int idAsignacion);

        Task<bool> EliminarAsignacionLogicaAsync(int idAsignacion);

        Task<object> GetAsignacionDetalleJsonAsync(int id);

        }
}
