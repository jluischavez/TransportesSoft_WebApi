using System.ComponentModel.DataAnnotations;

namespace TransportesSoft_WebApi.Models
{
    public class ContClientesCat
    {
        [Key]
        public int id_Client { get; set; }
        public required string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Estatus { get; set; }
        public int? EmpresaId { get; set; }
    }
}
