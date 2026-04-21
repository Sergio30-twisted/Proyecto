using ACTI_AssetManager_Project.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Domain.Interfaces
{
    public interface IAsignacionRepository
    {
        //Task<IEnumerable<Asignacion>> ListarActivasAsync();
        Task InsertarAsync(Asignacion asignacion);
        string ObtenerUsuarioActual();

        Task<bool> CrearAsignacionAsync(Asignacion asignacion);

        Task<List<Asignacion>> ObtenerTodasAsync();

        Task<List<AsignacionDetalle>> ObtenerDetallesConReferenciasAsync(int idAsignacion);
        Task<Asignacion> ObtenerPorIdAsync(int id);
        Task<bool> ActualizarAsync(Asignacion asignacion);
        Task<Asignacion> ObtenerAsignacionCompletaAsync(int id);
    }
}
