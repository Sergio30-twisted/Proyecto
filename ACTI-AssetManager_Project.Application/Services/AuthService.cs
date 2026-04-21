using ACTI_AssetManager_Project.Application.Data;
using ACTI_AssetManager_Project.Application.Interfaces;
using ACTI_AssetManager_Project.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ACTI_AssetManager_Project.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly AM_DBContext _context;
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config, AM_DBContext context)
        {
            _config = config;
            _context = context;
        }


        public string GenerarJwtToken(Usuario user)
        {
            var jwtKey = _config["JwtSettings:Key"];

            if (string.IsNullOrEmpty(jwtKey))
                throw new Exception("La clave JWT no está configurada.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Preguntar que es un claim
            //Respuesta: Un Claim es un "pedazo" de información sobre el usuario que viaja dentro del token.


            var claims = new List<Claim>();

            // Paso 1: Obtener el ID como string (Verifica si tu propiedad es .Id o .UsuarioId)
            string idParaClaim = user.IdUsuario.ToString();

            // Paso 2: Crear el claim explícitamente
            Claim idClaim = new Claim(ClaimTypes.NameIdentifier, idParaClaim);

            // Paso 3: Agregar a la lista
            claims.Add(idClaim);
            claims.Add(new Claim(ClaimTypes.Name, user.NombreCompleto ?? "Usuario"));
            claims.Add(new Claim(ClaimTypes.Role, "User"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims,"Jwt"),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = creds,
                Issuer = _config["JwtSettings:Issuer"],
                Audience = _config["JwtSettings:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string ValidarYGenerarToken(string username, string password)
        {
            string cleanUsername = username?.Trim();

            // Buscamos al usuario en la base de datos ALMACEN_GENERAL
            var user = _context.Usuarios
                .FirstOrDefault(u => u.NombreCompleto == username.Trim() && !u.Eliminado);

            if (user == null) 
            { 
                throw new Exception("Usuario no existe");
            }

            // Validamos con BCrypt
            if (!BCrypt.Net.BCrypt.Verify(password.Trim(), user.PasswordHash.Trim())) 
            { 
                throw new Exception("Contraseña incorrecta");
            }

            // Si todo es correcto, le pasamos el objeto 'user' al generador de tokens
            return GenerarJwtToken(user);
        }
    }
}
