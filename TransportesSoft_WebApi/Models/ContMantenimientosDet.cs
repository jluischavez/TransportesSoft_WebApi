using System.ComponentModel.DataAnnotations.Schema;

namespace TransportesSoft_WebApi.Models
{
    public class ContMantenimientosDet
    {
        public int IdMantenimiento { get; set; }
        public required string Refaccion { get; set; }
        public required string Proveedor { get; set; }
        [Column(TypeName = "money")]
        public decimal PrecioRefaccion { get; set; }
        public required string Comentarios { get; set; }
        public int Renglon { get; set; } = 1;
        public int? EmpresaId { get; set; }
    }
}