using ACTI_AssetManager_Project.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Domain.Interfaces
{
    public interface IProyectoRepository
    {
        Task<IEnumerable<Proyecto>> ObtenerTodosAsync();
        Task<bool> InsertarAsync(Proyecto proyecto);

        // Guarda los cambios de una entidad que ya existe
        Task<bool> ActualizarAsync(Proyecto proyecto);

        // Busca un solo proyecto por su llave primaria (ID)
        Task<Proyecto> ObtenerPorIdAsync(int id);
    }
}
