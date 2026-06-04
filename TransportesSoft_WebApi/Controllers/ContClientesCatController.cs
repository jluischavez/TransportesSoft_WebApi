using TransportesSoft_WebApi.Data;
using TransportesSoft_WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TransportesSoft_WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContClientesCatController : BaseApiController
    {
        private readonly AppDbContext _context;
        

        public ContClientesCatController(AppDbContext context)
        {
            _context = context;
        }

        // GET /ContClientesCat
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var empresaId = ObtenerEmpresaId();

            if (empresaId == null)
                return SinEmpresaAsignada();

            var clientes = await _context.ContClientesCat
                .Where(c => c.EmpresaId == empresaId.Value)
                .ToListAsync();

            return Ok(clientes);
        }

        // GET /ContClientesCat/1
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var empresaId = ObtenerEmpresaId();

            if (empresaId == null)
                return SinEmpresaAsignada();

            var cliente = await _context.ContClientesCat
                .FirstOrDefaultAsync(c =>
                    c.id_Client == id &&
                    c.EmpresaId == empresaId.Value
                );

            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        // POST /ContClientesCat
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(ContClientesCat cliente)
        {
            var empresaId = ObtenerEmpresaId();

            if (empresaId == null)
                return SinEmpresaAsignada();

            cliente.EmpresaId = empresaId.Value;

            _context.ContClientesCat.Add(cliente);
            await _context.SaveChangesAsync();

            return Ok(cliente);
        }

        // PUT /ContClientesCat/1
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ContClientesCat cliente)
        {
            var empresaId = ObtenerEmpresaId();

            if (empresaId == null)
                return SinEmpresaAsignada();

            var existing = await _context.ContClientesCat
                .FirstOrDefaultAsync(c =>
                    c.id_Client == id &&
                    c.EmpresaId == empresaId.Value
                );

            if (existing == null)
                return NotFound();

            existing.Nombre = cliente.Nombre;
            existing.Direccion = cliente.Direccion;
            existing.Telefono = cliente.Telefono;
            existing.Estatus = cliente.Estatus;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        // DELETE /ContClientesCat/1
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var empresaId = ObtenerEmpresaId();

            if (empresaId == null)
                return SinEmpresaAsignada();

            var cliente = await _context.ContClientesCat
                .FirstOrDefaultAsync(c =>
                    c.id_Client == id &&
                    c.EmpresaId == empresaId.Value
                );

            if (cliente == null)
                return NotFound();

            _context.ContClientesCat.Remove(cliente);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
