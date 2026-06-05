using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportesSoft_WebApi.Data;

namespace TransportesSoft_WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MunicipiosCatController : BaseApiController
    {
        private readonly AppDbContext _context;
        public MunicipiosCatController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var municipios = await _context.MunicipiosCat
                .Where(m => m.Activo)
                .OrderBy(m => m.Nombre)
                .ToListAsync();
            return Ok(municipios);
        }

        [HttpGet("estado/{idEstado}")]
        public async Task<IActionResult> GetByEstado(int idEstado)
        {
            var municipios = await _context.MunicipiosCat
                .Where(m => m.IdEstado == idEstado && m.Activo)
                .OrderBy(m => m.Nombre)
                .ToListAsync();
            return Ok(municipios);
        }
    }
}