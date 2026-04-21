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
    public class RecursoRepository : IRecursoRepository
    {
        private readonly AM_DBContext _context;

        public RecursoRepository(AM_DBContext context) => _context = context;

        public async Task<IEnumerable<Recurso>> GetDisponiblesParaLicenciaAsync()
        {
            // Aquí encapsulas la lógica compleja que tenías en el servicio
            var conLicencia = await _context.Licencias.Select(l => l.IdRecurso).ToListAsync();

            return await _context.Recursos
                .Include(r => r.TipoRecurso)
                .Where(r => !r.Eliminado
                            && r.TipoRecurso.NombreTipoRecurso == "Licencias"
                            && !conLicencia.Contains(r.IdRecurso))
                .OrderBy(r => r.CodigoInterno)
                .ToListAsync();
        }

        //___
        public async Task<IEnumerable<Recurso>> GetAllAsync() =>
        await _context.Recursos
            .Include(r => r.TipoRecurso)
            .Include(r => r.EstadoRecurso)
            .Include(r => r.UsuarioResponsable)
            .Where(r => !r.Eliminado)
            .OrderByDescending(r => r.FechaHoraCambio_Creacion_Recurso)
            .ToListAsync();

        public async Task<Recurso?> GetByIdAsync(int id) =>
            await _context.Recursos
                .Include(r => r.TipoRecurso)
                .Include(r => r.EstadoRecurso)
                .Include(r => r.UsuarioResponsable)
                .FirstOrDefaultAsync(r => r.IdRecurso == id && !r.Eliminado);

            public async Task<IEnumerable<TipoRecurso>> GetTiposAsync() =>
                await _context.TiposRecurso.Where(t => !t.Eliminado).OrderBy(t => t.NombreTipoRecurso).ToListAsync();

        public async Task<IEnumerable<EstadoRecurso>> GetEstadosAsync() =>
            await _context.EstadosRecurso.OrderBy(e => e.NombreEstado).ToListAsync();

        public async Task AddAsync(Recurso recurso) { _context.Recursos.Add(recurso); await _context.SaveChangesAsync(); }

        public async Task UpdateAsync(Recurso recurso) { _context.Recursos.Update(recurso); await _context.SaveChangesAsync(); }

        public async Task SoftDeleteAsync(int id)
        {
            var item = await _context.Recursos.FindAsync(id);
            if (item != null)
            {
                item.Eliminado = true;
                item.FechaHoraCambio_Creacion_Recurso = DateTime.Now;
                _context.Entry(item).Property(x => x.Eliminado).IsModified = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> ContarAsignacionesAsync(int idRecurso)
        {
            return await _context.Recursos // Asegúrate de cambiar 'Asignaciones' por el nombre de tu tabla
                .CountAsync(a => a.IdRecurso == idRecurso
                              && a.Eliminado == false);
        }

        public Task<int> GetCantidadTotalLicenciasAsync(int idRecurso)
        {
            throw new NotImplementedException();
        }


       

        public async Task<bool> LiberarRecursoRep(int idRecurso, int nuevoEstado)
        {
            var filasAfectadas = await _context.Recursos
            .Where(r => r.IdRecurso == idRecurso)
            .ExecuteUpdateAsync(s => s
            .SetProperty(r => r.IdUsuarioResponsable, (string?)null)
            .SetProperty(r => r.IdEstado, nuevoEstado)
        );

            return filasAfectadas > 0;
        }


        public async Task<(IEnumerable<Recurso> Recursos, int TotalRegistros)> GetRecursosParaPaginadoAsync(
        string? filtroCodigo, int? filtroTipo, int? filtroEstado, int pagina, int registrosPorPagina)
        {
            var query = _context.Recursos
                .Include(r => r.TipoRecurso)
                .Include(r => r.EstadoRecurso)
                .Include(r => r.UsuarioResponsable)
                .AsQueryable();


            query = query.Where(r => r.Eliminado == false);

            // 2. Filtro por Código (Buscador)
            if (!string.IsNullOrWhiteSpace(filtroCodigo))
            {
                query = query.Where(r => r.CodigoInterno.Contains(filtroCodigo));
            }

            // 3. Filtro por Tipo (Dropdown)
            if (filtroTipo.HasValue)
            {
                query = query.Where(r => r.IdTipoRecurso == filtroTipo.Value);
            }

            // 4. Filtro por Estado (Tus botones de colores)
            if (filtroEstado.HasValue)
            {
                if (filtroEstado == 5) // El usuario dio clic en el chip "Disponible"
                {
                    query = query.Where(r =>
                        // REGLA HARDWARE: Debe ser ID 5 (Esto excluye al 6 de mantenimiento)
                        ((r.IdTipoRecurso == 1 || r.IdTipoRecurso == 2) && r.IdEstado == 5)
                        ||
                        // REGLA SOFTWARE: Sin responsable y no vencido (ID 3)
                        ((r.IdTipoRecurso != 1 && r.IdTipoRecurso != 2) &&
                         string.IsNullOrEmpty(r.IdUsuarioResponsable) && r.IdEstado != 3)
                    );
                }
                else
                {
                    // Filtro normal para otros estados (Mantenimiento, En Uso, etc.)
                    query = query.Where(r => r.IdEstado == filtroEstado.Value);
                }

            }

            int total = await query.CountAsync();

            var datos = await query
                    .OrderByDescending(r => r.IdRecurso) // Usa solo el ID para probar, sin lógicas raras todavía
                    .Skip((pagina - 1) * registrosPorPagina)
                    .Take(registrosPorPagina)
                    .ToListAsync();

            return (datos, total);
        }

        public IQueryable<Recurso> AplicarFiltroEstadoDinamico(IQueryable<Recurso> query, int? filtroEstado)
        {
            if (!filtroEstado.HasValue) return query;

            // Aquí vive la lógica de SQL pura y dura
            return filtroEstado switch
            {
                5 => query.Where(r => string.IsNullOrWhiteSpace(r.IdUsuarioResponsable)
                                     && r.IdEstado != 2 && r.IdEstado != 3),

                4 => query.Where(r => !string.IsNullOrWhiteSpace(r.IdUsuarioResponsable)
                                     && r.IdEstado != 2 && r.IdEstado != 3),

                _ => query.Where(r => r.IdEstado == filtroEstado)
            };
        }

        public IQueryable<Recurso> ObtenerTodosQueryable()
        {
            return _context.Recursos
            .AsNoTracking()
            .Include(r => r.TipoRecurso)
            .Include(r => r.UsuarioResponsable)
            .Where(r => r.Eliminado == false) // <--- AGREGAR ESTO
            .AsQueryable();
        }

        public async Task<IEnumerable<Recurso>> ObtenerEstadoMantenimientoRep()
        {
            // Buscamos específicamente el ID 6 en la tabla maestra de estados
            return await _context.Recursos
             .Include(r => r.TipoRecurso)
             .Include(r => r.UsuarioResponsable)
             .Where(r => r.IdEstado == 6 && r.Eliminado == false)
             .ToListAsync();


        }


        /*METODOS REPOSITORY PARA EL CASO DE USO CREAR TIPO RECURSO*/

        public async Task GuardarTipoRecursoAsync(TipoRecurso entidad) 
        {
            _context.TiposRecurso.Add(entidad);
            await _context.SaveChangesAsync();
        
        }

        public async Task<bool> ExisteTipoNombreAsync(string nombre)
        {
            // Buscamos en la base de datos si ya existe ese nombre
            return await _context.TiposRecurso.AnyAsync(t => t.NombreTipoRecurso.ToLower() == nombre.ToLower());
        
        }

        /*METODO REPOSITORY PARA EL CASO DE USO EDITAR TIPO RECURSO*/
        //Aquí primero nos traemos la información actual del tipo recurso para despues editarla

        public async Task<TipoRecurso> ObtenerTipoRecursoPorIdAsync(int id)
        {
            // Buscamos el tipo de recurso por su ID 
            // y nos aseguramos de que no esté marcado como "Eliminado" (si manejas borrado lógico)
            return await _context.TiposRecurso
                .FirstOrDefaultAsync(t => t.IdTipoRecurso == id);
        }

        /*METODO REPOSITORY PARA EL CASO DE USO EDITAR TIPO RECURSO*/
        //Aquí como tal ya guardamos en la base de datos los valores que ingresamos
        //en la vista


        public async Task<bool> ActualizarTipoRecursoAsync(TipoRecurso entidad)
        {
            _context.TiposRecurso.Update(entidad);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<CategoriaRecurso>> ObtenerCategoriasAsync()
        {
            return await _context.CategoriaRecursos
             .OrderBy(c => c.IdCategoria)
             .ToListAsync();
        }

       
    }
}
