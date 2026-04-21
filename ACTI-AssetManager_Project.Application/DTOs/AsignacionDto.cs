using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Application.DTOs
{
    public class AsignacionDto
    {
        public int? IdProyecto { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string IdUsuarioRegistro { get; set; }


        public List<string> UsuariosResponsablesIds { get; set; } = new List<string>();
        public List<int> RecursosIds { get; set; } = new List<int>();
        public List<string> UsuariosIds { get; set; } = new List<string>();
    }
}
