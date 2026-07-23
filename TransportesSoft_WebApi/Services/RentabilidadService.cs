using TransportesSoft_WebApi.DTOs.Rentabilidad;
using TransportesSoft_WebApi.Repositories.Interfaces;
using TransportesSoft_WebApi.Services.Interfaces;

namespace TransportesSoft_WebApi.Services;

public sealed class RentabilidadService : IRentabilidadService
{
    private readonly IRentabilidadRepository _repository;

    public RentabilidadService(IRentabilidadRepository repository)
    {
        _repository = repository;
    }

    public async Task<ReporteRentabilidadDto> ObtenerReporteAsync(
        int empresaId,
        ReporteRentabilidadFiltroDto filtro,
        CancellationToken cancellationToken)
    {
        await ValidarFiltroAsync(empresaId, filtro, cancellationToken);

        var fechaInicio = filtro.FechaInicio.Date;
        var fechaFin = filtro.FechaFin.Date;
        var fechaFinExclusiva = fechaFin.AddDays(1);

        // Se obtienen los ingresos agrupados por unidad, remolque y cliente.
        // Esto permite prorratear los gastos cuando se aplican filtros.
        var ingresosBase = await _repository.ObtenerIngresosAgrupadosAsync(
            empresaId,
            fechaInicio,
            fechaFinExclusiva,
            cancellationToken);

        var ingresosSeleccionados = ingresosBase
            .Where(i => !filtro.IdUnidad.HasValue || i.IdUnidad == filtro.IdUnidad.Value)
            .Where(i => !filtro.IdCliente.HasValue || i.IdCliente == filtro.IdCliente.Value)
            .ToList();

        var gastosUnidadesBase = await _repository.ObtenerGastosUnidadesAgrupadosAsync(
            empresaId,
            fechaInicio,
            fechaFinExclusiva,
            filtro.IdUnidad,
            filtro.IncluirDiesel,
            filtro.IncluirMantenimientos,
            cancellationToken);

        var gastosRemolquesBase = await _repository.ObtenerMantenimientosRemolquesAgrupadosAsync(
            empresaId,
            fechaInicio,
            fechaFinExclusiva,
            filtro.IncluirMantenimientos,
            cancellationToken);

        var ingresosTotalesPorUnidad = ingresosBase
            .GroupBy(i => i.IdUnidad)
            .ToDictionary(
                grupo => grupo.Key,
                grupo => grupo.Sum(i => i.Ingreso));

        var ingresosSeleccionadosPorUnidad = ingresosSeleccionados
            .GroupBy(i => i.IdUnidad)
            .ToDictionary(
                grupo => grupo.Key,
                grupo => new
                {
                    Ingreso = grupo.Sum(i => i.Ingreso),
                    Viajes = grupo.Sum(i => i.Viajes)
                });

        var gastosDirectosPorUnidad = gastosUnidadesBase
            .Where(g => g.IdUnidad > 0)
            .ToDictionary(g => g.IdUnidad);

        var idsRemolques = gastosRemolquesBase
            .Where(g => g.IdRemolque > 0)
            .Select(g => g.IdRemolque)
            .Distinct()
            .ToList();

        var descripcionesRemolques = await _repository.ObtenerDescripcionesRemolquesAsync(
            empresaId,
            idsRemolques,
            cancellationToken);

        var mantenimientoRemolquePorUnidad = new Dictionary<int, decimal>();
        var mantenimientosPorRemolque = new List<RemolqueMantenimientoDto>();
        var mantenimientoRemolquesSinViajes = 0m;

        foreach (var gastoRemolque in gastosRemolquesBase.Where(g => g.IdRemolque > 0))
        {
            var ingresosTotalesRemolque = ingresosBase
                .Where(i => i.IdRemolque == gastoRemolque.IdRemolque)
                .Sum(i => i.Ingreso);

            var ingresosSeleccionadosRemolque = ingresosSeleccionados
                .Where(i => i.IdRemolque == gastoRemolque.IdRemolque)
                .Sum(i => i.Ingreso);

            decimal gastoAplicable;

            /*
             * Sin filtros de unidad o cliente se toma todo el mantenimiento
             * del remolque registrado dentro del periodo.
             */
            if (!filtro.IdUnidad.HasValue && !filtro.IdCliente.HasValue)
            {
                gastoAplicable = gastoRemolque.GastoMantenimiento;
            }
            else
            {
                /*
                 * Cuando hay filtros, solamente se toma la proporción
                 * correspondiente a los ingresos seleccionados.
                 */
                var factorParticipacion = ingresosTotalesRemolque == 0m
                    ? 0m
                    : ingresosSeleccionadosRemolque / ingresosTotalesRemolque;

                gastoAplicable = RedondearMoneda(
                    gastoRemolque.GastoMantenimiento * factorParticipacion);
            }

            if (gastoAplicable <= 0m)
            {
                continue;
            }

            /*
             * Se conserva como información independiente del remolque.
             * No se distribuye ni se registra dentro de ninguna unidad.
             */
            mantenimientosPorRemolque.Add(new RemolqueMantenimientoDto
            {
                IdRemolque = gastoRemolque.IdRemolque,
                Remolque = descripcionesRemolques.GetValueOrDefault(
                    gastoRemolque.IdRemolque,
                    $"Remolque {gastoRemolque.IdRemolque}"),
                GastoMantenimiento = gastoAplicable
            });

            /*
             * Sirve solamente para informar que el remolque tuvo gasto,
             * pero no tuvo viajes relacionados en el periodo.
             */
            if (ingresosTotalesRemolque == 0m)
            {
                mantenimientoRemolquesSinViajes += gastoAplicable;
            }
        }

        mantenimientosPorRemolque = mantenimientosPorRemolque
            .OrderByDescending(r => r.GastoMantenimiento)
            .ToList();


        var idsUnidades = ingresosSeleccionadosPorUnidad.Keys
            .Union(gastosDirectosPorUnidad.Keys)
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        var descripcionesUnidades = await _repository.ObtenerDescripcionesUnidadesAsync(
            empresaId,
            idsUnidades,
            cancellationToken);

        var rentabilidadPorUnidad = new List<UnidadRentabilidadDto>();

        foreach (var idUnidad in idsUnidades)
        {
            var ingreso = ingresosSeleccionadosPorUnidad.TryGetValue(idUnidad, out var ingresoUnidad)
                ? ingresoUnidad.Ingreso
                : 0m;

            var viajes = ingresoUnidad?.Viajes ?? 0;

            gastosDirectosPorUnidad.TryGetValue(idUnidad, out var gastoUnidad);

            var gastoDiesel = gastoUnidad?.GastoDiesel ?? 0m;
            var gastoMantenimientoUnidad = gastoUnidad?.GastoMantenimiento ?? 0m;

            if (filtro.IdCliente.HasValue)
            {
                var ingresoTotalUnidad = ingresosTotalesPorUnidad.GetValueOrDefault(idUnidad);
                var factorParticipacion = ingresoTotalUnidad == 0m
                    ? 0m
                    : ingreso / ingresoTotalUnidad;

                gastoDiesel = RedondearMoneda(gastoDiesel * factorParticipacion);
                gastoMantenimientoUnidad = RedondearMoneda(
                    gastoMantenimientoUnidad * factorParticipacion);
            }

             /*
             * En la rentabilidad por unidad solamente entran
             * los gastos directamente pertenecientes a la unidad.
             *
             * Los mantenimientos de remolques se muestran
             * en una sección independiente.
             */
            var gastoMantenimiento = gastoMantenimientoUnidad;
            var gastoTotal = gastoDiesel + gastoMantenimiento;
            var utilidad = ingreso - gastoTotal;

            rentabilidadPorUnidad.Add(new UnidadRentabilidadDto
            {
                IdUnidad = idUnidad,

                Unidad = descripcionesUnidades.GetValueOrDefault(
                idUnidad,
                $"Unidad {idUnidad}"),

                Viajes = viajes,
                Ingreso = ingreso,

                GastoDiesel = gastoDiesel,

                GastoMantenimientoUnidad =
                gastoMantenimientoUnidad,

                GastoMantenimientoRemolques = 0m,

                GastoMantenimiento =
                gastoMantenimientoUnidad,

                GastoTotal = gastoTotal,
                Utilidad = utilidad,

                MargenUtilidadPorcentaje =
                CalcularPorcentaje(utilidad, ingreso)
                    });
        }

        rentabilidadPorUnidad = rentabilidadPorUnidad
        .Where(u => u.IdUnidad > 0)
        .OrderByDescending(u => u.Ingreso)
        .ThenByDescending(u => u.Utilidad)
        .ToList();

        var ingresos = ingresosSeleccionados.Sum(i => i.Ingreso);
        var gastosDiesel = rentabilidadPorUnidad.Sum(u => u.GastoDiesel);
        var gastosMantenimientoUnidades = rentabilidadPorUnidad.Sum(
            u => u.GastoMantenimientoUnidad);
        var gastosMantenimientoRemolques = mantenimientosPorRemolque.Sum(
            r => r.GastoMantenimiento);
        var gastosMantenimiento =
            gastosMantenimientoUnidades + gastosMantenimientoRemolques;
        var gastosTotales = gastosDiesel + gastosMantenimiento;
        var utilidadEstimada = ingresos - gastosTotales;
        var viajesRealizados = ingresosSeleccionados.Sum(i => i.Viajes);

        var facturacionPorCliente = ingresosSeleccionados
            .GroupBy(i => new
            {
                i.IdCliente,
                i.NombreCliente
            })
            .Select(grupo =>
            {
                var totalCliente = grupo.Sum(i => i.Ingreso);

                return new ClienteFacturacionDto
                {
                    IdCliente = grupo.Key.IdCliente,
                    NombreCliente = grupo.Key.NombreCliente,
                    Viajes = grupo.Sum(i => i.Viajes),
                    TotalFacturado = totalCliente,
                    ParticipacionPorcentaje = CalcularPorcentaje(totalCliente, ingresos)
                };
            })
            .OrderByDescending(c => c.TotalFacturado)
            .ToList();

        var nombreUnidadFiltro = "Todas";
        var nombreClienteFiltro = "Todos";

        if (filtro.IdUnidad.HasValue)
        {
            nombreUnidadFiltro = await _repository.ObtenerDescripcionUnidadAsync(
                empresaId,
                filtro.IdUnidad.Value,
                cancellationToken) ?? $"Unidad {filtro.IdUnidad.Value}";
        }

        if (filtro.IdCliente.HasValue)
        {
            nombreClienteFiltro = await _repository.ObtenerNombreClienteAsync(
                empresaId,
                filtro.IdCliente.Value,
                cancellationToken) ?? $"Cliente {filtro.IdCliente.Value}";
        }

        return new ReporteRentabilidadDto
        {
            Periodo = new PeriodoRentabilidadDto
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            },
            Filtros = new FiltrosRentabilidadDto
            {
                IdUnidad = filtro.IdUnidad,
                Unidad = nombreUnidadFiltro,
                IdCliente = filtro.IdCliente,
                Cliente = nombreClienteFiltro,
                IncluirDiesel = filtro.IncluirDiesel,
                IncluirMantenimientos = filtro.IncluirMantenimientos
            },
            Resumen = new ResumenRentabilidadDto
            {
                Ingresos = ingresos,
                GastosDiesel = gastosDiesel,
                GastosMantenimientoUnidades = gastosMantenimientoUnidades,
                GastosMantenimientoRemolques = gastosMantenimientoRemolques,
                GastosMantenimientoRemolquesNoAsignados = mantenimientoRemolquesSinViajes,
                GastosMantenimiento = gastosMantenimiento,
                GastosTotales = gastosTotales,
                UtilidadEstimada = utilidadEstimada,
                MargenUtilidadPorcentaje = CalcularPorcentaje(utilidadEstimada, ingresos),
                ViajesRealizados = viajesRealizados,
                UnidadesConMovimiento = rentabilidadPorUnidad.Count,
                ClientesAtendidos = facturacionPorCliente.Count,
                IngresoPromedioPorViaje = viajesRealizados == 0
                    ? 0m
                    : RedondearMoneda(ingresos / viajesRealizados),
                UtilidadPromedioPorViaje = viajesRealizados == 0
                    ? 0m
                    : RedondearMoneda(utilidadEstimada / viajesRealizados)
            },
            ClienteMayorFacturacion = facturacionPorCliente.FirstOrDefault(),
            UnidadMayorGasto = rentabilidadPorUnidad
                .Where(u => u.IdUnidad > 0)
                .OrderByDescending(u => u.GastoTotal)
                .FirstOrDefault(),
            RemolqueMayorGasto = mantenimientosPorRemolque.FirstOrDefault(),
            RentabilidadPorUnidad = rentabilidadPorUnidad,
            MantenimientosPorRemolque = mantenimientosPorRemolque,
            FacturacionPorCliente = facturacionPorCliente,
            NotaCalculoGastos = ConstruirNotaCalculo(
                filtro,
                mantenimientoRemolquesSinViajes)
        };
    }

    public async Task<DashboardResumenDto> ObtenerDashboardMensualAsync(
        int empresaId,
        int? anio,
        int? mes,
        CancellationToken cancellationToken)
    {
        var fechaActual = DateTime.Now;
        var anioConsulta = anio ?? fechaActual.Year;
        var mesConsulta = mes ?? fechaActual.Month;

        if (mesConsulta is < 1 or > 12)
        {
            throw new ArgumentException("El mes debe estar entre 1 y 12.");
        }

        var fechaInicio = new DateTime(anioConsulta, mesConsulta, 1);
        var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

        var reporte = await ObtenerReporteAsync(
            empresaId,
            new ReporteRentabilidadFiltroDto
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                IncluirDiesel = true,
                IncluirMantenimientos = true
            },
            cancellationToken);

        return new DashboardResumenDto
        {
            Periodo = new PeriodoDashboardDto
            {
                Anio = anioConsulta,
                Mes = mesConsulta,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            },
            Resumen = new ResumenDashboardDto
            {
                IngresosMes = reporte.Resumen.Ingresos,
                GastosMes = reporte.Resumen.GastosTotales,
                GastosMantenimiento = reporte.Resumen.GastosMantenimiento,
                GastosDiesel = reporte.Resumen.GastosDiesel,
                UtilidadEstimada = reporte.Resumen.UtilidadEstimada,
                ViajesRealizados = reporte.Resumen.ViajesRealizados
            },
            ClienteMayorFacturacion = reporte.ClienteMayorFacturacion,
            RentabilidadPorUnidad = reporte.RentabilidadPorUnidad,
            UnidadesMayorGasto = reporte.RentabilidadPorUnidad
                .Where(u => u.IdUnidad > 0)
                .OrderByDescending(u => u.GastoTotal)
                .Take(5)
                .ToList()
        };
    }

    private async Task ValidarFiltroAsync(
        int empresaId,
        ReporteRentabilidadFiltroDto filtro,
        CancellationToken cancellationToken)
    {
        if (filtro.FechaInicio == default || filtro.FechaFin == default)
        {
            throw new ArgumentException("La fecha inicial y la fecha final son obligatorias.");
        }

        if (filtro.FechaInicio.Date > filtro.FechaFin.Date)
        {
            throw new ArgumentException("La fecha inicial no puede ser mayor que la fecha final.");
        }

        if ((filtro.FechaFin.Date - filtro.FechaInicio.Date).TotalDays > 730)
        {
            throw new ArgumentException("El periodo máximo permitido es de dos años.");
        }

        if (!filtro.IncluirDiesel && !filtro.IncluirMantenimientos)
        {
            throw new ArgumentException("Selecciona al menos un tipo de gasto.");
        }

        if (filtro.IdUnidad.HasValue &&
            !await _repository.ExisteUnidadAsync(
                empresaId,
                filtro.IdUnidad.Value,
                cancellationToken))
        {
            throw new ArgumentException("La unidad seleccionada no pertenece a la empresa.");
        }

        if (filtro.IdCliente.HasValue &&
            !await _repository.ExisteClienteAsync(
                empresaId,
                filtro.IdCliente.Value,
                cancellationToken))
        {
            throw new ArgumentException("El cliente seleccionado no pertenece a la empresa.");
        }
    }

    private static string ConstruirNotaCalculo(
    ReporteRentabilidadFiltroDto filtro,
    decimal mantenimientoRemolquesSinViajes)
    {
        var nota =
            "Los gastos de unidades y remolques se presentan por separado. " +
            "La rentabilidad por unidad considera únicamente los ingresos, " +
            "el diésel y los mantenimientos directamente registrados para esa unidad. " +
            "Los mantenimientos de remolques se muestran en una sección independiente " +
            "y se incluyen solamente en los gastos y la utilidad general del reporte.";

        if (filtro.IdUnidad.HasValue || filtro.IdCliente.HasValue)
        {
            nota +=
                " Cuando se aplica un filtro por unidad o cliente, " +
                "los gastos se calculan según la participación de los ingresos " +
                "correspondientes al filtro seleccionado.";
        }

        if (mantenimientoRemolquesSinViajes > 0m)
        {
            nota +=
                $" Existen {mantenimientoRemolquesSinViajes:C2} de mantenimiento " +
                "correspondiente a remolques que no tuvieron viajes registrados " +
                "dentro del periodo.";
        }

        return nota;
    }

    private static decimal CalcularPorcentaje(decimal valor, decimal baseCalculo)
    {
        if (baseCalculo == 0m)
        {
            return 0m;
        }

        return Math.Round(valor / baseCalculo * 100m, 2);
    }

    private static decimal RedondearMoneda(decimal valor)
    {
        return Math.Round(valor, 2, MidpointRounding.AwayFromZero);
    }
}
