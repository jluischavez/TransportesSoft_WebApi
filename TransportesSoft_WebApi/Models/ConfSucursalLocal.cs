using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportesSoft_WebApi.Models
{
    public class ConfSucursalLocal
    {
        [Key]
        public int id_Sucursal { get; set; }
        public string? NombreSucursal { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? URLImagen { get; set; }
        public int? KilometrajeNotificaciones { get; set; }
        public string RutaReportes { get; set; } = string.Empty;
        public int? EmpresaId { get; set; }
    }
}
