﻿using Microsoft.AspNetCore.Mvc;
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
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetCurrencies()
        {
            int userId = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("nameidentifier"))!.Value);
            var currencies = await _currencyService.GetCurrenciesAsync(userId);
            return Ok(currencies);
        }

        [HttpGet("get-symbols")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetSymbols()
        {
            int userId = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("nameidentifier"))!.Value);
            var symbols = await _currencyService.GetSymbols(userId);
            return Ok(symbols);
        }

        [HttpGet("{currencyId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetCurrencyById(int currencyId)
        {
            var currency = await _currencyService.GetCurrencyAsync(currencyId);

            if (currency == null)
            {
                return BadRequest();
            }

            return Ok(currency);
        }

        [HttpPost("crear-nueva-moneda")]
        [Authorize(Roles = "User,Admin")]
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
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> ConvertCurrency( CurrencyConversionRequestDTO request)
        {
            int userId = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("nameidentifier"))!.Value);
            var currency1 = new CurrencyForConversionDTO { CurrencySymbol = request.Symbol1 };
            var currency2 = new CurrencyForConversionDTO { CurrencySymbol = request.Symbol2 };
            

            var conversionValue = await _currencyService.Exchange(currency1, currency2, request.Quantity,userId);

                if (conversionValue == 0)
                {
                    return BadRequest("Error en conversion");
                }
                return Ok(new { conversionRate = conversionValue });            
        }

        [HttpPut]
        [Authorize(Roles = "User,Admin")]
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
        [Authorize(Roles = "User,Admin")]
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
