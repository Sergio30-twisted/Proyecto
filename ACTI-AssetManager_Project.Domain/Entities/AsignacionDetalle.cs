using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Domain.Entities
{
    public class AsignacionDetalle
    {
        public int IdAsignacionDetalle { get; set; }
        public int IdAsignacion { get; set; }
        public int IdRecurso { get; set; }
        public string IdUsuarioResponsable { get; set; }
        public bool Activo { get; set; }

        public virtual Asignacion Asignacion { get; set; }
        public virtual Recurso Recurso { get; set; }
       
        [ForeignKey("IdUsuarioResponsable")]
        public virtual Usuario UsuarioResponsable { get; set; }
    }
}
