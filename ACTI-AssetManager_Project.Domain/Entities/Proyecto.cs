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
        public int IdProyecto { get; set; }
        public string NombreProyecto { get; set; }
        public string? Descripcion { get; set; }
        public bool ELIMINADO { get; set; }
        public DateTime FECHAHORACAMBIO { get; set; }

    }
}
