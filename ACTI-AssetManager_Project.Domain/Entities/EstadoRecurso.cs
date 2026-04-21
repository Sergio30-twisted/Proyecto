using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Domain.Entities
{
    public class EstadoRecurso
    {
        public int IdEstado { get; set; }
        public string NombreEstado { get; set; }

        public virtual ICollection<Recurso> Recursos { get; set; } = new List<Recurso>();
    }
}
