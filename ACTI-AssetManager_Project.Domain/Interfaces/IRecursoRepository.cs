using ACTI_AssetManager_Project.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Domain.Interfaces
{
    public interface IRecursoRepository
    {
        Task<IEnumerable<Recurso>> GetDisponiblesParaLicenciaAsync();
        Task<IEnumerable<Recurso>> GetAllAsync();
        Task<Recurso?> GetByIdAsync(int id);
        Task<IEnumerable<TipoRecurso>> GetTiposAsync();
        Task<IEnumerable<EstadoRecurso>> GetEstadosAsync();
        Task AddAsync(Recurso recurso);
        Task UpdateAsync(Recurso recurso);
        Task SoftDeleteAsync(int id);
        Task<int> ContarAsignacionesAsync(int idRecurso);

        Task<int> GetCantidadTotalLicenciasAsync(int idRecurso);

     
        Task<bool> LiberarRecursoRep(int idRecurso, int nuevoEstado);

        Task<(IEnumerable<Recurso> Recursos, int TotalRegistros)> GetRecursosParaPaginadoAsync(
        string? filtroCodigo, int? filtroTipo, int? filtroEstado, int pagina, int registrosPorPagina);

        IQueryable<Recurso> AplicarFiltroEstadoDinamico(IQueryable<Recurso> query, int? filtroEstado);

        IQueryable<Recurso> ObtenerTodosQueryable();

        Task<IEnumerable<Recurso>> ObtenerEstadoMantenimientoRep();

        Task GuardarTipoRecursoAsync(TipoRecurso entidad);
        Task<bool> ExisteTipoNombreAsync(string nombre);

        Task<TipoRecurso> ObtenerTipoRecursoPorIdAsync(int id);

        Task<bool> ActualizarTipoRecursoAsync(TipoRecurso entidad); 
        Task<IEnumerable<CategoriaRecurso>> ObtenerCategoriasAsync();
       
    }
}
