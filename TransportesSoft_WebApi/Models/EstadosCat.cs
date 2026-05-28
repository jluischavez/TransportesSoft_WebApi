using System.ComponentModel.DataAnnotations;

namespace TransportesSoft_WebApi.Models
{
    public class EstadosCat
    {
        [Key]
        public int IdEstado { get; set; }
        public required string Nombre { get; set; }
        public string? Clave { get; set; }
    }
}