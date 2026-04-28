namespace ACTI_AssetManager_Project.Models.Proyecto
{
    public class ProyectoViewModel
    {
        public IEnumerable<ProyectoItemViewModel> Proyectos { get; set; } = new List<ProyectoItemViewModel>();

        // Propiedades para el resumen (opcional, como lo tienes en Asignaciones)
        public int TotalProyectos { get; set; }
        public int ProyectosActivos { get; set; }

    }
}
