using Microsoft.EntityFrameworkCore;
using ExchangeClick.Entities;
using ExchangeClick.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ExchangeClick.Models.DTO.CurrenciesDTO;

namespace ExchangeClick.Services
{
    public class CurrencyServices
    {
        private readonly ExchangeClickContext _context;

        public CurrencyServices(ExchangeClickContext context)
        {
            _context = context;
        }

        public async Task<List<CurrenciesForGetDTO>> GetCurrenciesAsync()
        {
            var currencies = await _context.Currencies.ToListAsync();
            return currencies.Select(currency => new CurrenciesForGetDTO
            {
                CurrencyId = currency.CurrencyId,
                CurrencyName = currency.CurrencyName,
                CurrencySymbol = currency.CurrencySymbol,
                CurrencyValue = currency.CurrencyValue,
            }).ToList();
        }

        public async Task<Currency> GetCurrencyAsync(int id)
        {
             var c =  await _context.Currencies.FindAsync(id);

            if (c == null)
            {
                throw new Exception();
            }
            else
            {
                return (c);
            }
            
        }

        public async Task<decimal> Exchange(CurrencyForConvesionDTO CurrencySymbol, CurrencyForConvesionDTO symbol2, int quantity)
        {
            // Busca las monedas en la base de datos
            var c1 = await _context.Currencies.SingleOrDefaultAsync(c => c.CurrencySymbol == CurrencySymbol.CurrencySymbol);
            var c2 = await _context.Currencies.SingleOrDefaultAsync(c => c.CurrencySymbol == symbol2.CurrencySymbol);

            // Si las monedas se encontraron, calcula el valor de cambio
            if (c1 != null && c2 != null)
            {
                var conversionValue = c1.CurrencyValue * quantity / c2.CurrencyValue;
                return conversionValue;
            }
            else
            {
                // Si las monedas no se encontraron, lanza una excepción
                throw new InvalidOperationException("No se encontraron las monedas especificadas.");
            }
        }

        public async Task<List<Currency>> GetAllCurrencyAsync()
        {
            return await _context.Currencies.ToListAsync();
        }

        public async Task<bool> AddCurrencyAsync(CurrencyForCreate currencyDTO)//agregar
        {
            if (await _context.Currencies.AnyAsync(c => c.CurrencySymbol == currencyDTO.CurrencySymbol))
            {
                return false; // Indica conflicto, el símbolo de la moneda ya existe.
            }

            var newCurrency = new Currency
            {
                
                CurrencyName = currencyDTO.CurrencyName,
                CurrencySymbol = currencyDTO.CurrencySymbol?.ToLower(),
                CurrencyValue = currencyDTO.CurrencyValue,
                UserId = currencyDTO.UserId
                
            };

            _context.Currencies.Add(newCurrency);
            await _context.SaveChangesAsync();

            return true; 
        }

        public async Task<bool> UpdateCurrencyAsync(string symbol, CurrenciesForGetDTO updatedCurrency)
        {
            var existingCurrency = await _context.Currencies
                .SingleOrDefaultAsync(c => c.CurrencySymbol == symbol);


            if (existingCurrency == null)
            {
                return false; // Indica que la moneda no se encontró en la base de datos.
            }

            existingCurrency.CurrencyName = updatedCurrency.CurrencyName;
            existingCurrency.CurrencySymbol = updatedCurrency.CurrencySymbol;
            existingCurrency.CurrencyValue = updatedCurrency.CurrencyValue;

            try
            {
                await _context.SaveChangesAsync();
                return true; // Indica éxito en la operación
            }
            catch (DbUpdateConcurrencyException)
            {
                return false; // Indica error al actualizar la moneda.
            }
        }

        public async Task<bool> DeleteCurrencyAsync(int id)
        {
            var currency = await _context.Currencies.FindAsync(id);

            if (currency == null)
            {
                return false; // Indica que la moneda no se encontró en la base de datos.
            }

            _context.Currencies.Remove(currency);
            await _context.SaveChangesAsync();

            return true; // Indica éxito en la operación
        }

        public async Task<bool> CurrencyExistsAsync(int id)
        {
            return await _context.Currencies.AnyAsync(e => e.CurrencyId == id);
        }
    }
}

