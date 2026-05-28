using System.ComponentModel.DataAnnotations;

namespace TransportesSoft_WebApi.Models
{
    public class ContPolizasReg
    {
        [Key]
        public int id { get; set; }
        public required string FolioPoliza { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaExpira { get; set; }
        public int idTipoPoliza { get; set; }
        public int idUsuario { get; set; }
        public int? EmpresaId { get; set; }
    }
}