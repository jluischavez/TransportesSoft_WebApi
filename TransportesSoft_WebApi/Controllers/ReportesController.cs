using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportesSoft_WebApi.DTOs.Rentabilidad;
using TransportesSoft_WebApi.Services.Interfaces;

namespace TransportesSoft_WebApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public sealed class ReportesController : BaseApiController
{
    private readonly IRentabilidadService _rentabilidadService;
    private readonly ILogger<ReportesController> _logger;

    public ReportesController(
        IRentabilidadService rentabilidadService,
        ILogger<ReportesController> logger)
    {
        _rentabilidadService = rentabilidadService;
        _logger = logger;
    }

    [HttpGet("rentabilidad-operativa")]
    public async Task<IActionResult> ObtenerRentabilidadOperativa(
        [FromQuery] ReporteRentabilidadFiltroDto filtro,
        CancellationToken cancellationToken)
    {
        var empresaId = ObtenerEmpresaId();

        if (empresaId == null)
        {
            return SinEmpresaAsignada();
        }

        try
        {
            var reporte = await _rentabilidadService.ObtenerReporteAsync(
                empresaId.Value,
                filtro,
                cancellationToken);

            return Ok(reporte);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al generar el reporte de rentabilidad para la empresa {EmpresaId}",
                empresaId.Value);

            return StatusCode(500, new
            {
                mensaje = "No se pudo generar el reporte de rentabilidad."
            });
        }
    }
}
