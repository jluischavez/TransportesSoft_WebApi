using System.ComponentModel.DataAnnotations;

namespace TransportesSoft_WebApi.Models
{
    public class ContUnidadesCat
    {
        [Key]
        public int id_Unidad { get; set; }
        public required string Marca { get; set; }
        public required string Serie { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public int id_Operador { get; set; }
        public string Estatus { get; set; } = "A";
        public int id_Remolque { get; set; }
        public int? EmpresaId { get; set; }
    }
}