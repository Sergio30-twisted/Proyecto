using ACTI_AssetManager_Project.Application.Data;
using ACTI_AssetManager_Project.Application.DTOs;
using ACTI_AssetManager_Project.Application.Interfaces;
using ACTI_AssetManager_Project.Domain.Entities;
using ACTI_AssetManager_Project.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Application.Services
{
    public class ResponsableRecursoService : IResponsableRecursoService
    {
        public readonly IResponsableRecursoRepository _repository;

        public ResponsableRecursoService(IResponsableRecursoRepository repository) 
        {
            _repository = repository;
        }

        public async Task<List<ResponsableDto>> ObtenerResponsablesRecursosServ()
        {
            var usuarios = await _repository.ObtenerResponsableRecursosRep();

            //Aquí mapeamos la lista Usuario a lista de ResponasableDto

            var listaDto = usuarios.Select(u => new ResponsableDto
            {
                IdUsuario = u.IdUsuario,
                NombreCompleto = u.NombreCompleto

            }).ToList(); //<-- Este .ToList() es el correcto para convertir el resultado del Select en una List<>

            return listaDto;

        }
    }
}
