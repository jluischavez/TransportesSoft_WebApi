using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportesSoft_WebApi.Models
{
    public class ContConsumoUnidades
    {
        public int id_Unidad { get; set; }
        public DateTime Fecha { get; set; }
        public int ConsumoLitros { get; set; }
        public string? Comentarios { get; set; }
        [Column(TypeName = "money")]
        public decimal ConsumoPesos { get; set; }
        public int? EmpresaId { get; set; }
    }
}
