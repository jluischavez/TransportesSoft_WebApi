namespace TransportesSoft_WebApi.DTOs.Rentabilidad;

public sealed class DashboardResumenDto
{
    public PeriodoDashboardDto Periodo { get; set; } = new();
    public ResumenDashboardDto Resumen { get; set; } = new();
    public ClienteFacturacionDto? ClienteMayorFacturacion { get; set; }
    public List<UnidadRentabilidadDto> RentabilidadPorUnidad { get; set; } = [];
    public List<UnidadRentabilidadDto> UnidadesMayorGasto { get; set; } = [];
}

public sealed class PeriodoDashboardDto
{
    public int Anio { get; set; }
    public int Mes { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
}

public sealed class ResumenDashboardDto
{
    public decimal IngresosMes { get; set; }
    public decimal GastosMes { get; set; }
    public decimal GastosMantenimiento { get; set; }
    public decimal GastosDiesel { get; set; }
    public decimal UtilidadEstimada { get; set; }
    public int ViajesRealizados { get; set; }
}
