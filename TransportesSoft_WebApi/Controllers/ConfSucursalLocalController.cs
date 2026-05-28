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
    public class ConfSucursalLocalController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ConfSucursalLocalController(AppDbContext context) => _context = context;

        private int GetEmpresaId() => int.Parse(User.FindFirst("EmpresaId")!.Value);

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var empresaId = GetEmpresaId();
            var sucursales = await _context.ConfSucursalLocal
                .Where(s => s.EmpresaId == empresaId)
                .ToListAsync();
            return Ok(sucursales);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ConfSucursalLocal sucursal)
        {
            var empresaId = GetEmpresaId();
            var existing = await _context.ConfSucursalLocal
                .FirstOrDefaultAsync(s => s.id_Sucursal == id && s.EmpresaId == empresaId);
            if (existing == null) return NotFound();

            existing.NombreSucursal = sucursal.NombreSucursal;
            existing.Direccion = sucursal.Direccion;
            existing.Telefono = sucursal.Telefono;
            existing.URLImagen = sucursal.URLImagen;
            existing.KilometrajeNotificaciones = sucursal.KilometrajeNotificaciones;
            existing.RutaReportes = sucursal.RutaReportes;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }
    }
}