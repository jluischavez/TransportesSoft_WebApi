namespace TransportesSoft_WebApi.DTOs.Rentabilidad;

public sealed class ReporteRentabilidadDto
{
    public PeriodoRentabilidadDto Periodo { get; set; } = new();
    public FiltrosRentabilidadDto Filtros { get; set; } = new();
    public ResumenRentabilidadDto Resumen { get; set; } = new();
    public ClienteFacturacionDto? ClienteMayorFacturacion { get; set; }
    public UnidadRentabilidadDto? UnidadMayorGasto { get; set; }
    public RemolqueMantenimientoDto? RemolqueMayorGasto { get; set; }
    public List<UnidadRentabilidadDto> RentabilidadPorUnidad { get; set; } = [];
    public List<RemolqueMantenimientoDto> MantenimientosPorRemolque { get; set; } = [];
    public List<ClienteFacturacionDto> FacturacionPorCliente { get; set; } = [];
    public string NotaCalculoGastos { get; set; } = string.Empty;
}

public sealed class PeriodoRentabilidadDto
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
}

public sealed class FiltrosRentabilidadDto
{
    public int? IdUnidad { get; set; }
    public string Unidad { get; set; } = "Todas";
    public int? IdCliente { get; set; }
    public string Cliente { get; set; } = "Todos";
    public bool IncluirDiesel { get; set; }
    public bool IncluirMantenimientos { get; set; }
}

public sealed class ResumenRentabilidadDto
{
    public decimal Ingresos { get; set; }
    public decimal GastosDiesel { get; set; }
    public decimal GastosMantenimientoUnidades { get; set; }
    public decimal GastosMantenimientoRemolques { get; set; }
    public decimal GastosMantenimientoRemolquesNoAsignados { get; set; }
    public decimal GastosMantenimiento { get; set; }
    public decimal GastosTotales { get; set; }
    public decimal UtilidadEstimada { get; set; }
    public decimal MargenUtilidadPorcentaje { get; set; }
    public int ViajesRealizados { get; set; }
    public int UnidadesConMovimiento { get; set; }
    public int ClientesAtendidos { get; set; }
    public decimal IngresoPromedioPorViaje { get; set; }
    public decimal UtilidadPromedioPorViaje { get; set; }
}

public sealed class UnidadRentabilidadDto
{
    public int IdUnidad { get; set; }
    public string Unidad { get; set; } = string.Empty;
    public int Viajes { get; set; }
    public decimal Ingreso { get; set; }
    public decimal GastoDiesel { get; set; }
    public decimal GastoMantenimientoUnidad { get; set; }
    public decimal GastoMantenimientoRemolques { get; set; }
    public decimal GastoMantenimiento { get; set; }
    public decimal GastoTotal { get; set; }
    public decimal Utilidad { get; set; }
    public decimal MargenUtilidadPorcentaje { get; set; }
}

public sealed class RemolqueMantenimientoDto
{
    public int IdRemolque { get; set; }
    public string Remolque { get; set; } = string.Empty;
    public decimal GastoMantenimiento { get; set; }
}

public sealed class ClienteFacturacionDto
{
    public int IdCliente { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public int Viajes { get; set; }
    public decimal TotalFacturado { get; set; }
    public decimal ParticipacionPorcentaje { get; set; }
}
