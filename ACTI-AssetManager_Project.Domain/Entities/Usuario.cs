using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Domain.Entities
{
    public class Usuario
    {
        public string IdUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string PasswordHash { get; set; }
        public DateTime LimitePassword { get; set; }
        public bool SuperUsuario { get; set; }
        public byte IdTipoUsuario { get; set; }
        public bool Eliminado { get; set; }
        public DateTime FechaHoraCambio { get; set; }
        public string IdUsuarioCambio { get; set; }
        public string? CorreoElectronico { get; set; }

        [NotMapped] // Esto le dice al Entity Framework que ignore esta propiedad en la DB
        public string RutaFoto { get; set; } = "/images/user.png";


    }
}
