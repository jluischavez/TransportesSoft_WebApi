using System.ComponentModel.DataAnnotations;

namespace TransportesSoft_WebApi.Models
{
    public class ContOperadoresCat
    {
        [Key]
        public int id_Operador { get; set; }
        public required string Nombre { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaEgreso { get; set; }
        public string Estatus { get; set; } = "A";
        public int? EmpresaId { get; set; }
    }
}