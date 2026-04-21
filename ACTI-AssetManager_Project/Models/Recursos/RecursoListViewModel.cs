using ACTI_AssetManager_Project.Application.DTOs;
using ACTI_AssetManager_Project.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ACTI_AssetManager_Project.Models.Recursos
{
    // ViewModel para el listado (Index)
    public class RecursoListViewModel
    {
        public IEnumerable<RecursoItemViewModel> Recursos { get; set; } = [];
        public string? FiltroCodigo { get; set; }
        public int? FiltroTipo { get; set; }
        public int? FiltroEstado { get; set; }
        public IEnumerable<SelectListItem> Tipos { get; set; } = [];
        public IEnumerable<SelectListItem> Estados { get; set; } = [];
        public IEnumerable<SelectListItem> Responsables { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> TiposConCategoria { get; set; } = new List<SelectListItem>();

        public string? IdUsuarioResponsable { get; set; } // Para saber si tiene o no
        public string? NombreCompletoResponsable { get; set; }

        public Task<List<ResponsableDto>>RecursosConAsignacion { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public bool TienePaginaAnterior => PaginaActual > 1;
        public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;

       


    }
}
