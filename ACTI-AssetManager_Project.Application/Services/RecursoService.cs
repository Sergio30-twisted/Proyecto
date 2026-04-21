using ACTI_AssetManager_Project.Application.Data;
using ACTI_AssetManager_Project.Application.DTOs;
using ACTI_AssetManager_Project.Application.Interfaces;
using ACTI_AssetManager_Project.Domain.Entities;
using ACTI_AssetManager_Project.Domain.Enums;
using ACTI_AssetManager_Project.Domain.Interfaces;
using ACTI_AssetManager_Project.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ACTI_AssetManager_Project.Application.Services
{
    public class RecursoService : IRecursoService
    {
        private readonly IRecursoRepository _repo;
        private readonly IResponsableRecursoService _responsableService;
        private readonly ITipoRecursoRepository _tiporecursoRepository;


        public RecursoService(IRecursoRepository repo, IResponsableRecursoService responsableRecursoService, ITipoRecursoRepository tiporecursoRepository)
        {
            _repo = repo;
            _responsableService = responsableRecursoService;
            _tiporecursoRepository = tiporecursoRepository;
        }

        public async Task<List<ResponsableDto>> ObtenerResponsablesParaDropdownAsync()
        {
            var usuarios = await _responsableService.ObtenerResponsablesRecursosServ();

            // DEBES mapear manualmente para que la lista no llegue vacía al Controller
            return usuarios.Select(u => new ResponsableDto
            {
                IdUsuario = u.IdUsuario.ToString(),
                NombreCompleto = u.NombreCompleto

            }).ToList();
        }

        public async Task<IEnumerable<Recurso>> ObtenerTodosAsync()
        {

            var lista = await _repo.GetAllAsync();
            return lista;

        }

        /*METODOS SERVICES PARA Trear un recurso por su Id*/
        public async Task<Recurso?> ObtenerPorIdAsync(int id) => await _repo.GetByIdAsync(id);

        /*METODOS SERVICES PARA Obtener los tipos de reucursos que existen en la base de datos*/
        public async Task<IEnumerable<TipoRecurso>> ObtenerTiposAsync() => await _repo.GetTiposAsync();

        /*METODOS SERVICES PARA Traer los estados que existen en la base de datos*/
        public async Task<IEnumerable<EstadoRecurso>> ObtenerEstadosAsync() => await _repo.GetEstadosAsync();


        /*METODOS SERVICES PARA Calcular el estado que va teniendo un recurso de tipo software*/
        private int CalcularEstadoLogico(DateTime? vigencia)
        {
            if (!vigencia.HasValue) return 1; // 1. Sin fecha = Vigente

            var hoy = DateTime.Today;
            var fechaVigencia = vigencia.Value.Date;

            // EL ORDEN IMPORTANTE:
            // Primero preguntamos si YA caducó (incluyendo HOY)
            if (fechaVigencia <= hoy)
                return 3; // Vencido

            // DESPUÉS preguntamos si le falta poco
            if (fechaVigencia <= hoy.AddDays(4))
                return 2; // Por Vencer

            // Si no es ninguna, está sano
            return 1; // Vigente
        }

        /*METODOS SERVICES PARA Calcular el estado que puede tener un recurso de tipo hardware*/
        public int CalcularEstadoHardware(string? idResponsable)
        {
            if (string.IsNullOrEmpty(idResponsable))
            {
                return 5; // Disponible (Azul)
            }

            return 4; // En Uso (Amarillo)

        }

        /*METODOS SERVICES PARA Determinar el estado final de un recurso*/
        public int DeterminarEstadoFinal(int? idEstadoOriginal, string? idResponsable, int? idTipoRecurso)
        {

            int estadoActual = idEstadoOriginal.GetValueOrDefault();

            if (estadoActual == 6) 
            {
                return 6;
            }

            // IDs: 2 = Por Vencer, 3 = Vencido
            // Si la base de datos dice que es un tema de vigencia, eso manda.
            if (estadoActual == (int)EstadoRecursoEnum.Mantenimiento ||
                estadoActual == (int)EstadoRecursoEnum.PorVencer ||
                estadoActual == (int)EstadoRecursoEnum.Vencido)
            {
                // Si es uno de estos, mándalo tal cual (Mantenimiento, Vencido, etc.)
                return estadoActual;
            }

            if (idTipoRecurso == 1 || idTipoRecurso == 2)
            {
                return CalcularEstadoHardware(idResponsable);
            }

            return estadoActual > 0 ? estadoActual : 5;

        }


        /*METODOS SERVICES PARA Obtener todos los recursos ya asignados*/
        public async Task<IEnumerable<Recurso>> ObtenerRecursos_YaAsignadosAsync()
        {
            var recursos = await _repo.GetAllAsync();



            // 2. Recalculamos el estado "al vuelo" para cada uno
            foreach (var r in recursos)
            {
                // 1. CAPTURAMOS EL VALOR ORIGINAL DE LA BASE DE DATOS
                int estadoOriginal = r.IdEstado ?? 0;

                // 2. ESCUDO: Si el estado es Mantenimiento (6), Vencido (3) o Por Vencer (2), 
                // NO permitas que entre a la lógica de Hardware/Software.
                if (estadoOriginal == 6 || estadoOriginal == 3 || estadoOriginal == 2)
                {
                    continue; // Salta al siguiente recurso de la lista
                }

                // 3. Si no es uno de esos, procedemos con el cálculo normal
                if (r.IdTipoRecurso == 1 || r.IdTipoRecurso == 2)
                {
                    r.IdEstado = CalcularEstadoHardware(r.IdUsuarioResponsable);
                }
                else
                {
                    r.IdEstado = CalcularEstadoLogico(r.Vigencia);
                }
            }

            return recursos;
        }


        public async Task<IEnumerable<Recurso>> ObtenerRecursosDisponiblesAsync()
        {
            // 1. Traemos todos los recursos de la base de datos
            var todosLosRecursos = await _repo.GetAllAsync();

            // 2. Aplicamos el filtro de disponibilidad según el tipo
            var disponibles = todosLosRecursos.Where(r =>
            {
                // REGLA PARA HARDWARE (Servidores = 1, Laptops = 2)
                if (r.IdTipoRecurso == 1 || r.IdTipoRecurso == 2)
                {
                    // Solo si el estado es exactamente 5 (Disponible)
                    return r.IdEstado == 5;
                }

                // REGLA PARA SOFTWARE (Licencias, etc.)
                else
                {
                    // No tiene responsable Y no está vencido (ID 3 es Vencido)
                    return string.IsNullOrEmpty(r.IdUsuarioResponsable) && r.IdEstado != 3;
                }
            });

            return disponibles;
        }



        /*METODOS SERVICES PARA EL CASO DE USO Crear Recurso*/
        public async Task CrearAsync(Recurso recurso)
        {

            if (recurso.Vigencia.HasValue && recurso.FechaAdquisicion.Date > recurso.Vigencia.Value.Date)
            {
                throw new InvalidOperationException("La fecha de vigencia no puede ser menor a la fecha de adquisición");

            }

            var tipo = await _tiporecursoRepository.GetByIdAsync(recurso.IdTipoRecurso);

            if (tipo.IdCategoria == 1 || tipo.IdCategoria == 3)
            {
                recurso.IdEstado = 5; 
                recurso.Vigencia = null;
            }

            else if (tipo.IdCategoria == 2)
            {
                // REGLA SOFTWARE CON VIGENCIA:
                // Aquí sí usamos la lógica de fechas (Vigente, Por Vencer, Vencido)
                recurso.IdEstado = CalcularEstadoLogico(recurso.Vigencia);
            }

            recurso.Eliminado = false;
            recurso.FechaHoraCambio_Creacion_Recurso = DateTime.Now;
            await _repo.AddAsync(recurso);
        }

        /*METODOS SERVICES PARA EL Mostrar el reporte de licencias*/
        public async Task<List<ReporteLicenciaDto>> GetReporteLicenciasAsync()
        {
            var recursos = await _repo.GetAllAsync();
            var licencias = recursos.Where(r => r.IdTipoRecurso == 3).ToList();
            var resultado = new List<ReporteLicenciaDto>();

            foreach (var l in licencias)
            {
                // Supongamos que aquí cuentas cuántos están en uso en tu tabla de asignaciones
                int enUso = await _repo.ContarAsignacionesAsync(l.IdRecurso);

                resultado.Add(new ReporteLicenciaDto
                {
                    Recurso = l,
                    CantidadEnUso = enUso
                });
            }
            return resultado;
        }


        /*METODOS SERVICES PARA EL CASO DE USO Actualizar un Recurso*/
        public async Task ActualizarAsync(Recurso recurso)
        {

            var existente = await _repo.GetByIdAsync(recurso.IdRecurso) ?? throw new Exception("Recurso no encontrado.");

            int estadoFinal;

            if (recurso.IdTipoRecurso == 1 || recurso.IdTipoRecurso == 2)
            {
                // Es Hardware: Usamos la nueva lógica
                estadoFinal = CalcularEstadoHardware(recurso.IdUsuarioResponsable);
            }
            else
            {
                // Es Software/Licencia: Usamos la lógica de fechas que ya tenías
                estadoFinal = CalcularEstadoLogico(recurso.Vigencia);
            }

            

            // Mapeo manual de propiedades
            existente.CodigoInterno = recurso.CodigoInterno.Trim().ToUpper();
            existente.IdTipoRecurso = recurso.IdTipoRecurso;
            existente.IdEstado = estadoFinal;
            existente.FechaAdquisicion = recurso.FechaAdquisicion;
            existente.Vigencia = recurso.Vigencia;
            existente.IdUsuarioCambio = recurso.IdUsuarioCambio;
            existente.IdUsuarioResponsable = recurso.IdUsuarioResponsable;
            existente.FechaHoraCambio_Creacion_Recurso = DateTime.Now;

            await _repo.UpdateAsync(existente);


        }



        /*METODOS SERVICES PARA EL CASO DE USO Eliminar un Recurso*/
        public async Task EliminarAsync(int id) => await _repo.SoftDeleteAsync(id);


        /*METODOS SERVICES PARA EL CASO DE USO Eliminar un TipoRecurso*/

        public async Task<bool> EliminarTipoRecursoAsync(int id)
        {
            // El Service simplemente orquesta la llamada. 
            // Si en el futuro quisieras agregar logs o notificaciones, 
            // este es el lugar ideal.

            try
            {
                // Ejecutamos la lógica de eliminación que definimos
                return await _tiporecursoRepository.EliminarTipoRecursoAsync(id);
            }
            catch (Exception ex)
            {
                // Aquí podrías loguear el error específico antes de devolver false
                System.Diagnostics.Debug.WriteLine($"Error en Service al eliminar: {ex.Message}");
                return false;
            }
        }

        /*METODOS SERVICES PARA Asignar un responsable a un recurso*/
        public async Task<bool> AsignarResponsableAsync(int idRecurso, string idUsuario, string idUsuarioCambio)
        {
            // 1. Buscamos el recurso en la base de datos
            var recurso = await _repo.GetByIdAsync(idRecurso);

            // 2. Si no existe, avisamos al Controller
            if (recurso == null) return false;

            // 3. Actualizamos los campos necesarios
            recurso.IdUsuarioResponsable = idUsuario; // El VARCHAR(15) que definimos
            recurso.FechaHoraCambio_Asignacion_Recurso = DateTime.Now;   // Auditoría
            recurso.IdEstado = 4;

           


            // 4. Mandamos la orden de actualización al Repositorio
            // Esto ejecutará el SQL: UPDATE AMRecurso SET idUsuarioResponsable = ... WHERE IdRecurso = ...
            await _repo.UpdateAsync(recurso);
          
            return true;
        }

        /*METODOS SERVICES PARA liberar un recurso asignado*/
        public async Task<bool> LiberarRecursoServ(int idRecurso)
        {
            var recurso = await _repo.GetByIdAsync(idRecurso);
            if (recurso == null) return false;

            int estadoALiberar;

            // Si es Hardware (1 o 2), al liberarse vuelve a estar Disponible (5)
            if (recurso.IdTipoRecurso == 1 || recurso.IdTipoRecurso == 2)
            {
                estadoALiberar = (int)EstadoRecursoEnum.Disponible; 
            }
            else
            {
                // Si es Software, recalculamos su vigencia (podría volver a 1, 2 o 3)
                estadoALiberar = CalcularEstadoLogico(recurso.Vigencia);
            }

            // Aquí podrías agregar validaciones extra si fuera necesario
            return await _repo.LiberarRecursoRep(idRecurso, estadoALiberar);
        }

        /*METODOS SERVICES PARA poner en mantenimiento un recurso*/
        public async Task<bool> PonerEnMantenimientoAsync(int idRecurso)
        {
            var recurso = await _repo.GetByIdAsync(idRecurso);

            if (recurso == null)
            {
                  return false;
              
            }

           
            // Actualizamos al ID 6 (Mantenimiento)
            recurso.IdEstado = (int)EstadoRecursoEnum.Mantenimiento;

            System.Diagnostics.Debug.WriteLine($"[DEBUG] Recurso ID: {recurso.IdRecurso} - Nuevo IdEstado: {recurso.IdEstado}");


            // Opcional: Si quieres que al entrar a taller ya no tenga dueño

            await _repo.UpdateAsync(recurso);
            return true;
        }

        /*METODOS SERVICES PARA Obtener recursos por paginado*/

        public async Task<(IEnumerable<Recurso> Recursos, int TotalRegistros)> ObtenerRecursosPaginadosAsync(
        string? filtroCodigo, int? filtroTipo, int? filtroEstado, int pagina, int registrosPorPagina)
        {
            return await _repo.GetRecursosParaPaginadoAsync(
                filtroCodigo, filtroTipo, filtroEstado, pagina, registrosPorPagina);
        }

        /*Obtener Estado Mantenimiento*/

        public async Task<IEnumerable<Recurso>> ObtenerRecursosEnMantenimientoServ()
        {
            return await _repo.ObtenerEstadoMantenimientoRep();
        }


        /*METODOS SERVICES PARA EL CASO DE USO CREAR TIPO RECURSO Registrar Nuevo Tipo Recurso*/

        public async Task<TipoRecurso> RegistrarNuevoTipoAsync(CrearTipoRecursoDto dto)
        {
            // CASO: Campo Obligatorio y Espacios en Blanco
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return null;

            // CASO: Límite de Caracteres
            if (dto.Nombre.Trim().Length > 50)
                return null;

            // CASO: Evitar Duplicados
            bool existe = await _repo.ExisteTipoNombreAsync(dto.Nombre.Trim());
            if (existe) return null;

            var entidad = new TipoRecurso
            {
                NombreTipoRecurso = dto.Nombre.Trim(),
                IdCategoria = dto.IdCategoria,
                FechaHoraCambio = DateTime.Now
            };

            await _repo.GuardarTipoRecursoAsync(entidad);
            return entidad;
        }

        /*METODOS SERVICES PARA EL CASO DE USO CREAR TIPO RECURSO Obtener los datos de un Tipo Recurso por Id*/
        public async Task<TipoRecurso> ObtenerTipoRecursoPorIdAsync(int id)
        {
            // 1. Validamos que el ID sea válido antes de ir a la base de datos
            if (id <= 0) return null;

            // 2. Le pedimos al repositorio que busque la entidad
            var tipoRecurso = await _repo.ObtenerTipoRecursoPorIdAsync(id);

            // 3. Aquí podrías agregar lógica adicional, como verificar permisos
            // o mapear a un DTO si no quieres exponer la entidad completa.

            return tipoRecurso;
        }

        /*METODOS SERVICES PARA EL CASO DE USO ACTUALIZAR TIPO RECURSO*/
        public async Task<bool> ActualizarTipoRecursoAsync(TipoRecursoDto dto)
        {
            // Buscamos el registro original
            var entidad = await _repo.ObtenerTipoRecursoPorIdAsync(dto.Id);
            if (entidad == null) return false;

            // Actualizamos solo los campos necesarios
            entidad.NombreTipoRecurso = dto.Nombre;
            entidad.IdCategoria = dto.IdCategoria;

            return await _repo.ActualizarTipoRecursoAsync(entidad);
        }

        public async Task<IEnumerable<CategoriaRecurso>> ObtenerCategoriasAsync()
        {
            // Llamamos al repositorio
            return await _repo.ObtenerCategoriasAsync();
        }

      
    }
}
