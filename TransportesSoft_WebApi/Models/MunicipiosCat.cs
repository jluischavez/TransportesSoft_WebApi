using System.ComponentModel.DataAnnotations;

namespace TransportesSoft_WebApi.Models
{
    public class MunicipiosCat
    {
        [Key]
        public int IdMunicipio { get; set; }
        public int IdEstado { get; set; }
        public required string Nombre { get; set; }
        public string? ClaveInegi { get; set; }
        public bool Activo { get; set; } = true;
    }
}