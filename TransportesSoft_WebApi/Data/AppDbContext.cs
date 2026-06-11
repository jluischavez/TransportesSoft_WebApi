using Microsoft.EntityFrameworkCore;
using TransportesSoft_WebApi.Models;

namespace TransportesSoft_WebApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ConfSucursalLocal> ConfSucursalLocal { get; set; }
        public DbSet<ContClientesCat> ContClientesCat { get; set; }
        public DbSet<ContConsumoUnidades> ContConsumoUnidades { get; set; }
        public DbSet<ContKilometrajeUnidad> ContKilometrajeUnidad { get; set; }
        public DbSet<ContMantenimientosCab> ContMantenimientosCab { get; set; }
        public DbSet<ContMantenimientosDet> ContMantenimientosDet { get; set; }
        public DbSet<ContOperadoresCat> ContOperadoresCat { get; set; }
        public DbSet<ContPolizasReg> ContPolizasReg { get; set; }
        public DbSet<ContPreciosDiesel> ContPreciosDiesel { get; set; }
        public DbSet<ContRemolquesCat> ContRemolquesCat { get; set; }
        public DbSet<ContTiposPolizas> ContTiposPolizas { get; set; }
        public DbSet<ContUnidadesCat> ContUnidadesCat { get; set; }
        public DbSet<ContViajes> ContViajes { get; set; }
        public DbSet<EmpresasCat> EmpresasCat { get; set; }
        public DbSet<EstadosCat> EstadosCat { get; set; }
        public DbSet<MunicipiosCat> MunicipiosCat { get; set; }
        public DbSet<RolesCat> RolesCat { get; set; }
        public DbSet<UsuarioRoles> UsuarioRoles { get; set; }
        public DbSet<UsuariosCat> UsuariosCat { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // llave compuesta para UsuarioRoles
            modelBuilder.Entity<UsuarioRoles>()
                .HasKey(ur => new { ur.UsuarioId, ur.RolId });

            // llave compuesta para ContMantenimientosDet
            modelBuilder.Entity<ContMantenimientosDet>()
                .HasKey(d => new { d.IdMantenimiento, d.Renglon });

            // ContConsumoUnidades no tiene PK definida en la BD
            modelBuilder.Entity<ContConsumoUnidades>();
        }
    }
}