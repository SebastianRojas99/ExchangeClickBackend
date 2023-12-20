using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExchangeClick.Entities;
using ExchangeClick.Models;
using ExchangeClick.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeClick.Models.DTO.CurrenciesDTO;
using ExchangeClick.Models.DTO.UsersDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace ExchangeClick.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="User,Admin")]
    
    public class CurrencyController : ControllerBase
    {
        private readonly ExchangeClickContext _context;
        private readonly CurrencyServices _currencyService;

        public CurrencyController(ExchangeClickContext context, CurrencyServices currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrencies()
        {
            var currencies = await _currencyService.GetCurrenciesAsync();
            return Ok(currencies);
        }

        [HttpGet("obtener-monedas/{id}")]
        public async Task<IActionResult> GetCurrency(int id)
        {
            var currency = await _currencyService.GetCurrencyAsync(id);

            if (currency == null)
            {
                return BadRequest();
            }

            return Ok(currency);
        }

        [HttpPost("crear-nueva-moneda")]
        public async Task<IActionResult> AddCurrency( CurrencyForCreate currencyDTO)
        {
            var result = await _currencyService.AddCurrencyAsync(currencyDTO);

            if (result)
            {
                return Created("GetCurrency", null); // Cambiar a Created si deseas proporcionar una URL para la nueva moneda
            }

            return Conflict("El símbolo de la moneda ya existe.");
        }

        [HttpPost("convertir-moneda")]
        public async Task<IActionResult> ConvertCurrency(string symbol1, string symbol2, int quantity)
        {
            // Crear instancias de CurrencyForConvesionDTO con los símbolos proporcionados
            var currency1 = new CurrencyForConvesionDTO { CurrencySymbol = symbol1 };
            var currency2 = new CurrencyForConvesionDTO { CurrencySymbol = symbol2 };
            var conversionValue = await _currencyService.Exchange(currency1, currency2, quantity);
            int userId = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("nameidentifier"))!.Value);
            var u = await _context.Users.Include(s => s.Subscription).SingleOrDefaultAsync(x => x.UserId == userId);
            if (conversionValue > 0 && u.Subscription.SubCount>0)
            {
                // Si la conversion fue exitosa, resta un intento de conversion de la suscripción del usuario y guarda los cambios
                
                u.Subscription.SubCount--;
                await _context.SaveChangesAsync();

                // Devuelve el valor de cambio
                return Ok(new { conversionRate = conversionValue });
            }
            else
            {
                // Si la conversion falla, devuelve un error
                return BadRequest("No se pudo realizar la conversion de moneda.");
            }
        }
        [HttpPut("actualizar-moneda")]
        public async Task<IActionResult> UpdateCurrency(string symbol, [FromBody] CurrenciesForGetDTO updatedCurrency)
        {
            if (symbol == updatedCurrency.CurrencySymbol)
            {
                return BadRequest("El símbolo proporcionado se encuentra en uso.");
            }

            var result = await _currencyService.UpdateCurrencyAsync(symbol, updatedCurrency);

            if (result)
            {
                return NoContent();
            }

            return NotFound("La moneda con el símbolo proporcionado no se encontró en la base de datos.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurrency(int id)
        {
            var result = await _currencyService.DeleteCurrencyAsync(id);

            if (result)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
