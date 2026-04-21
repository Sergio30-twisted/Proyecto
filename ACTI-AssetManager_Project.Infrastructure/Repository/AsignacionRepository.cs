using ACTI_AssetManager_Project.Application.Data;
using ACTI_AssetManager_Project.Domain.Entities;
using ACTI_AssetManager_Project.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Infrastructure.Repository
{
    public class AsignacionRepository : IAsignacionRepository
    {
        private readonly AM_DBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AsignacionRepository(AM_DBContext context, IHttpContextAccessor httpContextAccessor) {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        
        }

        //public async Task<IEnumerable<Asignacion>> ListarActivasAsync()
        //{
        //    return await _context.Asignacion
        //        .Include(a => a.Recurso) // Traemos el recurso vinculado
        //        .Where(a => a.Activa)
        //        .ToListAsync();
        //}

        public async Task InsertarAsync(Asignacion asignacion)
        {
            _context.Entry(asignacion).Property("IdRecurso").IsModified = true;

            await _context.Asignacion.AddAsync(asignacion);
            await _context.SaveChangesAsync();
        }

        public string ObtenerUsuarioActual()
        {
            // Buscamos el claim del nombre en el token de la cookie/sesión
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId ?? "0";
        }

        public async Task<bool> CrearAsignacionAsync(Asignacion asignacion)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Agregamos el objeto principal (Maestro)
                // Si 'asignacion' ya tiene la lista 'Detalles' cargada, EF Core
                // detecta automáticamente las relaciones y las inserta.
                await _context.Asignacion.AddAsync(asignacion);

                // 2. Guardamos cambios (Esto inserta Maestro y luego los Detalle)
                await _context.SaveChangesAsync();

                // 3. Si llegamos aquí, confirmamos la transacción
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Si algo falla, se hace un Rollback automático al salir del 'using' o manualmente:
                await transaction.RollbackAsync();
                // Loguear ex.Message
                return false;
            }
        }

        public async Task<List<Asignacion>> ObtenerTodasAsync()
        {
            return await _context.Asignacion
          .Include(a => a.Proyecto)
          .Where(a => !a.Eliminado)
          .AsNoTracking()
          .ToListAsync();
        }

        public async Task<List<AsignacionDetalle>> ObtenerDetallesConReferenciasAsync(int idAsignacion)
        {
            return await _context.AsignacionDetalles
                // 1. IMPORTANTE: Usa la propiedad de objeto, NO el ID numérico
                .Include(d => d.UsuarioResponsable)
                .Include(d => d.Recurso)
                .Where(d => d.IdAsignacion == idAsignacion && d.Activo == true)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Asignacion> ObtenerPorIdAsync(int id)
        {
            return await _context.Asignacion.FindAsync(id);
        }

        public async Task<bool> ActualizarAsync(Asignacion asignacion)
        {
            _context.Asignacion.Update(asignacion);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Asignacion> ObtenerAsignacionCompletaAsync(int id)
        {
            return await _context.Asignacion
        .Include(a => a.Detalles) // Usamos Detalles porque así se llama en tu clase Asignacion
            .ThenInclude(d => d.UsuarioResponsable)
        .Include(a => a.Detalles)
            .ThenInclude(d => d.Recurso)
        .Include(a => a.Proyecto)
        .FirstOrDefaultAsync(a => a.IdAsignacion == id);
        }
    
    }
}
