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
    public class TipoRecursoRepository : ITipoRecursoRepository
    {
        private readonly AM_DBContext _context;

        public TipoRecursoRepository(AM_DBContext context) 
        {
        _context = context;
        }

        public async Task<bool> EliminarTipoRecursoAsync(int id)
        {
            // 1. Buscar el registro en la base de datos
            var tipoRecurso = await _context.TiposRecurso.FindAsync(id);

            // 2. Si no existe, no hay nada que eliminar
            if (tipoRecurso == null)
            {
                return false;
            }

            // 3. VALIDACIÓN DE INTEGRIDAD (Clave Foránea)
            // No permitimos borrar si hay recursos que dependen de este tipo
            bool tieneRecursosAsociados = await _context.Recursos.AnyAsync(r => r.IdTipoRecurso == id);

            if (tieneRecursosAsociados)
            {
                // Retornamos false para que el Service sepa que la regla de negocio falló
                return false;
            }

            try
            {
                // 4. Ejecutar la eliminación
                _context.TiposRecurso.Remove(tipoRecurso);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                // Si ocurre un error inesperado de SQL, devolvemos false
                return false;
            }
        }

        public async Task<TipoRecurso?> GetByIdAsync(int id)
        {
            return await _context.TiposRecurso.FindAsync(id);
        }
    }
}
