using ACTI_AssetManager_Project.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Domain.Interfaces
{
    public interface IResponsableRecursoRepository
    {
        Task<List<Usuario>> ObtenerResponsableRecursosRep();

    }
}
