using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Application.DTOs
{
    public class RecursoDto
    {
        public int IdRecurso { get; set; }
        public string? IdAsignacion { get; set; }
        public string CodigoInterno { get; set; }
        public DateTime FechaAdquisicion { get; set; }
        public DateTime? Vigencia { get; set; }
        public string? NombreTipoRecurso { get; set; }
        public int IdTipoRecurso { get; set; }
        public int? IdEstado { get; set; }
        public string NombreEstado { get; set; }
        public string? IdUsuarioResponsable { get; set; }
        public string? NombreCompletoResponsable { get; set; }
    }
}
