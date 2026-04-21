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
    public class ProyectoRepository : IProyectoRepository
    {
        private readonly AM_DBContext _context;

        public ProyectoRepository(AM_DBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Proyecto>> ObtenerTodosAsync()
        {
            var lista = await _context.Proyectos
           .Where(p => !p.ELIMINADO)
           .ToListAsync();

            return lista ?? new List<Proyecto>();
        }

        public async Task<bool> InsertarAsync(Proyecto proyecto)
        {
            await _context.Proyectos.AddAsync(proyecto);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Proyecto> ObtenerPorIdAsync(int id)
        {
            // FindAsync es el método más eficiente para buscar por ID
            return await _context.Proyectos.FindAsync(id);
        }


        public async Task<bool> ActualizarAsync(Proyecto proyecto)
        {
            try
            {
                _context.Proyectos.Update(proyecto);

                var filasAfectadas = await _context.SaveChangesAsync();

                // Si se guardó al menos una fila, devolvemos true

                return filasAfectadas > 0;
            }
            catch (Exception ex)
            {

                return false;
            }

        }
    }
}
