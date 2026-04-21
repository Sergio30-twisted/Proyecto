using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Domain.Entities
{
    public class Licencia
    {
        public int IdLicencia { get; set; }
        public int IdRecurso { get; set; }
        public int CantidadTotal { get; set; }
        public int CantidadEnUso { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public Recurso Recurso { get; set; }
    }
}
