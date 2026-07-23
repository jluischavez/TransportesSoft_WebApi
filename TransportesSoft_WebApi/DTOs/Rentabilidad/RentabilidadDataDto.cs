namespace TransportesSoft_WebApi.DTOs.Rentabilidad;

public sealed class IngresoUnidadClienteDataDto
{
    public int IdUnidad { get; set; }
    public int IdRemolque { get; set; }
    public int IdCliente { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public decimal Ingreso { get; set; }
    public int Viajes { get; set; }
}

public sealed class GastoUnidadDataDto
{
    public int IdUnidad { get; set; }
    public decimal GastoDiesel { get; set; }
    public decimal GastoMantenimiento { get; set; }

    public decimal GastoTotal => GastoDiesel + GastoMantenimiento;
}

public sealed class GastoRemolqueDataDto
{
    public int IdRemolque { get; set; }
    public decimal GastoMantenimiento { get; set; }
}
