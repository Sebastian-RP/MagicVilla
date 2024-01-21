using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_API.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRespositorio
    {
        private readonly ApplicationDbContext _db;
        private string secretKey;

        public UsuarioRepositorio(ApplicationDbContext db, IConfiguration configuration)
        {
            _db= db;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public bool IsUsuarioUnico(string UserName)
        {
            var Usuario = _db.Usuarios.FirstOrDefault(u => u.UserName.ToLower() == UserName.ToLower());
            if (Usuario == null)
            {
                return true;
            }

            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var Usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower() &&
                                                                      u.Password == loginRequestDto.Password);

            if (Usuario == null)
            {
                return new LoginResponseDto
                {
                    Token = "",
                    Usuario = null
                };
            }

            //si el usuario existe generamos un JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, Usuario.Id.ToString()),
                    new Claim(ClaimTypes.Role, Usuario.Rol)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDto loginResponseDto = new()
            {
                Token = tokenHandler.WriteToken(token),
                Usuario = Usuario,
            };

            return loginResponseDto; 
        }

        public async Task<Usuario> Registrar(RegistroRequestDto registroRequestDto)
        {
            Usuario usuario = new()
            {
                UserName = registroRequestDto.UserName,
                Password = registroRequestDto.Password,
                Nombres = registroRequestDto.Nombres,
                Rol = registroRequestDto.Rol
            };
            await _db.Usuarios.AddAsync(usuario);
            await _db.SaveChangesAsync();
            usuario.Password = "";
            return usuario;
        }
    }
}
