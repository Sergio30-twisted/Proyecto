using ACTI_AssetManager_Project.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ACTI_AssetManager_Project.Models.Asignacion
{
    public class AsignacionViewModel
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public List<SelectListItem> RecursosDisponibles { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Responsables { get; set; } = new List<SelectListItem>();

        public List<string> UsuariosSeleccionadosIds { get; set; } = new List<string>();
        public List<int> RecursosSeleccionadosIds { get; set; } = new List<int>();


        public List<AsignacionRealizadaDto> AsignacionesRealizadas { get; set; } = new List<AsignacionRealizadaDto>();
    }
}
