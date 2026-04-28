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
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AM_DBContext _context;

        public UsuarioRepository(AM_DBContext context)
        {
            _context = context;
        }

        public async Task<bool> AgregarAsync(Usuario usuario)
        {
            try
            {
                await _context.Usuarios.AddAsync(usuario);

                // SaveChangesAsync devuelve el número de filas afectadas
                var resultado = await _context.SaveChangesAsync();

                return resultado > 0;
            }
            catch (Exception ex)
            {
                // Aquí podrías usar un Logger para registrar el error ex.Message
                return false;
            }
        }

        public async Task<Usuario?> ObtenerPorIdAsync(string idUsuario)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario && !u.Eliminado);
        }
    }
}
