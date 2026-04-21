using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Domain.Entities
{
    public class Asignacion
    {
        public int IdAsignacion { get; set; }
        public int? IdProyecto { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string IdUsuarioRegistroAsignacion { get; set; }
        public DateTime FechaRegistroAsignacion { get; set; }
        public bool Eliminado { get; set; }

        public virtual ICollection<AsignacionDetalle> Detalles { get; set; } = new List<AsignacionDetalle>();

        [ForeignKey("IdProyecto")]
        public virtual Proyecto? Proyecto { get; set; }
    }
}
