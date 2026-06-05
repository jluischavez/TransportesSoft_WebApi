using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportesSoft_WebApi.Data;
using TransportesSoft_WebApi.Models;

namespace TransportesSoft_WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ContTiposPolizasController : BaseApiController
    {
        private readonly AppDbContext _context;
        public ContTiposPolizasController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tipos = await _context.ContTiposPolizas.ToListAsync();
            return Ok(tipos);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContTiposPolizas tipo)
        {
            _context.ContTiposPolizas.Add(tipo);
            await _context.SaveChangesAsync();
            return Ok(tipo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tipo = await _context.ContTiposPolizas.FindAsync(id);
            if (tipo == null) return NotFound();
            _context.ContTiposPolizas.Remove(tipo);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}