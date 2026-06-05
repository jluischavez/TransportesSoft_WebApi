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
    public class ContMantenimientosCabController : BaseApiController
    {
        private readonly AppDbContext _context;
        public ContMantenimientosCabController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var empresaId = ObtenerEmpresaId();

            if (empresaId == null)
                return SinEmpresaAsignada();

            var mantenimientos = await _context.ContMantenimientosCab
                .Where(m => m.EmpresaId == empresaId)
                .OrderByDescending(m => m.FechaMantenimiento)
                .ToListAsync();
            return Ok(mantenimientos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var empresaId = ObtenerEmpresaId();

            if (empresaId == null)
                return SinEmpresaAsignada();

            var mantenimiento = await _context.ContMantenimientosCab
                .FirstOrDefaultAsync(m => m.IdMantenimiento == id && m.EmpresaId == empresaId);
            if (mantenimiento == null) return NotFound();
            return Ok(mantenimiento);
        }

        [HttpGet("{id}/detalle")]
        public async Task<IActionResult> GetDetalle(int id)
        {
            var empresaId = ObtenerEmpresaId();

            if (empresaId == null)
                return SinEmpresaAsignada();

            var detalle = await _context.ContMantenimientosDet
                .Where(d => d.IdMantenimiento == id && d.EmpresaId == empresaId)
                .ToListAsync();
            return Ok(detalle);
        }

        [HttpGet("unidad/{idUnidad}")]
        public async Task<IActionResult> GetByUnidad(int idUnidad)
        {
            var empresaId = ObtenerEmpresaId();

            if (empresaId == null)
                return SinEmpresaAsignada();

            var mantenimientos = await _context.ContMantenimientosCab
                .Where(m => m.id_Unidad == idUnidad && m.EmpresaId == empresaId)
                .OrderByDescending(m => m.FechaMantenimiento)
                .ToListAsync();
            return Ok(mantenimientos);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContMantenimientosCab mantenimiento)
        {
            mantenimiento.EmpresaId = ObtenerEmpresaId();

            if (mantenimiento.EmpresaId == null)
                return SinEmpresaAsignada();

            _context.ContMantenimientosCab.Add(mantenimiento);
            await _context.SaveChangesAsync();
            return Ok(mantenimiento);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ContMantenimientosCab mantenimiento)
        {
            var empresaId = ObtenerEmpresaId();

            if (empresaId == null)
                return SinEmpresaAsignada();

            var existing = await _context.ContMantenimientosCab
                .FirstOrDefaultAsync(m => m.IdMantenimiento == id && m.EmpresaId == empresaId);
            if (existing == null) return NotFound();

            existing.FechaMantenimiento = mantenimiento.FechaMantenimiento;
            existing.Kilometraje = mantenimiento.Kilometraje;
            existing.Proveedor = mantenimiento.Proveedor;
            existing.CostoTotal = mantenimiento.CostoTotal;
            existing.id_Unidad = mantenimiento.id_Unidad;
            existing.id_Remolque = mantenimiento.id_Remolque;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var empresaId = ObtenerEmpresaId();

            if (empresaId == null)
                return SinEmpresaAsignada();

            var mantenimiento = await _context.ContMantenimientosCab
                .FirstOrDefaultAsync(m => m.IdMantenimiento == id && m.EmpresaId == empresaId);
            if (mantenimiento == null) return NotFound();

            // borrar detalle primero
            var detalle = await _context.ContMantenimientosDet
                .Where(d => d.IdMantenimiento == id && d.EmpresaId == empresaId)
                .ToListAsync();
            _context.ContMantenimientosDet.RemoveRange(detalle);

            _context.ContMantenimientosCab.Remove(mantenimiento);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}