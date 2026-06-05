using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportesSoft_WebApi.Data;
using TransportesSoft_WebApi.Models;

namespace TransportesSoft_WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ContPreciosDieselController : BaseApiController
    {
        private readonly AppDbContext _context;
        public ContPreciosDieselController(AppDbContext context) => _context = context;

        private int GetEmpresaId() => int.Parse(User.FindFirst("EmpresaId")!.Value);

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var empresaId = GetEmpresaId();
            var precios = await _context.ContPreciosDiesel
                .Where(p => p.EmpresaId == empresaId)
                .OrderByDescending(p => p.FechaRegistro)
                .ToListAsync();
            return Ok(precios);
        }

        [HttpGet("vigente")]
        public async Task<IActionResult> GetVigente()
        {
            var empresaId = GetEmpresaId();
            var hoy = DateTime.Today;
            var precio = await _context.ContPreciosDiesel
                .Where(p => p.EmpresaId == empresaId && p.FechaRegistro <= hoy && p.FechaExpiro >= hoy)
                .FirstOrDefaultAsync();
            if (precio == null) return NotFound(new { mensaje = "No hay precio vigente." });
            return Ok(precio);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContPreciosDiesel precio)
        {
            precio.EmpresaId = GetEmpresaId();
            _context.ContPreciosDiesel.Add(precio);
            await _context.SaveChangesAsync();
            return Ok(precio);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ContPreciosDiesel precio)
        {
            var empresaId = GetEmpresaId();
            var existing = await _context.ContPreciosDiesel
                .FirstOrDefaultAsync(p => p.IdDiesel == id && p.EmpresaId == empresaId);
            if (existing == null) return NotFound();

            existing.Precio = precio.Precio;
            existing.FechaRegistro = precio.FechaRegistro;
            existing.FechaExpiro = precio.FechaExpiro;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var empresaId = GetEmpresaId();
            var precio = await _context.ContPreciosDiesel
                .FirstOrDefaultAsync(p => p.IdDiesel == id && p.EmpresaId == empresaId);
            if (precio == null) return NotFound();
            _context.ContPreciosDiesel.Remove(precio);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}