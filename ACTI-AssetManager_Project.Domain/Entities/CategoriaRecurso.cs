using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Domain.Entities
{
    public class CategoriaRecurso
    {
        public int IdCategoria { get; set; }
        public string Nombre { get; set; }

        //Esta colección existe porque representa
        //la relación de uno a muchos desde la perspectiva de la categoría.
        // indica que una CategoriaRecurso puede tener una colección de tipo recursos
        public virtual ICollection<TipoRecurso> TiposRecursos { get; set; }

    }
}
