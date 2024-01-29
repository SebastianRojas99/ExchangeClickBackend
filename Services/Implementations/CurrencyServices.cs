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

        public async Task<List<CurrenciesForGetDTO>> GetCurrenciesAsync(int id)
        {
            
            var currencies = await _context.Currencies.Include(u=>u.User).ToListAsync();
            return currencies.Where(c => c.User?.UserId == id).Select(currency => new CurrenciesForGetDTO
            {
                CurrencyId = currency.CurrencyId,
                CurrencyName = currency.CurrencyName,
                CurrencySymbol = currency.CurrencySymbol,
                CurrencyValue = currency.CurrencyValue,
            }).ToList();
        }

        public async Task<CurrenciesForGetDTO> GetCurrencyAsync(int id)
        {
            var currency = await _context.Currencies
                .Where(c => c.CurrencyId == id)
                .Select(c => new CurrenciesForGetDTO
                {
                    CurrencyId = c.CurrencyId,
                    CurrencyName = c.CurrencyName,
                    CurrencySymbol = c.CurrencySymbol,
                    CurrencyValue = c.CurrencyValue,
                })
                .FirstOrDefaultAsync();

            if (currency == null)
            {
                // Puedes manejar el caso en el que la moneda no se encuentra.
                throw new Exception($"No se encontró una moneda con id {id}");
            }

            return currency;
        }

        public async Task<decimal> Exchange(CurrencyForConvesionDTO symbol1, CurrencyForConvesionDTO symbol2, int quantity, int loggedUser)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == loggedUser);

            var c1 = await _context.Currencies.FirstOrDefaultAsync(c => c.CurrencySymbol == symbol1.CurrencySymbol);
            var c2 = await _context.Currencies.FirstOrDefaultAsync(c => c.CurrencySymbol == symbol2.CurrencySymbol);

            if (user != null && c1 != null && c2 != null)
            {
                var conversionValue = c1.CurrencyValue * quantity / c2.CurrencyValue;

                if (user.SubscriptionId != null && user.SubCount > 0)
                {
                    user.SubCount--;
                    await _context.SaveChangesAsync();
                    
                }
                return conversionValue;

            }
            else
            {
                return 0;
            }
        }



        public async Task<bool> AddCurrencyAsync(CurrencyForCreate currencyDTO, int loggedUser)//agregar
        {

            var newCurrency = new Currency
            {
                
                CurrencyName = currencyDTO.CurrencyName,
                CurrencySymbol = currencyDTO.CurrencySymbol?.ToLower(),
                CurrencyValue = currencyDTO.CurrencyValue,
                UserId = loggedUser
                
            };

            _context.Currencies.Add(newCurrency);
            await _context.SaveChangesAsync();

            return true; 
        }

        public async Task<bool> UpdateCurrencyAsync( CurrencyForCreate updatedCurrency, int currencyId)
        {
            var existingCurrency = await _context.Currencies
                .SingleOrDefaultAsync(c => c.CurrencyId == currencyId);


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

        public async Task<bool> DeleteCurrencyAsync(int currencyId)
        {
            var currency = await _context.Currencies.FindAsync(currencyId);

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

