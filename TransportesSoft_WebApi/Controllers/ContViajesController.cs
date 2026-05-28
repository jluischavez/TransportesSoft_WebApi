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
    public class ContViajesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ContViajesController(AppDbContext context) => _context = context;

        private int GetEmpresaId() => int.Parse(User.FindFirst("EmpresaId")!.Value);

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var empresaId = GetEmpresaId();
            var viajes = await _context.ContViajes
                .Where(v => v.EmpresaId == empresaId)
                .OrderByDescending(v => v.FechaViaje)
                .ToListAsync();
            return Ok(viajes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var empresaId = GetEmpresaId();
            var viaje = await _context.ContViajes
                .FirstOrDefaultAsync(v => v.id_Viaje == id && v.EmpresaId == empresaId);
            if (viaje == null) return NotFound();
            return Ok(viaje);
        }

        [HttpGet("cliente/{idCliente}")]
        public async Task<IActionResult> GetByCliente(int idCliente)
        {
            var empresaId = GetEmpresaId();
            var viajes = await _context.ContViajes
                .Where(v => v.id_Client == idCliente && v.EmpresaId == empresaId)
                .OrderByDescending(v => v.FechaViaje)
                .ToListAsync();
            return Ok(viajes);
        }

        [HttpGet("unidad/{idUnidad}")]
        public async Task<IActionResult> GetByUnidad(int idUnidad)
        {
            var empresaId = GetEmpresaId();
            var viajes = await _context.ContViajes
                .Where(v => v.id_Unidad == idUnidad && v.EmpresaId == empresaId)
                .OrderByDescending(v => v.FechaViaje)
                .ToListAsync();
            return Ok(viajes);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContViajes viaje)
        {
            viaje.EmpresaId = GetEmpresaId();
            _context.ContViajes.Add(viaje);
            await _context.SaveChangesAsync();
            return Ok(viaje);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ContViajes viaje)
        {
            var empresaId = GetEmpresaId();
            var existing = await _context.ContViajes
                .FirstOrDefaultAsync(v => v.id_Viaje == id && v.EmpresaId == empresaId);
            if (existing == null) return NotFound();

            existing.id_Client = viaje.id_Client;
            existing.NombreCliente = viaje.NombreCliente;
            existing.FechaViaje = viaje.FechaViaje;
            existing.FechaFactura = viaje.FechaFactura;
            existing.Factura = viaje.Factura;
            existing.NumeroTransporte = viaje.NumeroTransporte;
            existing.Origen = viaje.Origen;
            existing.Destino = viaje.Destino;
            existing.Monto = viaje.Monto;
            existing.IVA = viaje.IVA;
            existing.Retenciones = viaje.Retenciones;
            existing.Total = viaje.Total;
            existing.Comentarios = viaje.Comentarios;
            existing.Maniobra = viaje.Maniobra;
            existing.id_Operador = viaje.id_Operador;
            existing.id_Unidad = viaje.id_Unidad;
            existing.id_Remolque = viaje.id_Remolque;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var empresaId = GetEmpresaId();
            var viaje = await _context.ContViajes
                .FirstOrDefaultAsync(v => v.id_Viaje == id && v.EmpresaId == empresaId);
            if (viaje == null) return NotFound();
            _context.ContViajes.Remove(viaje);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET /ContViajes/validar-factura/{folio}
        [HttpGet("validar-factura/{folio}")]
        public async Task<IActionResult> ValidarFactura(string folio)
        {
            var empresaId = GetEmpresaId();
            var existe = await _context.ContViajes
                .AnyAsync(v => v.Factura == folio && v.EmpresaId == empresaId);
            return Ok(new { disponible = !existe });
        }

        // GET /ContViajes/validar-transporte/{numero}
        [HttpGet("validar-transporte/{numero}")]
        public async Task<IActionResult> ValidarTransporte(string numero)
        {
            var empresaId = GetEmpresaId();
            var existe = await _context.ContViajes
                .AnyAsync(v => v.NumeroTransporte == numero && v.EmpresaId == empresaId);
            return Ok(new { disponible = !existe });
        }
    }
}