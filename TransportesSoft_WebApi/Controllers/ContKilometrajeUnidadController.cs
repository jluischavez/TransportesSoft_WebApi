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
    public class ContKilometrajeUnidadController : BaseApiController
    {
        private readonly AppDbContext _context;
        public ContKilometrajeUnidadController(AppDbContext context) => _context = context;

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var empresaId = ObtenerEmpresaId();

                if (empresaId == null)
                    return SinEmpresaAsignada();

                var kilometrajes = await _context.ContKilometrajeUnidad
                    .Where(k => k.EmpresaId == empresaId)
                    .OrderByDescending(k => k.FechaRegistro)
                    .ToListAsync();
                return Ok(kilometrajes);
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al obtener los kilometrajes." });
            }
        }

        [Authorize]
        [HttpGet("GetAllPorUnidad")]
        public async Task<IActionResult> GetAllPorUnidad()
        {
            try
            {
                var empresaId = ObtenerEmpresaId();

                if (empresaId == null)
                    return SinEmpresaAsignada();

                //Obtener los kilometrajes por unidad
                var kilometrajes = await _context.ContKilometrajeUnidad
                .Where(k => k.EmpresaId == empresaId)
                .GroupBy(k => k.id_Unidad)
                .Select(g => g
                    .OrderByDescending(k => k.FechaRegistro)
                    .ThenByDescending(k => k.Id)
                    .FirstOrDefault()
                )
                .ToListAsync();

                return Ok(kilometrajes);
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al obtener los kilometrajes." });
            }
        }

        [Authorize]
        [HttpGet("unidad/{idUnidad}")]
        public async Task<IActionResult> GetByUnidad(int idUnidad)
        {
            try
            {
                var empresaId = ObtenerEmpresaId();

                if (empresaId == null)
                    return SinEmpresaAsignada();

                var kilometrajes = await _context.ContKilometrajeUnidad
                    .Where(k => k.id_Unidad == idUnidad && k.EmpresaId == empresaId)
                    .OrderByDescending(k => k.FechaRegistro)
                    .ToListAsync();
                return Ok(kilometrajes);
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al obtener los kilometrajes por unidad." });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(ContKilometrajeUnidad kilometraje)
        {
            try
            {
                kilometraje.EmpresaId = ObtenerEmpresaId();

                if (kilometraje.EmpresaId == null)
                    return SinEmpresaAsignada();

                    kilometraje.FechaRegistro = DateTime.Now;
                    _context.ContKilometrajeUnidad.Add(kilometraje);
                    await _context.SaveChangesAsync();
                    return Ok(kilometraje);
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al registrar el kilometraje." });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var empresaId = ObtenerEmpresaId();

                if (empresaId == null)
                    return SinEmpresaAsignada();

                var kilometraje = await _context.ContKilometrajeUnidad
                    .FirstOrDefaultAsync(k => k.Id == id && k.EmpresaId == empresaId);
                if (kilometraje == null) return NotFound();
                _context.ContKilometrajeUnidad.Remove(kilometraje);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el kilometraje." });
            }
        }
    }
}