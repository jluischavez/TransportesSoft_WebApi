using System.ComponentModel.DataAnnotations;

namespace TransportesSoft_WebApi.Models
{
    public class ContKilometrajeUnidad
    {
        [Key]
        public int Id { get; set; }
        public int id_Unidad { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int KilometrajeActual { get; set; }
        public int? EmpresaId { get; set; }
    }
}