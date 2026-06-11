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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var empresaId = GetEmpresaId();
                var precios = await _context.ContPreciosDiesel
                    .Where(p => p.EmpresaId == empresaId)
                    .OrderByDescending(p => p.FechaRegistro)
                    .ToListAsync();
                return Ok(precios);
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al obtener los precios." });
            }
            
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(ContPreciosDiesel precio)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                precio.EmpresaId = ObtenerEmpresaId();
                if (precio.EmpresaId == null)
                    return SinEmpresaAsignada();

                // expirar el precio anterior
                var precioAnterior = await _context.ContPreciosDiesel
                    .Where(p => p.EmpresaId == precio.EmpresaId)
                    .OrderByDescending(p => p.FechaRegistro)
                    .ThenByDescending(p => p.IdDiesel)
                    .FirstOrDefaultAsync();

                if (precioAnterior != null)
                    precioAnterior.FechaExpiro = DateTime.Today;

                precio.FechaRegistro = DateTime.Today;
                _context.ContPreciosDiesel.Add(precio);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(precio);
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Error al guardar." });
            }
        }

        [Authorize]
        [HttpGet("Precio-Actual")]
        public async Task<IActionResult> GetPrecioActual()
        {
            try
            {
                /*PRECIO VIGENTE*/
                var empresaId = ObtenerEmpresaId();

                if (empresaId == null)
                    return SinEmpresaAsignada();

                var precioActual = await _context.ContPreciosDiesel
                    .Where(p => p.EmpresaId == empresaId)
                    .OrderByDescending(p => p.FechaRegistro)
                    .ThenByDescending(p => p.IdDiesel)
                    .FirstOrDefaultAsync();

                if (precioActual == null)
                    return NotFound(new { mensaje = "No se encontró un precio registrado para esta empresa." });
                return Ok(precioActual);
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al obtener el precio actual." });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var empresaId = GetEmpresaId();
                var precio = await _context.ContPreciosDiesel
                    .FirstOrDefaultAsync(p => p.IdDiesel == id && p.EmpresaId == empresaId);
                if (precio == null) return NotFound();
                _context.ContPreciosDiesel.Remove(precio);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el precio." });
            }
            
        }
    }
}