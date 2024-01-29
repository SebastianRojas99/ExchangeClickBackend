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
            int userId = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("nameidentifier"))!.Value);
            var currencies = await _currencyService.GetCurrenciesAsync(userId);
            return Ok(currencies);
        }

        [HttpGet("{currencyId}")]
        public async Task<IActionResult> GetCurrency(int currencyId)
        {
            var currency = await _currencyService.GetCurrencyAsync(currencyId);

            if (currency == null)
            {
                return BadRequest();
            }

            return Ok(currency);
        }

        [HttpPost("crear-nueva-moneda")]
        public async Task<IActionResult> AddCurrency( CurrencyForCreate currencyDTO)
        {
            int userId = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("nameidentifier"))!.Value);
            var result = await _currencyService.AddCurrencyAsync(currencyDTO,userId);

            if (result)
            {
                return Created("GetCurrency", result); // Cambiar a Created si deseas proporcionar una URL para la nueva moneda
            }

            return Conflict("El símbolo de la moneda ya existe.");
        }

        [HttpPost]
        public async Task<IActionResult> ConvertCurrency( CurrencyConversionRequestDTO request)
        {
            // Crear instancias de CurrencyForConvesionDTO con los símbolos proporcionados
            int userId = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("nameidentifier"))!.Value);
            var currency1 = new CurrencyForConvesionDTO { CurrencySymbol = request.Symbol1 };
            var currency2 = new CurrencyForConvesionDTO { CurrencySymbol = request.Symbol2 };
            

            var conversionValue = await _currencyService.Exchange(currency1, currency2, request.Quantity,userId);

                return Ok(new { conversionRate = conversionValue });            
        }

        [HttpPut]
        
        public async Task<IActionResult> UpdateCurrency( CurrencyForCreate updatedCurrency, int currencyId)
        {
            
            
            var result = await _currencyService.UpdateCurrencyAsync( updatedCurrency, currencyId);

            if (result)
            {
                return NoContent();
            }

            return NotFound("La moneda con el símbolo proporcionado no se encontró en la base de datos.");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCurrency(int currencyId)
        {
            var result = await _currencyService.DeleteCurrencyAsync(currencyId);

            if (result)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
