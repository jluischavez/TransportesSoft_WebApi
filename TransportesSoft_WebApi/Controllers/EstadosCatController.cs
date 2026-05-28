using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportesSoft_WebApi.Data;

namespace TransportesSoft_WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class EstadosCatController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EstadosCatController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var estados = await _context.EstadosCat
                .OrderBy(e => e.Nombre)
                .ToListAsync();
            return Ok(estados);
        }
    }
}