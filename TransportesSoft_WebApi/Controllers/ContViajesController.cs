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
    public class ContViajesController : BaseApiController
    {
        private readonly AppDbContext _context;
        public ContViajesController(AppDbContext context) => _context = context;

        private int GetEmpresaId() => int.Parse(User.FindFirst("EmpresaId")!.Value);

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var empresaId = ObtenerEmpresaId();

                if (empresaId == null)
                    return SinEmpresaAsignada();

                var viajes = await _context.ContViajes
                    .Where(v => v.EmpresaId == empresaId)
                    .OrderByDescending(v => v.FechaViaje)
                    .ToListAsync();
                return Ok(viajes);
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al obtener los viajes." });
            }
            
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var empresaId = ObtenerEmpresaId();

                if (empresaId == null)
                    return SinEmpresaAsignada();

                var viaje = await _context.ContViajes
                    .FirstOrDefaultAsync(v => v.id_Viaje == id && v.EmpresaId == empresaId);
                if (viaje == null) return NotFound();
                return Ok(viaje);
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al obtener el viaje por ID." });
            }
        }

        [Authorize]
        [HttpGet("cliente/{idCliente}")]
        public async Task<IActionResult> GetByCliente(int idCliente)
        {
            try
            {
                var empresaId = ObtenerEmpresaId();

                if (empresaId == null)
                    return SinEmpresaAsignada();

                var viajes = await _context.ContViajes
                    .Where(v => v.id_Client == idCliente && v.EmpresaId == empresaId)
                    .OrderByDescending(v => v.FechaViaje)
                    .ToListAsync();
                return Ok(viajes);
            } catch
            {
                return StatusCode(500, new { mensaje = "Error al obtener los viajes por cliente." });
            }
            
        }

        [Authorize]
        [HttpGet("unidad/{idUnidad}")]
        public async Task<IActionResult> GetByUnidad(int idUnidad)
        {
            try
            {
                var empresaId = GetEmpresaId();
                var viajes = await _context.ContViajes
                    .Where(v => v.id_Unidad == idUnidad && v.EmpresaId == empresaId)
                    .OrderByDescending(v => v.FechaViaje)
                    .ToListAsync();
                return Ok(viajes);
            } catch
            {
                return StatusCode(500, new { mensaje = "Error al obtener los viajes por unidad." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContViajes viaje)
        {
            try
            {
                viaje.EmpresaId = ObtenerEmpresaId();

                if (viaje.EmpresaId == null)
                    return SinEmpresaAsignada();

                _context.ContViajes.Add(viaje);
                await _context.SaveChangesAsync();
                return Ok(viaje);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear el viaje.", detalle = ex.Message, inner = ex.InnerException?.Message });
            }
            
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ContViajes viaje)
        {
            try
            {
                var empresaId = ObtenerEmpresaId();

                if (empresaId == null)
                    return SinEmpresaAsignada();

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
            catch
            {
                return StatusCode(500, new { mensaje = "Error al actualizar el viaje." });
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

                var viaje = await _context.ContViajes
                    .FirstOrDefaultAsync(v => v.id_Viaje == id && v.EmpresaId == empresaId);
                if (viaje == null) return NotFound();
                _context.ContViajes.Remove(viaje);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el viaje." });
            }
            
        }

        // GET /ContViajes/validar-factura/{folio}
        [Authorize]
        [HttpGet("validar-factura/{folio}")]
        public async Task<IActionResult> ValidarFactura(string folio)
        {
            try
            {
                var empresaId = ObtenerEmpresaId();

                if (empresaId == null)
                    return SinEmpresaAsignada();

                var existe = await _context.ContViajes
                    .AnyAsync(v => v.Factura == folio && v.EmpresaId == empresaId);
                return Ok(new { disponible = !existe });
            }
            catch
            {
                               return StatusCode(500, new { mensaje = "Error al validar el folio de factura." });
            }
        }

        // GET /ContViajes/validar-transporte/{numero}
        [Authorize]
        [HttpGet("validar-transporte/{numero}")]
        public async Task<IActionResult> ValidarTransporte(string numero)
        {
            try
            {
                var empresaId = ObtenerEmpresaId();

                if (empresaId == null)
                    return SinEmpresaAsignada();

                var existe = await _context.ContViajes
                    .AnyAsync(v => v.NumeroTransporte == numero && v.EmpresaId == empresaId);
                return Ok(new { disponible = !existe });
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al validar el número de transporte." });
            }
           
        }


        [Authorize]
        [HttpGet("reporte")]
        public async Task<IActionResult> Reporte(DateTime fechaInicio, DateTime fechaFin, string tipoFecha = "FechaViaje", int? idUnidad = null)
        {
            try
            {
                var empresaId = ObtenerEmpresaId();

                if (empresaId == null)
                    return SinEmpresaAsignada();

                fechaFin = fechaFin.Date.AddDays(1).AddTicks(-1);

                var query = _context.ContViajes
                    .Where(v => v.EmpresaId == empresaId.Value);

                if (tipoFecha == "FechaFactura")
                {
                    query = query.Where(v => v.FechaFactura >= fechaInicio && v.FechaFactura <= fechaFin);
                }
                else
                {
                    query = query.Where(v => v.FechaViaje >= fechaInicio && v.FechaViaje <= fechaFin);
                }

                if (idUnidad.HasValue)
                {
                    query = query.Where(v => v.id_Unidad == idUnidad.Value);
                }

                var viajes = await query
                .OrderBy(v => v.FechaViaje)
                .Select(v => new
                {
                    v.id_Viaje,
                    v.NombreCliente,
                    v.FechaViaje,
                    v.FechaFactura,
                    v.Factura,
                    v.NumeroTransporte,
                    v.Origen,
                    v.Destino,
                    v.Monto,
                    v.IVA,
                    v.Retenciones,
                    v.Maniobra,
                    v.Total,
                    v.Comentarios,

                    operador = _context.ContOperadoresCat
                        .Where(o => o.id_Operador == v.id_Operador && o.EmpresaId == empresaId.Value)
                        .Select(o => o.Nombre)
                        .FirstOrDefault() ?? v.id_Operador.ToString(),

                    unidad = _context.ContUnidadesCat
                        .Where(u => u.id_Unidad == v.id_Unidad && u.EmpresaId == empresaId.Value)
                        .Select(u => u.id_Unidad + " — " + u.Marca + " | " + u.Serie)
                        .FirstOrDefault() ?? v.id_Unidad.ToString(),

                    remolque = _context.ContRemolquesCat
                        .Where(r => r.id_Remolque == v.id_Remolque && r.EmpresaId == empresaId.Value)
                        .Select(r => r.id_Remolque + " — " + r.Marca + " - " + r.Modelo)
                        .FirstOrDefault() ?? v.id_Remolque.ToString()
                })
                .ToListAsync();

                return Ok(viajes);

            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al generar el reporte." });
            }
        }
    }
}