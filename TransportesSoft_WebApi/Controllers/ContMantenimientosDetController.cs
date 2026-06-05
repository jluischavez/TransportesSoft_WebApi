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
    public class ContMantenimientosDetController : BaseApiController
    {
        private readonly AppDbContext _context;
        public ContMantenimientosDetController(AppDbContext context) => _context = context;

        private int GetEmpresaId() => int.Parse(User.FindFirst("EmpresaId")!.Value);

        [HttpGet("{idMantenimiento}")]
        public async Task<IActionResult> GetByMantenimiento(int idMantenimiento)
        {
            var empresaId = GetEmpresaId();
            var detalle = await _context.ContMantenimientosDet
                .Where(d => d.IdMantenimiento == idMantenimiento && d.EmpresaId == empresaId)
                .OrderBy(d => d.Renglon)
                .ToListAsync();
            return Ok(detalle);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContMantenimientosDet detalle)
        {
            detalle.EmpresaId = GetEmpresaId();
            _context.ContMantenimientosDet.Add(detalle);
            await _context.SaveChangesAsync();
            return Ok(detalle);
        }

        [HttpPut("{idMantenimiento}/{renglon}")]
        public async Task<IActionResult> Update(int idMantenimiento, int renglon, ContMantenimientosDet detalle)
        {
            var empresaId = GetEmpresaId();
            var existing = await _context.ContMantenimientosDet
                .FirstOrDefaultAsync(d => d.IdMantenimiento == idMantenimiento && d.Renglon == renglon && d.EmpresaId == empresaId);
            if (existing == null) return NotFound();

            existing.Refaccion = detalle.Refaccion;
            existing.Proveedor = detalle.Proveedor;
            existing.PrecioRefaccion = detalle.PrecioRefaccion;
            existing.Comentarios = detalle.Comentarios;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{idMantenimiento}/{renglon}")]
        public async Task<IActionResult> Delete(int idMantenimiento, int renglon)
        {
            var empresaId = GetEmpresaId();
            var detalle = await _context.ContMantenimientosDet
                .FirstOrDefaultAsync(d => d.IdMantenimiento == idMantenimiento && d.Renglon == renglon && d.EmpresaId == empresaId);
            if (detalle == null) return NotFound();
            _context.ContMantenimientosDet.Remove(detalle);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}