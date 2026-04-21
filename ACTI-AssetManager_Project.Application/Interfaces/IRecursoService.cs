using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACTI_AssetManager_Project.Application.DTOs;
using ACTI_AssetManager_Project.Domain.Entities;

namespace ACTI_AssetManager_Project.Application.Interfaces
{
    public interface IRecursoService
    {
        Task<IEnumerable<Recurso>> ObtenerTodosAsync();
        Task<Recurso?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<TipoRecurso>> ObtenerTiposAsync();
        Task<IEnumerable<EstadoRecurso>> ObtenerEstadosAsync();
        Task CrearAsync(Recurso recurso);
        Task ActualizarAsync(Recurso recurso);
        Task EliminarAsync(int id);
        Task<bool> EliminarTipoRecursoAsync(int id);
        Task<List<ResponsableDto>> ObtenerResponsablesParaDropdownAsync();
        Task<List<ReporteLicenciaDto>> GetReporteLicenciasAsync();
        Task<bool> AsignarResponsableAsync(int idRecurso, string idUsuario, string idUsuarioCambio);
        Task<IEnumerable<Recurso>> ObtenerRecursos_YaAsignadosAsync();
        Task<bool> LiberarRecursoServ(int idRecurso);
        Task<(IEnumerable<Recurso> Recursos, int TotalRegistros)> ObtenerRecursosPaginadosAsync(
        string? filtroCodigo, int? filtroTipo, int? filtroEstado, int pagina, int registrosPorPagina);

        int CalcularEstadoHardware(string? idResponsable);

        int DeterminarEstadoFinal(int? idEstadoOriginal, string? idResponsable, int? idTipoRecurso);

        Task<bool> PonerEnMantenimientoAsync(int idRecurso);

        Task<IEnumerable<Recurso>> ObtenerRecursosEnMantenimientoServ();

        Task<TipoRecurso> RegistrarNuevoTipoAsync(CrearTipoRecursoDto dto);

        Task<TipoRecurso> ObtenerTipoRecursoPorIdAsync(int id);

        Task<bool> ActualizarTipoRecursoAsync(TipoRecursoDto dto);

        Task<IEnumerable<Recurso>> ObtenerRecursosDisponiblesAsync();

        Task<IEnumerable<CategoriaRecurso>> ObtenerCategoriasAsync();

       

    }
}
