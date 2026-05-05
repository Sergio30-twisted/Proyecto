using ACTI_AssetManager_Project.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<bool> AgregarAsync(Usuario usuario);
        Task<Usuario?> ObtenerPorIdAsync(string idUsuario);

    }
}
