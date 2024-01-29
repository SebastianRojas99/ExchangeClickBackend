using System;
using System.Linq;
using ExchangeClick.Entities;
using ExchangeClick.Models;
using ExchangeClick.Models.DTO.SubscriptionDTO;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ExchangeClick.Services.Implementations
{
	public class SubscriptionServices
	{
        private readonly ExchangeClickContext _context;
        public SubscriptionServices(ExchangeClickContext context)
		{
            _context = context;
        }

        public async Task<List<SubscriptionForViewDTO>> GetSubscriptionsAvaible()
        {
            var myList = await _context.Subscriptions.ToListAsync();
            return myList.Select(subs => new SubscriptionForViewDTO
            {
                SubscriptionId = subs.SubscriptionId,
                SubscriptionName = subs.SubscriptionName,
                SubCount = subs.SubCount,
                SubPrice = subs.SubPrice,

            }).ToList();
        }

        

        public async Task<bool> AddSubs(SubscriptionForSelectDTO dto)
        {
            if (await _context.Subscriptions.AnyAsync(c => c.SubscriptionName == dto.SubscriptionName))
            {
                return false; // Indica conflicto, la suscripción ya existe.
            }

            var newSub = new Subscription
            {
                SubscriptionName = dto.SubscriptionName,
                SubPrice = GetDefaultSubPrice(dto.SubscriptionName),
                SubCount = GetDefaultSubCount(dto.SubscriptionName)
            };

            _context.Subscriptions.Add(newSub);
            await _context.SaveChangesAsync();
            return true;
        }

            
        


        private decimal GetDefaultSubPrice(string subscriptionName)
        {
            return subscriptionName switch
            {
                "Subscription Free" => 0,
                "Subscription Trial" => 5,
                "Subscription Pro" => 20,
                _ => (decimal)0,
            };
        }

        private int GetDefaultSubCount(string subscriptionName)
        {
            return subscriptionName switch
            {
                "Subscription Free" => 10,
                "Subscription Trial" => 100,
                "Subscription Pro" => int.MaxValue,
                _ => 0,
            };
        }

    }
}

