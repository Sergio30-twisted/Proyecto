using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Application.DTOs
{
    public class AgregarUsuarioDto
    {
        public string IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; } // Se usará como IdUsuario (Matrícula/Correo)
        public string Password { get; set; }
        public string Rol { get; set; }

    }
}
