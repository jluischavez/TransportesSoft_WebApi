using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportesSoft_WebApi.Data;
using TransportesSoft_WebApi.Models;
using TransportesSoft_WebApi.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TransportesSoft_WebApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UsuariosCatController : BaseApiController
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public UsuariosCatController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST /UsuariosCat/registro
        [HttpPost("registro")]
        public async Task<IActionResult> Registro(LoginDto dto)
        {
            var existe = await _context.UsuariosCat
                .AnyAsync(u => u.NombreUsuario == dto.NombreUsuario);

            if (existe)
                return BadRequest("El usuario ya existe.");

            var usuario = new UsuariosCat
            {
                NombreUsuario = dto.NombreUsuario,
                ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena),
                FechaRegistro = DateTime.Now,
                Activo = true
            };

            _context.UsuariosCat.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok("Usuario registrado correctamente.");
        }

        // método para generar el token
        private string GenerarToken(UsuariosCat usuario)
        {
            var claims = new[]
            {
            new Claim("Id", usuario.Id.ToString()),
            new Claim("NombreUsuario", usuario.NombreUsuario),
            new Claim("EmpresaId", usuario.EmpresaId?.ToString() ?? "")  // <- agrega esto
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("generarhash")]
        public IActionResult GenerarHash([FromBody] string contrasena)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(contrasena);
            return Ok(hash);
        }

        // POST /UsuariosCat/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var usuario = await _context.UsuariosCat
                .FirstOrDefaultAsync(u => u.NombreUsuario == dto.NombreUsuario);

            if (usuario == null || !usuario.Activo)
                return Unauthorized("Usuario o contraseña incorrectos.");

            var contrasenaValida = BCrypt.Net.BCrypt.Verify(dto.Contrasena, usuario.ContrasenaHash);

            if (!contrasenaValida)
                return Unauthorized("Usuario o contraseña incorrectos.");

            EmpresasCat? empresa = null;
            if (usuario.EmpresaId != null)
            {
                empresa = await _context.EmpresasCat.FindAsync(usuario.EmpresaId);
            }

            return Ok(new
            {
                mensaje = "Login exitoso",
                usuario.Id,
                usuario.NombreUsuario,
                usuario.EmpresaId,
                empresaNombre = empresa?.NombreComercial,
                empresaRFC = empresa?.RFC,
                empresaTelefono = empresa?.Telefono,
                token = GenerarToken(usuario)  // <- agrega esto
            });
        }
    }
}
