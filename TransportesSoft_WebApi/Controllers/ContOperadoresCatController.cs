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
    public class ContOperadoresCatController : BaseApiController
    {
        private readonly AppDbContext _context;
        public ContOperadoresCatController(AppDbContext context) => _context = context;

        private int GetEmpresaId() => int.Parse(User.FindFirst("EmpresaId")!.Value);

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var empresaId = GetEmpresaId();
            var operadores = await _context.ContOperadoresCat
                .Where(o => o.EmpresaId == empresaId)
                .ToListAsync();
            return Ok(operadores);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var empresaId = GetEmpresaId();
            var operador = await _context.ContOperadoresCat
                .FirstOrDefaultAsync(o => o.id_Operador == id && o.EmpresaId == empresaId);
            if (operador == null) return NotFound();
            return Ok(operador);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContOperadoresCat operador)
        {
            operador.EmpresaId = GetEmpresaId();
            _context.ContOperadoresCat.Add(operador);
            await _context.SaveChangesAsync();
            return Ok(operador);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ContOperadoresCat operador)
        {
            var empresaId = GetEmpresaId();
            var existing = await _context.ContOperadoresCat
                .FirstOrDefaultAsync(o => o.id_Operador == id && o.EmpresaId == empresaId);
            if (existing == null) return NotFound();

            existing.Nombre = operador.Nombre;
            existing.FechaIngreso = operador.FechaIngreso;
            existing.FechaEgreso = operador.FechaEgreso;
            existing.Estatus = operador.Estatus;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var empresaId = GetEmpresaId();
            var operador = await _context.ContOperadoresCat
                .FirstOrDefaultAsync(o => o.id_Operador == id && o.EmpresaId == empresaId);
            if (operador == null) return NotFound();
            _context.ContOperadoresCat.Remove(operador);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}