using ACTI_AssetManager_Project.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Application.Interfaces
{
    public interface IAuthService
    {
        string GenerarJwtToken(Usuario user);

        string ValidarYGenerarToken(string username, string password);
    }
}
