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

        [Authorize]
        [HttpGet("reporte")]
        public async Task<IActionResult> Reporte(
        DateTime fechaInicio,
        DateTime fechaFin,
        string? tipoEquipo = null,
        int? idEquipo = null)
        {
            try
            {
                var empresaId = ObtenerEmpresaId();

                if (empresaId == null)
                    return SinEmpresaAsignada();

                fechaInicio = fechaInicio.Date;
                fechaFin = fechaFin.Date.AddDays(1).AddTicks(-1);

                var queryCab = _context.ContMantenimientosCab
                    .Where(c => c.EmpresaId == empresaId.Value)
                    .Where(c =>
                        c.FechaMantenimiento >= fechaInicio &&
                        c.FechaMantenimiento <= fechaFin);

                if (!string.IsNullOrWhiteSpace(tipoEquipo) && idEquipo.HasValue)
                {
                    tipoEquipo = tipoEquipo.Trim().ToLower();

                    if (tipoEquipo == "unidad")
                    {
                        queryCab = queryCab.Where(c => c.id_Unidad == idEquipo.Value);
                    }
                    else if (tipoEquipo == "remolque")
                    {
                        queryCab = queryCab.Where(c => c.id_Remolque == idEquipo.Value);
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            mensaje = "Tipo de equipo inválido. Usa 'unidad' o 'remolque'."
                        });
                    }
                }

                var cabeceras = await queryCab
                    .OrderBy(c => c.FechaMantenimiento)
                    .ThenBy(c => c.IdMantenimiento)
                    .Select(c => new
                    {
                        c.IdMantenimiento,
                        c.FechaMantenimiento,
                        c.Kilometraje,
                        c.Proveedor,
                        c.CostoTotal,
                        c.id_Unidad,
                        c.id_Remolque,

                        unidad = _context.ContUnidadesCat
                            .Where(u =>
                                u.id_Unidad == c.id_Unidad &&
                                u.EmpresaId == empresaId.Value)
                            .Select(u => u.id_Unidad + " — " + u.Marca + " | " + u.Serie)
                            .FirstOrDefault() ?? c.id_Unidad.ToString(),

                        remolque = _context.ContRemolquesCat
                        .Where(r =>
                            r.id_Remolque == c.id_Remolque &&
                            r.EmpresaId == empresaId.Value)
                        .Select(r => r.id_Remolque + " — " + r.Marca + " | " + r.Modelo)
                        .FirstOrDefault() ?? c.id_Remolque.ToString()
                    })
                    .ToListAsync();

                var idsMantenimiento = cabeceras
                    .Select(c => c.IdMantenimiento)
                    .ToList();

                var detalles = await _context.ContMantenimientosDet
                    .Where(d => d.EmpresaId == empresaId.Value)
                    .Where(d => idsMantenimiento.Contains(d.IdMantenimiento))
                    .OrderBy(d => d.IdMantenimiento)
                    .ThenBy(d => d.Renglon)
                    .Select(d => new
                    {
                        d.IdMantenimiento,
                        d.Renglon,
                        d.Refaccion,
                        d.Proveedor,
                        d.PrecioRefaccion,
                        d.Comentarios
                    })
                    .ToListAsync();

                return Ok(new
                {
                    cabeceras,
                    detalles
                });
            }
            catch
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al generar el reporte de mantenimientos."
                });
            }
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

        [Authorize]
        [HttpGet("estado-mantenimiento-unidades")]
        public async Task<IActionResult> EstadoMantenimientoUnidades()
        {
            var empresaId = ObtenerEmpresaId();

            if (empresaId == null)
                return SinEmpresaAsignada();

            const int kmMantenimiento = 15000;
            const int kmAlerta = 3000;

            var unidades = await _context.ContUnidadesCat
                .Where(u => u.EmpresaId == empresaId.Value)
                .Select(u => new
                {
                    u.id_Unidad,
                    u.Marca,
                    u.Serie
                })
                .ToListAsync();

            var resultado = new List<object>();

            foreach (var unidad in unidades)
            {
                var ultimoMantenimiento = await _context.ContMantenimientosCab
                    .Where(m =>
                        m.EmpresaId == empresaId.Value &&
                        m.id_Unidad == unidad.id_Unidad)
                    .OrderByDescending(m => m.FechaMantenimiento)
                    .ThenByDescending(m => m.IdMantenimiento)
                    .FirstOrDefaultAsync();

                var ultimoKilometraje = await _context.ContKilometrajeUnidad
                    .Where(k =>
                        k.EmpresaId == empresaId.Value &&
                        k.id_Unidad == unidad.id_Unidad)
                    .OrderByDescending(k => k.FechaRegistro)
                    .ThenByDescending(k => k.Id)
                    .FirstOrDefaultAsync();

                string estado;
                string color;
                int? kmRecorridos = null;
                int? kmRestantes = null;

                if (ultimoMantenimiento == null)
                {
                    estado = "Sin mantenimiento registrado";
                    color = "rojo";
                }
                else if (ultimoKilometraje == null)
                {
                    estado = "Sin kilometraje registrado";
                    color = "gris";
                }
                else
                {
                    kmRecorridos = ultimoKilometraje.KilometrajeActual - ultimoMantenimiento.Kilometraje;
                    kmRestantes = kmMantenimiento - kmRecorridos.Value;

                    if (kmRecorridos >= kmMantenimiento)
                    {
                        estado = "Mantenimiento vencido";
                        color = "rojo";
                    }
                    else if (kmRestantes <= kmAlerta)
                    {
                        estado = "Próximo a mantenimiento";
                        color = "amarillo";
                    }
                    else
                    {
                        estado = "Correcto";
                        color = "verde";
                    }
                }

                resultado.Add(new
                {
                    unidad.id_Unidad,
                    unidad.Marca,
                    unidad.Serie,

                    ultimoMantenimientoKm = ultimoMantenimiento?.Kilometraje,
                    ultimoMantenimientoFecha = ultimoMantenimiento?.FechaMantenimiento,

                    kilometrajeActual = ultimoKilometraje?.KilometrajeActual,
                    fechaKilometrajeActual = ultimoKilometraje?.FechaRegistro,

                    kmRecorridos,
                    kmRestantes,

                    estado,
                    color
                });
            }

            return Ok(resultado);
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