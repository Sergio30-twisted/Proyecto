using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACTI_AssetManager_Project.Application.Data;
using ACTI_AssetManager_Project.Application.Interfaces;
using ACTI_AssetManager_Project.Domain.Entities;
using ACTI_AssetManager_Project.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ACTI_AssetManager_Project.Application.Services
{
    public class LicenciaService : ILicenciaService
    {
        private readonly ILicenciaRepository _licenciaRepo;
        private readonly IRecursoRepository _recursoRepo;

        public LicenciaService(ILicenciaRepository licenciaRepo, IRecursoRepository recursoRepo)
        {
            _licenciaRepo = licenciaRepo;
            _recursoRepo = recursoRepo;
        }

        public async Task<IEnumerable<Licencia>> ObtenerTodasAsync()
        {
            // El include de Recursos y TipoRecurso debe estar definido dentro de GetAllAsync en el repositorio
            return await _licenciaRepo.GetAllAsync();
        }

        public async Task<Licencia?> ObtenerPorIdAsync(int id)
        {
            return await _licenciaRepo.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Recurso>> ObtenerRecursosDisponiblesAsync()
        {
            // Toda la lógica de "filtrar recursos que no tengan licencia" 
            // debe ir dentro de GetDisponiblesParaLicenciaAsync en el RecursoRepository
            return await _recursoRepo.GetDisponiblesParaLicenciaAsync();
        }

        public async Task CrearAsync(Licencia licencia)
        {
            // La validación y mapeo debería ocurrir aquí antes de llamar al repo
            await _licenciaRepo.AddAsync(licencia);
        }

        public async Task ActualizarAsync(Licencia licencia)
        {
            var existente = await _licenciaRepo.GetByIdAsync(licencia.IdLicencia)
                            ?? throw new Exception("Licencia no encontrada.");

            existente.IdRecurso = licencia.IdRecurso;
            existente.CantidadTotal = licencia.CantidadTotal;
            existente.CantidadEnUso = licencia.CantidadEnUso;
            existente.FechaVencimiento = licencia.FechaVencimiento;

            await _licenciaRepo.UpdateAsync(existente);
        }

        public async Task EliminarAsync(int id)
        {
            var existente = await _licenciaRepo.GetByIdAsync(id)
                            ?? throw new Exception("Licencia no encontrada.");

            await _licenciaRepo.DeleteAsync(existente.IdLicencia);
        }

        public (string estado, string css) ObtenerEstadoVigencia(DateTime vencimiento)
        {
            var hoy = DateTime.Today;
            var en30 = hoy.AddDays(30);

            if (vencimiento < hoy)
                return ("Vencida", "badge-vencida");

            if (vencimiento <= en30)
                return ("Por vencer", "badge-proximo");

            return ("Vigente", "badge-vigente");
        }

        
    }
}
