using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Domain.Entities
{
    public class Proyecto
    {
        [Key]
        public int idProyecto { get; set; }
        public string NombreProyecto { get; set; }
        public string? Descripcion { get; set; }
        public DateOnly? FechaInicio { get; set; }
        public DateOnly? FechaFin { get; set; }
        public bool Activo { get; set; }
        public bool ELIMINADO { get; set; }
        public DateTime FECHAHORACAMBIO { get; set; }

    }
}
