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
    public class ContPolizasRegController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ContPolizasRegController(AppDbContext context) => _context = context;

        private int GetEmpresaId() => int.Parse(User.FindFirst("EmpresaId")!.Value);

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var empresaId = GetEmpresaId();
            var polizas = await _context.ContPolizasReg
                .Where(p => p.EmpresaId == empresaId)
                .OrderByDescending(p => p.FechaRegistro)
                .ToListAsync();
            return Ok(polizas);
        }

        [HttpGet("vigentes")]
        public async Task<IActionResult> GetVigentes()
        {
            var empresaId = GetEmpresaId();
            var hoy = DateTime.Today;
            var polizas = await _context.ContPolizasReg
                .Where(p => p.EmpresaId == empresaId && p.FechaExpira >= hoy)
                .OrderBy(p => p.FechaExpira)
                .ToListAsync();
            return Ok(polizas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var empresaId = GetEmpresaId();
            var poliza = await _context.ContPolizasReg
                .FirstOrDefaultAsync(p => p.id == id && p.EmpresaId == empresaId);
            if (poliza == null) return NotFound();
            return Ok(poliza);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContPolizasReg poliza)
        {
            poliza.EmpresaId = GetEmpresaId();
            _context.ContPolizasReg.Add(poliza);
            await _context.SaveChangesAsync();
            return Ok(poliza);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ContPolizasReg poliza)
        {
            var empresaId = GetEmpresaId();
            var existing = await _context.ContPolizasReg
                .FirstOrDefaultAsync(p => p.id == id && p.EmpresaId == empresaId);
            if (existing == null) return NotFound();

            existing.FolioPoliza = poliza.FolioPoliza;
            existing.FechaRegistro = poliza.FechaRegistro;
            existing.FechaExpira = poliza.FechaExpira;
            existing.idTipoPoliza = poliza.idTipoPoliza;
            existing.idUsuario = poliza.idUsuario;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var empresaId = GetEmpresaId();
            var poliza = await _context.ContPolizasReg
                .FirstOrDefaultAsync(p => p.id == id && p.EmpresaId == empresaId);
            if (poliza == null) return NotFound();
            _context.ContPolizasReg.Remove(poliza);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}