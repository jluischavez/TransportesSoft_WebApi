using System.ComponentModel.DataAnnotations;

namespace TransportesSoft_WebApi.Models
{
    public class EmpresasCat
    {
        [Key]
        public int Id { get; set; }
        public required string NombreComercial { get; set; }
        public required string RFC { get; set; }
        public required string ClaveAcceso { get; set; }
        public required string Email { get; set; }
        public required string RazonSocial { get; set; }
        public required string Telefono { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public bool Activo { get; set; } = true;
    }
}