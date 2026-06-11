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
    public class ContConsumoUnidadesController : BaseApiController
    {
        private readonly AppDbContext _context;
        public ContConsumoUnidadesController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var empresaId = ObtenerEmpresaId();

                if (empresaId == null)
                    return SinEmpresaAsignada();

                var consumos = await _context.ContConsumoUnidades
                    .Where(c => c.EmpresaId == empresaId)
                    .OrderByDescending(c => c.Fecha)
                    .ToListAsync();
                return Ok(consumos);
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al obtener los consumos." });
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

                var consumos = await _context.ContConsumoUnidades
                    .Where(c => c.EmpresaId == empresaId.Value)
                    .GroupBy(c => c.id_Unidad)
                    .Select(g => g
                        .OrderByDescending(c => c.Fecha)
                        .ThenByDescending(c => c.Id)
                        .First()
                    )
                    .ToListAsync();

                return Ok(consumos);
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al obtener los consumos." });
            }
        }

        [HttpGet("unidad/{idUnidad}")]
        public async Task<IActionResult> GetByUnidad(int idUnidad)
        {
            try
            {
                var empresaId = ObtenerEmpresaId();

                if (empresaId == null)
                    return SinEmpresaAsignada();

                var consumos = await _context.ContConsumoUnidades
                    .Where(c => c.id_Unidad == idUnidad && c.EmpresaId == empresaId)
                    .OrderByDescending(c => c.Fecha)
                    .ToListAsync();
                return Ok(consumos);
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al obtener los consumos por unidad." });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(ContConsumoUnidades consumo)
        {
            try
            {
                consumo.EmpresaId = ObtenerEmpresaId();

                if (consumo.EmpresaId == null)
                    return SinEmpresaAsignada();

                _context.ContConsumoUnidades.Add(consumo);
                await _context.SaveChangesAsync();
                return Ok(consumo);
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al guardar el consumo." });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] ContConsumoUnidades consumo)
        {
            try
            {
                var empresaId = ObtenerEmpresaId();

                if (empresaId == null)
                    return SinEmpresaAsignada();

                var existing = await _context.ContConsumoUnidades
                    .FirstOrDefaultAsync(c => c.id_Unidad == consumo.id_Unidad && c.Fecha == consumo.Fecha && c.EmpresaId == empresaId);
                if (existing == null) return NotFound();
                _context.ContConsumoUnidades.Remove(existing);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el consumo." });
            }
           
        }
    }
}