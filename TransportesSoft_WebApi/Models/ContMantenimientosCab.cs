using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportesSoft_WebApi.Models
{
    public class ContMantenimientosCab
    {
        [Key]
        public int IdMantenimiento { get; set; }
        public DateTime FechaMantenimiento { get; set; }
        public int Kilometraje { get; set; }
        public required string Proveedor { get; set; }
        [Column(TypeName = "money")]
        public decimal CostoTotal { get; set; }
        public int id_Unidad { get; set; }
        public int id_Remolque { get; set; }
        public int? EmpresaId { get; set; }
    }
}