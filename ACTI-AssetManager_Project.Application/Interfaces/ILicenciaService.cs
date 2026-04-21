using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACTI_AssetManager_Project.Domain.Entities;

namespace ACTI_AssetManager_Project.Application.Interfaces
{
    public interface ILicenciaService
    {
        Task<IEnumerable<Licencia>> ObtenerTodasAsync();
        Task<Licencia?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Recurso>> ObtenerRecursosDisponiblesAsync();
        Task CrearAsync(Licencia licencia);
        Task ActualizarAsync(Licencia licencia);
        Task EliminarAsync(int id);

        // Nuevos métodos para el Servicio
        (string estado, string css) ObtenerEstadoVigencia(DateTime vencimiento);
        
    }
}
