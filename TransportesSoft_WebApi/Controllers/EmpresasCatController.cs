using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportesSoft_WebApi.Data;
using TransportesSoft_WebApi.DTOs;
using TransportesSoft_WebApi.Models;

namespace TransportesSoft_WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmpresasCatController : BaseApiController
    {
        private readonly AppDbContext _context;

        public EmpresasCatController(AppDbContext context)
        {
            _context = context;
        }

        // GET /EmpresasCat - trae solo Id y NombreComercial para el select
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var empresas = await _context.EmpresasCat
                .Where(e => e.Activo)
                .Select(e => new { e.Id, e.NombreComercial })
                .ToListAsync();
            return Ok(empresas);
        }

        [Authorize]
        [HttpPost("crear")]
        public async Task<IActionResult> CrearEmpresa(CrearEmpresaDto dto)
        {
            var usuarioId = ObtenerUsuarioId();

            if (usuarioId == null)
                return SinUsuarioValido();

            if (string.IsNullOrWhiteSpace(dto.NombreComercial))
                return BadRequest(new { mensaje = "El nombre comercial es obligatorio." });

            if (string.IsNullOrWhiteSpace(dto.RazonSocial))
                return BadRequest(new { mensaje = "La razón social es obligatoria." });

            if (string.IsNullOrWhiteSpace(dto.RFC))
                return BadRequest(new { mensaje = "El RFC es obligatorio." });

            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { mensaje = "El email es obligatorio." });

            if (string.IsNullOrWhiteSpace(dto.ClaveAcceso))
                return BadRequest(new { mensaje = "La clave de acceso es obligatoria." });

            var nombreComercial = dto.NombreComercial.Trim();
            var razonSocial = dto.RazonSocial.Trim();
            var rfc = dto.RFC.Trim().ToUpper();
            var email = dto.Email.Trim().ToLower();
            var telefono = dto.Telefono?.Trim();

            var usuario = await _context.UsuariosCat
                .FirstOrDefaultAsync(x => x.Id == usuarioId.Value);

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            if (usuario.EmpresaId != null)
                return BadRequest(new { mensaje = "Este usuario ya tiene una empresa asignada." });

            var rolActual = await _context.UsuarioRoles
                .FirstOrDefaultAsync(x => x.UsuarioId == usuario.Id);

            if (rolActual != null)
                return BadRequest(new { mensaje = "Este usuario ya tiene un rol asignado." });

            var existeRFC = await _context.EmpresasCat
                .AnyAsync(x => x.RFC == rfc);

            if (existeRFC)
                return BadRequest(new { mensaje = "Ya existe una empresa registrada con ese RFC." });

            var existeRazonSocial = await _context.EmpresasCat
                .AnyAsync(x => x.RazonSocial == razonSocial);

            if (existeRazonSocial)
                return BadRequest(new { mensaje = "Ya existe una empresa registrada con esa Razón Social." });

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var empresa = new EmpresasCat
                {
                    NombreComercial = nombreComercial,
                    RazonSocial = razonSocial,
                    RFC = rfc,
                    Email = email,
                    Telefono = telefono,
                    ClaveAcceso = BCrypt.Net.BCrypt.HashPassword(dto.ClaveAcceso),
                    FechaRegistro = DateTime.UtcNow,
                    Activo = true
                };

                _context.EmpresasCat.Add(empresa);
                await _context.SaveChangesAsync();

                usuario.EmpresaId = empresa.Id;

                _context.UsuarioRoles.Add(new UsuarioRoles
                {
                    UsuarioId = usuario.Id,
                    RolId = 2
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    mensaje = "Empresa creada correctamente.",
                    empresaId = empresa.Id,
                    nombreComercial = empresa.NombreComercial
                });
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Error al crear la empresa." });
            }
        }

        // POST /EmpresasCat/asignar - valida clave y asigna empresa al usuario
        [Authorize]
        [HttpPost("asignar")]
        public async Task<IActionResult> AsignarEmpresa(AsignarEmpresaDto dto)
        {
            var usuarioId = ObtenerUsuarioId();

            if (usuarioId == null)
                return SinUsuarioValido();

            if (dto.EmpresaId <= 0)
                return BadRequest(new { mensaje = "Empresa inválida." });

            if (string.IsNullOrWhiteSpace(dto.ClaveAcceso))
                return BadRequest(new { mensaje = "La clave de acceso es obligatoria." });

            var usuario = await _context.UsuariosCat
                .FirstOrDefaultAsync(x => x.Id == usuarioId.Value);

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            if (usuario.EmpresaId != null)
                return BadRequest(new { mensaje = "Este usuario ya tiene una empresa asignada." });

            var rolActual = await _context.UsuarioRoles
                .FirstOrDefaultAsync(x => x.UsuarioId == usuario.Id);

            if (rolActual != null)
                return BadRequest(new { mensaje = "Este usuario ya tiene un rol asignado." });

            var empresa = await _context.EmpresasCat
                .FirstOrDefaultAsync(x => x.Id == dto.EmpresaId && x.Activo == true);

            if (empresa == null)
                return NotFound(new { mensaje = "Empresa no encontrada o inactiva." });

            var claveCorrecta = BCrypt.Net.BCrypt.Verify(dto.ClaveAcceso, empresa.ClaveAcceso);

            if (!claveCorrecta)
                return Unauthorized(new { mensaje = "Clave de acceso incorrecta." });

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                usuario.EmpresaId = empresa.Id;

                _context.UsuarioRoles.Add(new UsuarioRoles
                {
                    UsuarioId = usuario.Id,
                    RolId = 3
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    mensaje = "Empresa asignada correctamente.",
                    empresaId = empresa.Id,
                    nombreComercial = empresa.NombreComercial
                });
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Error al asignar empresa." });
            }
        }
    }
}