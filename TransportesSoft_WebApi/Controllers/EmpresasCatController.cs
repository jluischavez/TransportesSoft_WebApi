using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportesSoft_WebApi.Data;
using TransportesSoft_WebApi.Models;
using BCrypt.Net;

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

        // POST /EmpresasCat - crear empresa (solo superadmin usará esto)
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(EmpresasCat empresa)
        {
            empresa.ClaveAcceso = BCrypt.Net.BCrypt.HashPassword(empresa.ClaveAcceso);
            empresa.FechaRegistro = DateTime.Now;
            _context.EmpresasCat.Add(empresa);
            await _context.SaveChangesAsync();
            return Ok(empresa);
        }

        // POST /EmpresasCat/asignar - valida clave y asigna empresa al usuario
        [Authorize]
        [HttpPost("asignar")]
        public async Task<IActionResult> Asignar([FromBody] AsignarEmpresaDto dto)
        {
            var empresa = await _context.EmpresasCat.FindAsync(dto.EmpresaId);
            if (empresa == null || !empresa.Activo)
                return NotFound(new { mensaje = "Empresa no encontrada." });

            var claveValida = BCrypt.Net.BCrypt.Verify(dto.ClaveAcceso, empresa.ClaveAcceso);
            if (!claveValida)
                return Unauthorized(new { mensaje = "Clave de acceso incorrecta." });

            var usuario = await _context.UsuariosCat.FindAsync(dto.UsuarioId);
            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            usuario.EmpresaId = dto.EmpresaId;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Empresa asignada correctamente.", empresa.NombreComercial });
        }
    }
}