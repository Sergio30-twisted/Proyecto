using ACTI_AssetManager_Project.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Application.DTOs
{
    public class ReporteLicenciaDto
    {
        public Recurso Recurso { get; set; }
        public int CantidadEnUso { get; set; }

    }
}
