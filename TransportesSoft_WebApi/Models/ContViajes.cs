using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportesSoft_WebApi.Models
{
    public class ContViajes
    {
        [Key]
        public int id_Viaje { get; set; }
        public int id_Client { get; set; }
        public required string NombreCliente { get; set; }
        public DateTime FechaViaje { get; set; }
        public DateTime FechaFactura { get; set; }
        public required string Factura { get; set; }
        public required string NumeroTransporte { get; set; }
        public required string Origen { get; set; }
        public required string Destino { get; set; }
        [Column(TypeName = "money")]
        public decimal Monto { get; set; }
        [Column(TypeName = "money")]
        public decimal IVA { get; set; }
        [Column(TypeName = "money")]
        public decimal Retenciones { get; set; }
        [Column(TypeName = "money")]
        public decimal Total { get; set; }
        public required string Comentarios { get; set; }
        [Column(TypeName = "money")]
        public decimal Maniobra { get; set; }
        public int id_Operador { get; set; }
        public int id_Unidad { get; set; }
        public int id_Remolque { get; set; }
        public int? EmpresaId { get; set; }
    }
}