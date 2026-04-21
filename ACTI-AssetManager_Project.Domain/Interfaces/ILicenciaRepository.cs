using ACTI_AssetManager_Project.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Domain.Interfaces
{
    public interface ILicenciaRepository
    {
        Task<IEnumerable<Licencia>> GetAllAsync();
        Task<Licencia?> GetByIdAsync(int id);
        Task AddAsync(Licencia licencia);
        Task UpdateAsync(Licencia licencia);
        Task DeleteAsync(int id);

    }
}
