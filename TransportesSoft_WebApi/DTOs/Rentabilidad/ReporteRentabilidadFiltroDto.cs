namespace TransportesSoft_WebApi.DTOs.Rentabilidad;

public sealed class ReporteRentabilidadFiltroDto
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int? IdUnidad { get; set; }
    public int? IdCliente { get; set; }
    public bool IncluirDiesel { get; set; } = true;
    public bool IncluirMantenimientos { get; set; } = true;
}
