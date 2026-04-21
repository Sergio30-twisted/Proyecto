using ACTI_AssetManager_Project.Application.Data;
using ACTI_AssetManager_Project.Domain.Entities;
using ACTI_AssetManager_Project.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Infrastructure.Repository
{
    public class ResponsableRecursoRepository : IResponsableRecursoRepository
    {
        private readonly AM_DBContext _context;

        public ResponsableRecursoRepository(AM_DBContext context) 
        {
            _context = context;
        }

        /*
         Le dices al servidor: "Envía la petición a la base de datos y libera 
         el hilo de ejecución". Mientras la base de datos busca los datos, tu servidor puede atender otras peticiones. 
         Cuando la base de datos termina,el servidor retoma el trabajo y te entrega la lista.
         
         Cuando ponemos ToListAsync() decimos
         Aquí es donde le dices: "¡Ya! Ejecuta el plan ahora mismo". Entity Framework toma ese plan, 
         lo traduce a SQL (SELECT * FROM ...), va a la base de datos, 
         recibe los resultados y los convierte en una List<T> de C#.

    1. ¿Qué significa Task<List<Usuario>>?
       Es un contrato de tres niveles:

        Task: Promete que la operación es asíncrona (no bloquea al "mesero").

        List: Promete que te entregaré una colección (varios elementos), no un solo objeto.

        ResponsableDto: Promete que cada elemento de esa lista será el objeto de tipo Usuario */

        public async Task<List<Usuario>> ObtenerResponsableRecursosRep()
        {
            return await _context.Usuarios
            .Where(a => a.Eliminado == false)
            .OrderBy(a => a.NombreCompleto) // Opcional: para que salgan en orden alfabetico
            .ToListAsync();
        }

        
    }
}
