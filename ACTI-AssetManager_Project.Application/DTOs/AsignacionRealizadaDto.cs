using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Application.DTOs
{
    public class AsignacionRealizadaDto
    {
        public int IdAsignacion { get; set; }
        public string? NombreProyecto { get; set; } // Para manejar el null que mencionaste
        public string? Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        // Aquí puedes agregar un contador o nombres de responsables si gustas
        public int CantidadResponsables { get; set; }

        public bool Eliminado { get; set; }


    }
}
