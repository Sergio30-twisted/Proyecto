using ACTI_AssetManager_Project.Application.DTOs;
using ACTI_AssetManager_Project.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Application.Interfaces
{
    public interface IProyectoService
    {
        Task<IEnumerable<ProyectoDto>> ObtenerTodosLosProyectosAsync();

        Task<bool> RegistrarProyectoAsync(ProyectoDto dto, string idUsuario);
        Task<bool> EliminarProyectoAsync(int id);

        Task<Proyecto> ObtenerPorIdAsync(int id);
        Task<bool> ActualizarProyectoAsync(ProyectoDto model);
    }

}
