using Microsoft.AspNetCore.Mvc;

namespace TransportesSoft_WebApi.Controllers
{
    public abstract class BaseApiController : ControllerBase
    {
        protected int? ObtenerEmpresaId()
        {
            var empresaIdClaim = User.FindFirst("EmpresaId")?.Value;

            if (!int.TryParse(empresaIdClaim, out var empresaId))
                return null;

            return empresaId;
        }

        protected IActionResult SinEmpresaAsignada()
        {
            return Unauthorized(new { mensaje = "No tienes empresa asignada." });
        }
    }
}