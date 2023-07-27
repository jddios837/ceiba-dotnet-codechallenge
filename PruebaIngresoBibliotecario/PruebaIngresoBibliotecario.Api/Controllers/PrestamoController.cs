using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PruebaIngresoBibliotecario.Api.Exceptions;
using PruebaIngresoBibliotecario.Api.InputModels;
using PruebaIngresoBibliotecario.Api.Services.Interfaces;
using PruebaIngresoBibliotecario.Core.Models;

namespace PruebaIngresoBibliotecario.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrestamoController : ControllerBase
    {
        private readonly IPrestamoService _prestamoService;

        public PrestamoController(IPrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
        }
        
        [HttpPost]
        public async Task<IActionResult> CrearPrestamo([FromBody] CrearPrestamoInputModel model)
        {
            try
            {
                var prestamo = await _prestamoService.Create(model);
                return Ok(new
                {
                    id = prestamo.Id,
                    fechaMaximaDevolucion = prestamo.FechaMaximaDevolucion.ToString("dd/MM/yyyy")
                });
            }
            catch (UsuarioConPrestamoException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("{idPrestamo:Guid}")]
        public async Task<IActionResult> GetPrestamo([FromRoute] Guid idPrestamo)
        {
            if (idPrestamo == Guid.Empty)
                return BadRequest($"Prestamo Id no puede estar vacio {idPrestamo}");

            try
            {
                var prestamo = await _prestamoService.Get(idPrestamo);
                return Ok(prestamo);
            }
            catch (Exception ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }
        
    }
}
