using System.ComponentModel.DataAnnotations;

namespace ACTI_AssetManager_Project.Models.Asignacion
{
    public class CrearAsignacionViewModel
    {
        [Required]
        public int IdProyecto { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        // Estas listas capturarán los inputs ocultos de cada chip/píldora
        public List<int> UsuariosSeleccionadosIds { get; set; } = new List<int>();
        public List<int> RecursosSeleccionadosIds { get; set; } = new List<int>();

    }
}
