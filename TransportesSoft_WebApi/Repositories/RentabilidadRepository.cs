using Microsoft.EntityFrameworkCore;
using TransportesSoft_WebApi.Data;
using TransportesSoft_WebApi.DTOs.Rentabilidad;
using TransportesSoft_WebApi.Repositories.Interfaces;

namespace TransportesSoft_WebApi.Repositories;

public sealed class RentabilidadRepository : IRentabilidadRepository
{
    private readonly AppDbContext _context;

    public RentabilidadRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<IngresoUnidadClienteDataDto>> ObtenerIngresosAgrupadosAsync(
        int empresaId,
        DateTime fechaInicio,
        DateTime fechaFinExclusiva,
        CancellationToken cancellationToken)
    {
        return _context.ContViajes
            .AsNoTracking()
            .Where(v =>
                v.EmpresaId == empresaId &&
                v.FechaViaje >= fechaInicio &&
                v.FechaViaje < fechaFinExclusiva &&
                v.id_Unidad > 0)
            .GroupBy(v => new
            {
                IdUnidad = v.id_Unidad,
                IdRemolque = v.id_Remolque,
                IdCliente = v.id_Client,
                v.NombreCliente
            })
            .Select(grupo => new IngresoUnidadClienteDataDto
            {
                IdUnidad = grupo.Key.IdUnidad,
                IdRemolque = grupo.Key.IdRemolque,
                IdCliente = grupo.Key.IdCliente,
                NombreCliente = grupo.Key.NombreCliente,
                Ingreso = grupo.Sum(v => v.Total),
                Viajes = grupo.Count()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<GastoUnidadDataDto>> ObtenerGastosUnidadesAgrupadosAsync(
        int empresaId,
        DateTime fechaInicio,
        DateTime fechaFinExclusiva,
        int? idUnidad,
        bool incluirDiesel,
        bool incluirMantenimientos,
        CancellationToken cancellationToken)
    {
        var gastosDiesel = new List<GastoUnidadDataDto>();
        var gastosMantenimiento = new List<GastoUnidadDataDto>();

        if (incluirDiesel)
        {
            var dieselQuery = _context.ContConsumoUnidades
                .AsNoTracking()
                .Where(c =>
                    c.EmpresaId == empresaId &&
                    c.Fecha >= fechaInicio &&
                    c.Fecha < fechaFinExclusiva &&
                    c.id_Unidad > 0);

            if (idUnidad.HasValue)
            {
                dieselQuery = dieselQuery.Where(c => c.id_Unidad == idUnidad.Value);
            }

            gastosDiesel = await dieselQuery
                .GroupBy(c => c.id_Unidad)
                .Select(grupo => new GastoUnidadDataDto
                {
                    IdUnidad = grupo.Key,
                    GastoDiesel = grupo.Sum(c => c.ConsumoPesos)
                })
                .ToListAsync(cancellationToken);
        }

        if (incluirMantenimientos)
        {
            var mantenimientoQuery = _context.ContMantenimientosCab
                .AsNoTracking()
                .Where(m =>
                    m.EmpresaId == empresaId &&
                    m.FechaMantenimiento >= fechaInicio &&
                    m.FechaMantenimiento < fechaFinExclusiva &&
                    m.id_Unidad > 0 &&
                    m.id_Remolque == 0);

            if (idUnidad.HasValue)
            {
                mantenimientoQuery = mantenimientoQuery.Where(m => m.id_Unidad == idUnidad.Value);
            }

            gastosMantenimiento = await mantenimientoQuery
                .GroupBy(m => m.id_Unidad)
                .Select(grupo => new GastoUnidadDataDto
                {
                    IdUnidad = grupo.Key,
                    GastoMantenimiento = grupo.Sum(m => m.CostoTotal)
                })
                .ToListAsync(cancellationToken);
        }

        return gastosDiesel
            .Concat(gastosMantenimiento)
            .GroupBy(g => g.IdUnidad)
            .Select(grupo => new GastoUnidadDataDto
            {
                IdUnidad = grupo.Key,
                GastoDiesel = grupo.Sum(g => g.GastoDiesel),
                GastoMantenimiento = grupo.Sum(g => g.GastoMantenimiento)
            })
            .ToList();
    }

    public Task<List<GastoRemolqueDataDto>> ObtenerMantenimientosRemolquesAgrupadosAsync(
        int empresaId,
        DateTime fechaInicio,
        DateTime fechaFinExclusiva,
        bool incluirMantenimientos,
        CancellationToken cancellationToken)
    {
        if (!incluirMantenimientos)
        {
            return Task.FromResult(new List<GastoRemolqueDataDto>());
        }

        return _context.ContMantenimientosCab
            .AsNoTracking()
            .Where(m =>
                m.EmpresaId == empresaId &&
                m.FechaMantenimiento >= fechaInicio &&
                m.FechaMantenimiento < fechaFinExclusiva &&
                m.id_Remolque > 0 &&
                m.id_Unidad == 0)
            .GroupBy(m => m.id_Remolque)
            .Select(grupo => new GastoRemolqueDataDto
            {
                IdRemolque = grupo.Key,
                GastoMantenimiento = grupo.Sum(m => m.CostoTotal)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<int, string>> ObtenerDescripcionesUnidadesAsync(
        int empresaId,
        IReadOnlyCollection<int> idsUnidades,
        CancellationToken cancellationToken)
    {
        if (idsUnidades.Count == 0)
        {
            return new Dictionary<int, string>();
        }

        var ids = idsUnidades
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        return await _context.ContUnidadesCat
            .AsNoTracking()
            .Where(u =>
                u.EmpresaId == empresaId &&
                ids.Contains(u.id_Unidad))
            .ToDictionaryAsync(
                u => u.id_Unidad,
                u => $"{u.id_Unidad} - {u.Marca} | {u.Serie}",
                cancellationToken);
    }

    public async Task<Dictionary<int, string>> ObtenerDescripcionesRemolquesAsync(
        int empresaId,
        IReadOnlyCollection<int> idsRemolques,
        CancellationToken cancellationToken)
    {
        if (idsRemolques.Count == 0)
        {
            return new Dictionary<int, string>();
        }

        var ids = idsRemolques
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        return await _context.ContRemolquesCat
            .AsNoTracking()
            .Where(r =>
                r.EmpresaId == empresaId &&
                ids.Contains(r.id_Remolque))
            .ToDictionaryAsync(
                r => r.id_Remolque,
                r => $"{r.id_Remolque} - {r.Marca} | {r.Modelo}",
                cancellationToken);
    }

    public Task<string?> ObtenerDescripcionUnidadAsync(
        int empresaId,
        int idUnidad,
        CancellationToken cancellationToken)
    {
        return _context.ContUnidadesCat
            .AsNoTracking()
            .Where(u =>
                u.EmpresaId == empresaId &&
                u.id_Unidad == idUnidad)
            .Select(u => $"{u.id_Unidad} - {u.Marca} | {u.Serie}")
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<string?> ObtenerNombreClienteAsync(
        int empresaId,
        int idCliente,
        CancellationToken cancellationToken)
    {
        return _context.ContClientesCat
            .AsNoTracking()
            .Where(c =>
                c.EmpresaId == empresaId &&
                c.id_Client == idCliente)
            .Select(c => c.Nombre)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> ExisteUnidadAsync(
        int empresaId,
        int idUnidad,
        CancellationToken cancellationToken)
    {
        return _context.ContUnidadesCat
            .AsNoTracking()
            .AnyAsync(u =>
                u.EmpresaId == empresaId &&
                u.id_Unidad == idUnidad,
                cancellationToken);
    }

    public Task<bool> ExisteClienteAsync(
        int empresaId,
        int idCliente,
        CancellationToken cancellationToken)
    {
        return _context.ContClientesCat
            .AsNoTracking()
            .AnyAsync(c =>
                c.EmpresaId == empresaId &&
                c.id_Client == idCliente,
                cancellationToken);
    }
}
