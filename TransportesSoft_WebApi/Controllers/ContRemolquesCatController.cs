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
    public class ContRemolquesCatController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ContRemolquesCatController(AppDbContext context) => _context = context;

        private int GetEmpresaId() => int.Parse(User.FindFirst("EmpresaId")!.Value);

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var empresaId = GetEmpresaId();
            var remolques = await _context.ContRemolquesCat
                .Where(r => r.EmpresaId == empresaId)
                .ToListAsync();
            return Ok(remolques);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var empresaId = GetEmpresaId();
            var remolque = await _context.ContRemolquesCat
                .FirstOrDefaultAsync(r => r.id_Remolque == id && r.EmpresaId == empresaId);
            if (remolque == null) return NotFound();
            return Ok(remolque);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContRemolquesCat remolque)
        {
            remolque.EmpresaId = GetEmpresaId();
            _context.ContRemolquesCat.Add(remolque);
            await _context.SaveChangesAsync();
            return Ok(remolque);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ContRemolquesCat remolque)
        {
            var empresaId = GetEmpresaId();
            var existing = await _context.ContRemolquesCat
                .FirstOrDefaultAsync(r => r.id_Remolque == id && r.EmpresaId == empresaId);
            if (existing == null) return NotFound();

            existing.Marca = remolque.Marca;
            existing.Modelo = remolque.Modelo;
            existing.Serie = remolque.Serie;
            existing.Year = remolque.Year;
            existing.Placas = remolque.Placas;
            existing.Fecha_Llantas = remolque.Fecha_Llantas;
            existing.Fecha_Fisico_SCT = remolque.Fecha_Fisico_SCT;
            existing.Impermeabilizacion = remolque.Impermeabilizacion;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var empresaId = GetEmpresaId();
            var remolque = await _context.ContRemolquesCat
                .FirstOrDefaultAsync(r => r.id_Remolque == id && r.EmpresaId == empresaId);
            if (remolque == null) return NotFound();
            _context.ContRemolquesCat.Remove(remolque);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}