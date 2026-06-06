using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TransportesSoft_WebApi.Controllers
{
    public abstract class BaseApiController : ControllerBase
    {
        /*PARTE DE EMPRESAS*/
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

        /*PARTE DE USUARIOS*/
        protected int? ObtenerUsuarioId()
        {
            var usuarioIdClaim =
                User.FindFirst("UsuarioId")?.Value ??
                User.FindFirst("Id")?.Value ??
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(usuarioIdClaim, out var usuarioId))
                return null;

            return usuarioId;
        }
        protected IActionResult SinUsuarioValido()
        {
            return Unauthorized(new { mensaje = "No se pudo identificar al usuario." });
        }


        
    }
}