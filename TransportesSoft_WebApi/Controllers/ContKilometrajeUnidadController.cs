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
    public class ContKilometrajeUnidadController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ContKilometrajeUnidadController(AppDbContext context) => _context = context;

        private int GetEmpresaId() => int.Parse(User.FindFirst("EmpresaId")!.Value);

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var empresaId = GetEmpresaId();
            var kilometrajes = await _context.ContKilometrajeUnidad
                .Where(k => k.EmpresaId == empresaId)
                .OrderByDescending(k => k.FechaRegistro)
                .ToListAsync();
            return Ok(kilometrajes);
        }

        [HttpGet("unidad/{idUnidad}")]
        public async Task<IActionResult> GetByUnidad(int idUnidad)
        {
            var empresaId = GetEmpresaId();
            var kilometrajes = await _context.ContKilometrajeUnidad
                .Where(k => k.id_Unidad == idUnidad && k.EmpresaId == empresaId)
                .OrderByDescending(k => k.FechaRegistro)
                .ToListAsync();
            return Ok(kilometrajes);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContKilometrajeUnidad kilometraje)
        {
            kilometraje.EmpresaId = GetEmpresaId();
            kilometraje.FechaRegistro = DateTime.Now;
            _context.ContKilometrajeUnidad.Add(kilometraje);
            await _context.SaveChangesAsync();
            return Ok(kilometraje);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var empresaId = GetEmpresaId();
            var kilometraje = await _context.ContKilometrajeUnidad
                .FirstOrDefaultAsync(k => k.Id == id && k.EmpresaId == empresaId);
            if (kilometraje == null) return NotFound();
            _context.ContKilometrajeUnidad.Remove(kilometraje);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}