using System.ComponentModel.DataAnnotations;

namespace TransportesSoft_WebApi.Models
{
    public class ContRemolquesCat
    {
        [Key]
        public int id_Remolque { get; set; }
        public required string Marca { get; set; }
        public required string Modelo { get; set; }
        public required string Serie { get; set; }
        public int Year { get; set; }
        public required string Placas { get; set; }
        public DateTime? Fecha_Llantas { get; set; }
        public DateTime? Fecha_Fisico_SCT { get; set; }
        public DateTime? Impermeabilizacion { get; set; }
        public int? EmpresaId { get; set; }
    }
}