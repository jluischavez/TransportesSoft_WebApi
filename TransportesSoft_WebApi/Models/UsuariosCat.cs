using System.ComponentModel.DataAnnotations;

namespace TransportesSoft_WebApi.Models
{
    public class UsuariosCat
    {
        [Key]
        public int Id { get; set; }
        public required string NombreUsuario { get; set; }
        public required string ContrasenaHash { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }
    }
}
