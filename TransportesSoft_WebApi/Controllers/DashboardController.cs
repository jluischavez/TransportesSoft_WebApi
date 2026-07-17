using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportesSoft_WebApi.Data;

namespace TransportesSoft_WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DashboardController : BaseApiController
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("resumen-mensual")]
        public async Task<IActionResult> ObtenerResumenMensual(
            int? anio = null,
            int? mes = null)
        {
            try
            {
                var empresaId = ObtenerEmpresaId();

                if (empresaId == null)
                    return SinEmpresaAsignada();

                var fechaActual = DateTime.Now;

                var anioConsulta = anio ?? fechaActual.Year;
                var mesConsulta = mes ?? fechaActual.Month;

                if (mesConsulta < 1 || mesConsulta > 12)
                {
                    return BadRequest(new
                    {
                        mensaje = "El mes debe estar entre 1 y 12."
                    });
                }

                var fechaInicio = new DateTime(anioConsulta, mesConsulta, 1);
                var fechaFin = fechaInicio.AddMonths(1);

                /*
                 * INGRESOS DEL MES
                 */
                var viajesMes = await _context.ContViajes
                    .AsNoTracking()
                    .Where(v =>
                        v.EmpresaId == empresaId.Value &&
                        v.FechaViaje >= fechaInicio &&
                        v.FechaViaje < fechaFin)
                    .Select(v => new
                    {
                        v.id_Viaje,
                        v.id_Client,
                        v.NombreCliente,
                        v.id_Unidad,
                        v.Total
                    })
                    .ToListAsync();

                /*
                 * MANTENIMIENTOS DEL MES
                 */
                var mantenimientosMes = await _context.ContMantenimientosCab
                    .AsNoTracking()
                    .Where(m =>
                        m.EmpresaId == empresaId.Value &&
                        m.FechaMantenimiento >= fechaInicio &&
                        m.FechaMantenimiento < fechaFin)
                    .Select(m => new
                    {
                        m.id_Unidad,
                        m.CostoTotal
                    })
                    .ToListAsync();

                /*
                 * DIÉSEL DEL MES
                 */
                var consumosMes = await _context.ContConsumoUnidades
                    .AsNoTracking()
                    .Where(c =>
                        c.EmpresaId == empresaId.Value &&
                        c.Fecha >= fechaInicio &&
                        c.Fecha < fechaFin)
                    .Select(c => new
                    {
                        c.id_Unidad,
                        c.ConsumoPesos
                    })
                    .ToListAsync();

                /*
                 * TOTALES GENERALES
                 */
                var ingresosMes = viajesMes.Sum(v => v.Total);
                var gastosMantenimiento = mantenimientosMes.Sum(m => m.CostoTotal);
                var gastosDiesel = consumosMes.Sum(c => c.ConsumoPesos);

                var gastosMes = gastosMantenimiento + gastosDiesel;
                var utilidadEstimada = ingresosMes - gastosMes;
                var viajesRealizados = viajesMes.Count;

                /*
                 * INGRESOS AGRUPADOS POR UNIDAD
                 */
                var ingresosPorUnidad = viajesMes
                    .GroupBy(v => v.id_Unidad)
                    .ToDictionary(
                        grupo => grupo.Key,
                        grupo => grupo.Sum(v => v.Total)
                    );

                /*
                 * MANTENIMIENTOS AGRUPADOS POR UNIDAD
                 */
                var mantenimientosPorUnidad = mantenimientosMes
                    .GroupBy(m => m.id_Unidad)
                    .ToDictionary(
                        grupo => grupo.Key,
                        grupo => grupo.Sum(m => m.CostoTotal)
                    );

                /*
                 * DIÉSEL AGRUPADO POR UNIDAD
                 */
                var dieselPorUnidad = consumosMes
                    .GroupBy(c => c.id_Unidad)
                    .ToDictionary(
                        grupo => grupo.Key,
                        grupo => grupo.Sum(c => c.ConsumoPesos)
                    );

                /*
                 * IDs DE TODAS LAS UNIDADES QUE TUVIERON MOVIMIENTO
                 */
                var idsUnidades = ingresosPorUnidad.Keys
                    .Union(mantenimientosPorUnidad.Keys)
                    .Union(dieselPorUnidad.Keys)
                    .Distinct()
                    .ToList();

                /*
                 * OBTENER DATOS DEL CATÁLOGO DE UNIDADES
                 */
                var unidadesCatalogo = await _context.ContUnidadesCat
                    .AsNoTracking()
                    .Where(u =>
                        u.EmpresaId == empresaId.Value &&
                        idsUnidades.Contains(u.id_Unidad))
                    .Select(u => new
                    {
                        u.id_Unidad,
                        u.Marca,
                        u.Serie
                    })
                    .ToListAsync();

                var nombresUnidades = unidadesCatalogo.ToDictionary(
                    u => u.id_Unidad,
                    u => $"{u.id_Unidad} — {u.Marca} | {u.Serie}"
                );

                /*
                 * RENTABILIDAD POR UNIDAD
                 */
                var rentabilidadPorUnidad = idsUnidades
                    .Select(idUnidad =>
                    {
                        var ingreso = ingresosPorUnidad.GetValueOrDefault(idUnidad);
                        var mantenimiento = mantenimientosPorUnidad.GetValueOrDefault(idUnidad);
                        var diesel = dieselPorUnidad.GetValueOrDefault(idUnidad);

                        var gasto = mantenimiento + diesel;
                        var utilidad = ingreso - gasto;

                        return new
                        {
                            idUnidad,
                            unidad = nombresUnidades.GetValueOrDefault(
                                idUnidad,
                                $"Unidad {idUnidad}"
                            ),
                            ingreso,
                            gastoMantenimiento = mantenimiento,
                            gastoDiesel = diesel,
                            gastoTotal = gasto,
                            utilidad
                        };
                    })
                    .OrderByDescending(u => u.ingreso)
                    .ToList();

                /*
                 * CLIENTE QUE MÁS FACTURA
                 */
                var clienteMayorFacturacion = viajesMes
                    .GroupBy(v => new
                    {
                        v.id_Client,
                        v.NombreCliente
                    })
                    .Select(grupo => new
                    {
                        idCliente = grupo.Key.id_Client,
                        nombreCliente = grupo.Key.NombreCliente,
                        totalFacturado = grupo.Sum(v => v.Total),
                        viajes = grupo.Count()
                    })
                    .OrderByDescending(c => c.totalFacturado)
                    .FirstOrDefault();

                /*
                 * UNIDADES CON MÁS GASTO
                 */
                var unidadesMayorGasto = rentabilidadPorUnidad
                    .OrderByDescending(u => u.gastoTotal)
                    .Take(5)
                    .ToList();

                return Ok(new
                {
                    periodo = new
                    {
                        anio = anioConsulta,
                        mes = mesConsulta,
                        fechaInicio,
                        fechaFin = fechaFin.AddDays(-1)
                    },

                    resumen = new
                    {
                        ingresosMes,
                        gastosMes,
                        gastosMantenimiento,
                        gastosDiesel,
                        utilidadEstimada,
                        viajesRealizados
                    },

                    clienteMayorFacturacion,
                    rentabilidadPorUnidad,
                    unidadesMayorGasto
                });
            }
            catch (Exception ex)
            {
                // Después conviene mandar ex a logs internos,
                // no regresarlo al frontend.
                Console.WriteLine(ex);

                return StatusCode(500, new
                {
                    mensaje = "No se pudo obtener el resumen del dashboard."
                });
            }
        }
    }
}