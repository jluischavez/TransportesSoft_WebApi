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
    public class ContUnidadesCatController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ContUnidadesCatController(AppDbContext context) => _context = context;

        private int GetEmpresaId()
        {
            var claim = User.FindFirst("EmpresaId");
            if (claim == null) return 0;
            return int.Parse(claim.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var empresaId = GetEmpresaId();
            Console.WriteLine($"EmpresaId del token: {empresaId}");

            if (empresaId == 0)
                return Unauthorized(new { mensaje = "No se encontró EmpresaId en el token." });

            var unidades = await _context.ContUnidadesCat
                .Where(c => c.EmpresaId == empresaId)
                .ToListAsync();

            return Ok(unidades);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var empresaId = GetEmpresaId();
            var unidad = await _context.ContUnidadesCat
                .FirstOrDefaultAsync(u => u.id_Unidad == id && u.EmpresaId == empresaId);
            if (unidad == null) return NotFound();
            return Ok(unidad);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContUnidadesCat unidad)
        {
            unidad.EmpresaId = GetEmpresaId();
            unidad.FechaActualizacion = DateTime.Now;
            _context.ContUnidadesCat.Add(unidad);
            await _context.SaveChangesAsync();
            return Ok(unidad);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ContUnidadesCat unidad)
        {
            var empresaId = GetEmpresaId();
            var existing = await _context.ContUnidadesCat
                .FirstOrDefaultAsync(u => u.id_Unidad == id && u.EmpresaId == empresaId);
            if (existing == null) return NotFound();

            existing.Marca = unidad.Marca;
            existing.Serie = unidad.Serie;
            existing.id_Operador = unidad.id_Operador;
            existing.Estatus = unidad.Estatus;
            existing.id_Remolque = unidad.id_Remolque;
            existing.FechaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var empresaId = GetEmpresaId();
            var unidad = await _context.ContUnidadesCat
                .FirstOrDefaultAsync(u => u.id_Unidad == id && u.EmpresaId == empresaId);
            if (unidad == null) return NotFound();
            _context.ContUnidadesCat.Remove(unidad);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET /ContUnidadesCat/operador/{idOperador}
        [HttpGet("operador/{idOperador}")]
        public async Task<IActionResult> GetByOperador(int idOperador)
        {
            var empresaId = GetEmpresaId();
            var unidad = await _context.ContUnidadesCat
                .FirstOrDefaultAsync(u => u.id_Operador == idOperador && u.EmpresaId == empresaId && u.Estatus == "A");
            if (unidad == null)
                return NotFound(new { mensaje = "El operador no tiene una unidad activa asignada." });
            return Ok(unidad);
        }
    }
}