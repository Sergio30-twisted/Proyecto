using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Application.DTOs
{
    public class ProyectoDto
    {
        public int IdProyeto { get; set; }

        public string NombreProyecto { get; set; }

        public string Descripcion { get; set; }

        public DateOnly FechaInicio { get; set; }
        
        public DateOnly FechaFin { get; set; }

    }
}
