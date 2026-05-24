namespace TransportesSoft_WebApi.Data
{
    public class AsignarEmpresaDto
    {
        public int UsuarioId { get; set; }
        public int EmpresaId { get; set; }
        public required string ClaveAcceso { get; set; }
    }
}
