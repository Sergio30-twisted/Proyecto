using ACTI_AssetManager_Project.Application.Data;
using ACTI_AssetManager_Project.Domain.Entities;
using ACTI_AssetManager_Project.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Infrastructure.Repository
{
    public class LicenciaRepository:ILicenciaRepository
    {
        private readonly AM_DBContext _context;
        public LicenciaRepository(AM_DBContext context) => _context = context;

        public async Task<IEnumerable<Licencia>> GetAllAsync() =>
            await _context.Licencias.Include(l => l.Recurso).ThenInclude(r => r.TipoRecurso).OrderBy(l => l.FechaVencimiento).ToListAsync();

        public async Task<Licencia?> GetByIdAsync(int id) =>
            await _context.Licencias.Include(l => l.Recurso).FirstOrDefaultAsync(l => l.IdLicencia == id);

        public async Task AddAsync(Licencia licencia) { _context.Licencias.Add(licencia); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(Licencia licencia) { _context.Licencias.Update(licencia); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(int id)
        {
            var entidad = await _context.Licencias.FindAsync(id);
            if (entidad != null) { _context.Licencias.Remove(entidad); await _context.SaveChangesAsync(); }
        }

    }
}
