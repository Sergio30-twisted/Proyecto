using ACTI_AssetManager_Project.Application.DTOs;
using ACTI_AssetManager_Project.Application.Interfaces;
using ACTI_AssetManager_Project.Domain.Entities;
using ACTI_AssetManager_Project.Domain.Interfaces;
using ACTI_AssetManager_Project.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Application.Services
{
    public class ProyectoService : IProyectoService
    {
        private readonly IProyectoRepository _proyectoRepo;

        public ProyectoService(IProyectoRepository proyectoRepo)
        {
            _proyectoRepo = proyectoRepo;
        }

        public async Task<IEnumerable<ProyectoDto>> ObtenerTodosLosProyectosAsync()
        {
            var entidades = await _proyectoRepo.ObtenerTodosAsync();

            return entidades.Select(p => new ProyectoDto
            {
                IdProyecto = p.IdProyecto,
                NombreProyecto = p.NombreProyecto
            }).ToList(); // Convertimos a la lista que espera el ViewModel
        }

        public async Task<Proyecto> ObtenerPorIdAsync(int id)
        {
            // Llamamos al repositorio para ir a la base de datos
            var proyecto = await _proyectoRepo.ObtenerPorIdAsync(id);

            // Opcional: Si el proyecto no existe o está marcado como ELIMINADO, 
            // podrías retornar null para que el Controller maneje el error.
            if (proyecto == null || proyecto.ELIMINADO == true)
            {
                return null;
            }

            return proyecto;
        }

        public async Task<bool> RegistrarProyectoAsync(ProyectoDto dto, string idUsuario)
        {
            /*
             EXPLICACIÓN DE LO QUE HACEMOS AQUÍ

             Imaginemos que el DTO es el paquete que llega por correo, y la entidad
             es el estante de un bodega.

             El dto es el que trae lo que el usuario escribio en la pantalla.

            La entidad es el objeto que el repositorio entiende y sabe cómo guardar SQL.
             */


            /*
             El DTO (El Paquete): Es lo que viaja por la carretera (el internet) desde el formulario del usuario hasta tu servidor. Solo lleva lo necesario.

             El Service (La Oficina de Correos): Recibe el paquete. Abre el DTO y crea una instancia de la Entidad (el formulario oficial de la empresa). Aquí es donde "copias" los datos del paquete al formulario oficial y le pones los sellos internos que el cliente no ve (como FechaRegistro o EstadoActivo).

             La Entidad (El Documento Oficial): Es el único objeto que el Repository acepta.

             El Repository (El Archivero): Toma esa Entidad y la guarda en el cajón correspondiente de la Base de Datos.
            */

            /*
             Entonces aquí creamos una instancia donde a las propiedades de la entidad
             Proyecto le pasamos los valores del dto para que la entidad se los de al repository
             y esto se guarde en la base de datos.
             */

            var nuevoProyecto = new Proyecto
            {
                // Si IdProyecto es 0, la DB generará el nuevo ID (si es Identity)
                NombreProyecto = dto.NombreProyecto,
                Descripcion = dto.Descripcion,
                ELIMINADO = false,
                FECHAHORACAMBIO = DateTime.Now,
            };

            return await _proyectoRepo.AgregarAsync(nuevoProyecto);
        }

        public async Task<bool> EliminarProyectoAsync(int id)
        {

            var proyectoExistente = await _proyectoRepo.ObtenerPorIdAsync(id);

            if (proyectoExistente == null)
            {
                return false;
            }

            proyectoExistente.ELIMINADO = true;
            proyectoExistente.FECHAHORACAMBIO = DateTime.Now;

            return await _proyectoRepo.ActualizarAsync(proyectoExistente);
        }

        public async Task<bool> ActualizarProyectoAsync(ProyectoDto model)
        {
            // 1. Buscamos el proyecto actual en la base de datos para no perder datos que no vienen en el DTO
            var proyectoExistente = await _proyectoRepo.ObtenerPorIdAsync(model.IdProyecto);

            if (proyectoExistente == null) return false;

            // 2. Actualizamos solo los campos que vienen del formulario
            proyectoExistente.NombreProyecto = model.NombreProyecto;
            proyectoExistente.Descripcion = model.Descripcion;

            // 3. Importante: Actualizamos la auditoría
            proyectoExistente.FECHAHORACAMBIO = DateTime.Now;

            // 4. Llamamos a tu método del Repository
            return await _proyectoRepo.ActualizarAsync(proyectoExistente);
        }



    }
}
