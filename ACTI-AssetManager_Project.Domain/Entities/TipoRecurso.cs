using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Domain.Entities
{
    public class TipoRecurso
    {
        public int IdTipoRecurso { get; set; }
        public string NombreTipoRecurso { get; set; }
        public string? Descripcion { get; set; }
        public bool Eliminado { get; set; }
        public DateTime FechaHoraCambio { get; set; }

        public int IdCategoria { get; set; }

        //Aquí estamos apuntando hacia la tabla CategoriaRecurso indicando que
        //hay un relación de esta tabla a la de Categoriarecurso
        public virtual CategoriaRecurso Categoria { get; set; }
        public ICollection<Recurso> Recursos { get; set; }
    }
}
