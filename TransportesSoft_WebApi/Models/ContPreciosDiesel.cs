using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportesSoft_WebApi.Models
{
    public class ContPreciosDiesel
    {
        [Key]
        public int IdDiesel { get; set; }
        [Column(TypeName = "money")]
        public decimal Precio { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaExpiro { get; set; }
        public int? EmpresaId { get; set; }
    }
}