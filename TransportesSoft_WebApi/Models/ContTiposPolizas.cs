using System.ComponentModel.DataAnnotations;

namespace TransportesSoft_WebApi.Models
{
    public class ContTiposPolizas
    {
        [Key]
        public int id { get; set; }
        public required string TipoPoliza { get; set; }
    }
}