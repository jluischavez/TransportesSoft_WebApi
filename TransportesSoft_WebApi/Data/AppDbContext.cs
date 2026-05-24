using Microsoft.EntityFrameworkCore;
using TransportesSoft_WebApi.Models;

namespace TransportesSoft_WebApi.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {}
        public DbSet<ContClientesCat> ContClientesCat { get; set; }
        public DbSet<UsuariosCat> UsuariosCat { get; set; }
        public DbSet<EmpresasCat> EmpresasCat { get; set; }
    }
}
