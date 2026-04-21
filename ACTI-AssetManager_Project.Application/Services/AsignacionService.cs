using ACTI_AssetManager_Project.Application.DTOs;
using ACTI_AssetManager_Project.Application.Interfaces;
using ACTI_AssetManager_Project.Domain.Entities;
using ACTI_AssetManager_Project.Domain.Interfaces;
using ACTI_AssetManager_Project.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ACTI_AssetManager_Project.Application.Services
{
    public class AsignacionService:IAsignacionService
    {
        private readonly IAsignacionRepository _repository;

        public AsignacionService(IAsignacionRepository repository) {
        _repository = repository;
        
        }

        public async Task<bool> CrearAsignacionAsync(AsignacionDto dto)
        {
            if (dto.RecursosIds == null || !dto.RecursosIds.Any())
            {
                return false;
            }

            var nuevaAsignacion = new Asignacion
            {
                IdProyecto = dto.IdProyecto,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                IdUsuarioRegistroAsignacion = dto.IdUsuarioRegistro,
                FechaRegistroAsignacion = DateTime.Now,
                // Inicializamos la lista de detalles dentro del mismo objeto
                Detalles = new List<AsignacionDetalle>()
            };

            // Mapeamos las "cartas" (listas paralelas) a la colección de detalles de la entidad
            for (int i = 0; i < dto.RecursosIds.Count; i++)
            {
                nuevaAsignacion.Detalles.Add(new AsignacionDetalle
                {
                    IdRecurso = dto.RecursosIds[i],
                    IdUsuarioResponsable = dto.UsuariosIds[i],
                    Activo = true
                });
            }

            // Enviamos el objeto "gordo" (maestro + hijos) al repositorio
            return await _repository.CrearAsignacionAsync(nuevaAsignacion);
        }

        public async Task<bool> EliminarAsignacionLogicaAsync(int idAsignacion)
        {
            // Buscamos la asignación en el repositorio
            var asignacion = await _repository.ObtenerPorIdAsync(idAsignacion);

            if (asignacion == null) return false;

            // Cambio de estado (Eliminación lógica)
            asignacion.Eliminado = true;

            // Guardamos los cambios a través del repositorio
            return await _repository.ActualizarAsync(asignacion);
        }

        public async Task<List<DetalleResponsableDto>> ObtenerDetallesPorAsignacionAsync(int idAsignacion)
        {
            var detallesEntidad = await _repository.ObtenerDetallesConReferenciasAsync(idAsignacion);

            if (detallesEntidad == null || !detallesEntidad.Any())
                return new List<DetalleResponsableDto>();

            // Agrupamos por el ID o el Nombre del usuario para no repetir
            return detallesEntidad
                .GroupBy(d => d.UsuarioResponsable?.NombreCompleto ?? "Sin Responsable")
                .Select(grupo => new DetalleResponsableDto
                {
                    NombreUsuario = grupo.Key,
                    FotoUrl = "/images/user.png",
                    // Unimos todos los recursos del mismo usuario con una coma o espacio
                    NombreRecurso = string.Join(", ", grupo.Select(g => g.Recurso?.CodigoInterno ?? "N/A"))
                })
                .ToList();
        }

        public async Task<List<AsignacionRealizadaDto>> ObtenerTodasLasAsignacionesAsync()
        {
            var asignacionesEntidad = await _repository.ObtenerTodasAsync();

            var listaDto = asignacionesEntidad
                    .Where(a => !a.Eliminado) // <--- ESTO ES LO NUEVO: Solo las que NO estén eliminadas
                    .Select(a => new AsignacionRealizadaDto
                    {
                        IdAsignacion = a.IdAsignacion,
                        NombreProyecto = a.Proyecto?.NombreProyecto,
                        Descripcion = a.Proyecto?.Descripcion ?? "Asignación de recursos sin proyecto específico",
                        FechaInicio = a.FechaInicio,
                        FechaFin = a.FechaFin,
                        // Mapeamos la propiedad para que la vista sepa el estado si lo necesita
                        Eliminado = a.Eliminado
                    }).ToList();
            return listaDto;
        }

        public async Task<object> GetAsignacionDetalleJsonAsync(int id)
        {
            var asignacion = await _repository.ObtenerAsignacionCompletaAsync(id);

            if (asignacion == null) return null;

            return new
            {
                idProyecto = asignacion.IdProyecto,
                fechaInicio = asignacion.FechaInicio.ToString("yyyy-MM-dd"),
                fechaFin = asignacion.FechaFin?.ToString("yyyy-MM-dd"),
                usuarios = asignacion.Detalles
                .GroupBy(d => d.IdUsuarioResponsable)
                .Select(grupo => new {
                idUsuario = grupo.Key,
                // Sacamos el nombre del primer registro del grupo
                nombre = grupo.First().UsuarioResponsable?.NombreCompleto ?? "Sin nombre",
                // Creamos la lista de IDs de recursos asignados a este usuario
                recursos = grupo.Select(d => d.IdRecurso).ToList()
            }).ToList()
            };
        }


    }
}
