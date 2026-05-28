using System.ComponentModel.DataAnnotations;

namespace TransportesSoft_WebApi.Models
{
    public class RolesCat
    {
        [Key]
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
    }
}