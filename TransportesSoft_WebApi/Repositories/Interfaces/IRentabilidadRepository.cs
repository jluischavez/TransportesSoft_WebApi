using TransportesSoft_WebApi.DTOs.Rentabilidad;

namespace TransportesSoft_WebApi.Repositories.Interfaces;

public interface IRentabilidadRepository
{
    Task<List<IngresoUnidadClienteDataDto>> ObtenerIngresosAgrupadosAsync(
        int empresaId,
        DateTime fechaInicio,
        DateTime fechaFinExclusiva,
        CancellationToken cancellationToken);

    Task<List<GastoUnidadDataDto>> ObtenerGastosUnidadesAgrupadosAsync(
        int empresaId,
        DateTime fechaInicio,
        DateTime fechaFinExclusiva,
        int? idUnidad,
        bool incluirDiesel,
        bool incluirMantenimientos,
        CancellationToken cancellationToken);

    Task<List<GastoRemolqueDataDto>> ObtenerMantenimientosRemolquesAgrupadosAsync(
        int empresaId,
        DateTime fechaInicio,
        DateTime fechaFinExclusiva,
        bool incluirMantenimientos,
        CancellationToken cancellationToken);

    Task<Dictionary<int, string>> ObtenerDescripcionesUnidadesAsync(
        int empresaId,
        IReadOnlyCollection<int> idsUnidades,
        CancellationToken cancellationToken);

    Task<Dictionary<int, string>> ObtenerDescripcionesRemolquesAsync(
        int empresaId,
        IReadOnlyCollection<int> idsRemolques,
        CancellationToken cancellationToken);

    Task<string?> ObtenerDescripcionUnidadAsync(
        int empresaId,
        int idUnidad,
        CancellationToken cancellationToken);

    Task<string?> ObtenerNombreClienteAsync(
        int empresaId,
        int idCliente,
        CancellationToken cancellationToken);

    Task<bool> ExisteUnidadAsync(
        int empresaId,
        int idUnidad,
        CancellationToken cancellationToken);

    Task<bool> ExisteClienteAsync(
        int empresaId,
        int idCliente,
        CancellationToken cancellationToken);
}
