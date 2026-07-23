using TransportesSoft_WebApi.DTOs.Rentabilidad;

namespace TransportesSoft_WebApi.Services.Interfaces;

public interface IRentabilidadService
{
    Task<ReporteRentabilidadDto> ObtenerReporteAsync(
        int empresaId,
        ReporteRentabilidadFiltroDto filtro,
        CancellationToken cancellationToken);

    Task<DashboardResumenDto> ObtenerDashboardMensualAsync(
        int empresaId,
        int? anio,
        int? mes,
        CancellationToken cancellationToken);
}
