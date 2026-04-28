using ACTI_AssetManager_Project.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<bool> RegistrarUsuarioAsync(AgregarUsuarioDto dto, string idAdmin);

    }
}
