using ACTI_AssetManager_Project.Application.DTOs;
using ACTI_AssetManager_Project.Domain.Enums;

namespace ACTI_AssetManager_Project.Models.Recursos
{
    public class RecursoItemViewModel
    {
        public int IdRecurso { get; set; }
        public string CodigoInterno { get; set; } = string.Empty;
        public string TipoRecurso { get; set; } = string.Empty;
        public int? IdEstado { get; set; }
        public string? NombreEstado { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string CssEstado { get; set; } = string.Empty;
        public string FechaAdquisicion { get; set; } = string.Empty;
        public string Vigencia { get; set; } = "Recurso sin Vigencia";
        public string? IdUsuarioResponsable { get; set; }
        public string? NombreResponsable { get; set; }

        public int IdTipoRecurso { get; set; }


        // --- CONSTRUCTOR: Aquí es donde el DTO alimenta al ViewModel ---
        // Recibe el DTO 'r' y el nombre del estado para procesarlos
        public RecursoItemViewModel(RecursoDto r, string? nombreEstado)
        {
            IdRecurso = r.IdRecurso;
            CodigoInterno = r.CodigoInterno;

            // CORRECCIÓN: Usamos directamente la propiedad del DTO que trae el texto
            TipoRecurso = r.NombreTipoRecurso ?? "—";

            IdEstado = r.IdEstado;

            NombreEstado = nombreEstado;

            // El ViewModel decide su propio color de etiqueta
            CssEstado = MapearCssEstado(r.IdEstado);

            // Formateo de fechas (El DTO las tiene como DateTime, aquí salen como string)
            FechaAdquisicion = r.FechaAdquisicion.ToString("dd/MM/yyyy");

            Vigencia = r.Vigencia.HasValue
                       ? r.Vigencia.Value.ToString("dd/MM/yyyy")
                       : "Recurso sin Vigencia";

            IdUsuarioResponsable = r.IdUsuarioResponsable;
            NombreResponsable = r.NombreCompletoResponsable;

        }

        private string MapearCssEstado(int? IdEstado)
        {
            return IdEstado.Value switch
            {
                1 => "badge-activo",     // Vigente (Verde)
                2 => "badge-PorVencer",  // Por Vencer (Naranja)
                3 => "badge-baja",       // Vencido (Rojo)
                4 => "badge-enUso",      // En Uso (Amarillo)
                5 => "badge-disponible", // Disponible (Azul) 
                6 => "badge-mantenimiento"
            };
        }

    }
}
