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
    public class ContConsumoUnidadesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ContConsumoUnidadesController(AppDbContext context) => _context = context;

        private int GetEmpresaId() => int.Parse(User.FindFirst("EmpresaId")!.Value);

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var empresaId = GetEmpresaId();
            var consumos = await _context.ContConsumoUnidades
                .Where(c => c.EmpresaId == empresaId)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();
            return Ok(consumos);
        }

        [HttpGet("unidad/{idUnidad}")]
        public async Task<IActionResult> GetByUnidad(int idUnidad)
        {
            var empresaId = GetEmpresaId();
            var consumos = await _context.ContConsumoUnidades
                .Where(c => c.id_Unidad == idUnidad && c.EmpresaId == empresaId)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();
            return Ok(consumos);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContConsumoUnidades consumo)
        {
            consumo.EmpresaId = GetEmpresaId();
            _context.ContConsumoUnidades.Add(consumo);
            await _context.SaveChangesAsync();
            return Ok(consumo);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] ContConsumoUnidades consumo)
        {
            var empresaId = GetEmpresaId();
            var existing = await _context.ContConsumoUnidades
                .FirstOrDefaultAsync(c => c.id_Unidad == consumo.id_Unidad && c.Fecha == consumo.Fecha && c.EmpresaId == empresaId);
            if (existing == null) return NotFound();
            _context.ContConsumoUnidades.Remove(existing);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}