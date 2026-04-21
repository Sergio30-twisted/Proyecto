using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ACTI_AssetManager_Project.Models.Recursos
{
    public class RecursoFormViewModel
    {
        public int IdRecurso { get; set; }

        [Required(ErrorMessage = "El código interno es obligatorio.")]
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres.")]
        [Display(Name = "Código Interno")]

        public string CodigoInterno { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecciona el tipo de recurso.")]
        [Display(Name = "Tipo de Recurso")]

        public int IdTipoRecurso { get; set; }

       
        [Display(Name = "Estado")]

        public int? IdEstado { get; set; }

        [Required(ErrorMessage = "La fecha de adquisición es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Adquisición")]

        public DateTime FechaAdquisicion { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        [Display(Name = "Vigencia")]

        public DateTime? Vigencia { get; set; }

        public string? IdUsuarioResponsable { get; set; } = string.Empty;

        [Display(Name = "CapacidadUso")] // Esto es lo que sale en el <label>
        public int? CapacidadUso { get; set; }

        // Dropdowns
        public IEnumerable<SelectListItem> Tipos { get; set; } = [];
        public IEnumerable<SelectListItem> Estados { get; set; } = [];
        public IEnumerable<SelectListItem> ListaResponsables { get; set; } = [];

        public List<SelectListItem> TiposConCategoria { get; set; } = new List<SelectListItem>();
    }
}
