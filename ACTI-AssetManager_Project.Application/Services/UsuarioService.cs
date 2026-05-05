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
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<bool> RegistrarUsuarioAsync(AgregarUsuarioDto dto, string idAdmin)
        {
            // Mapeo manual a la entidad de Dominio
            var nuevoUsuario = new Usuario
            {
                IdUsuario = dto.IdUsuario,
                NombreCompleto = dto.Nombre,
                CorreoElectronico = dto.Email,
                // Usamos la contraseña que viene del DTO
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                LimitePassword = DateTime.Now.AddMonths(6),
                SuperUsuario = (dto.Rol == "Admin"),
                IdTipoUsuario = (byte)(dto.Rol == "Admin" ? 1 : 2),
                Eliminado = false,

                // --- AUDITORÍA ---
                FechaHoraCambio = DateTime.Now,
                IdUsuarioCambio = idAdmin // Aquí usamos el segundo parámetro
            };

            return await _usuarioRepository.AgregarAsync(nuevoUsuario);
        }
    }
}
