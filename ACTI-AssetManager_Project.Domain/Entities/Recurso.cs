using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Domain.Entities
{
    public class Recurso
    {
        public int IdRecurso { get; set; }
        public string CodigoInterno { get; set; }
        public int IdTipoRecurso { get; set; }
        public int? IdEstado { get; set; }
        public DateTime FechaAdquisicion { get; set; }
        public DateTime? Vigencia { get; set; }
        public bool Eliminado { get; set; }
        public DateTime FechaHoraCambio_Creacion_Recurso { get; set; }
        public string IdUsuarioCambio { get; set; }
        public TipoRecurso TipoRecurso { get; set; }
        public EstadoRecurso EstadoRecurso { get; set; }
        public string? IdUsuarioResponsable { get; set; }
        public DateTime? FechaHoraCambio_Asignacion_Recurso { get; set; }

        public int? CapacidadUso { get; set; } 

        [ForeignKey("IdUsuarioResponsable")]
        public virtual Usuario? UsuarioResponsable { get; set; }
        

        public virtual ICollection<AsignacionDetalle> AsignacionDetalles { get; set; }
    }
}
