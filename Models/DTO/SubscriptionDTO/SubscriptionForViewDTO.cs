using System;
namespace ExchangeClick.Models.DTO.SubscriptionDTO
{
	public class SubscriptionForViewDTO
	{
        public int SubscriptionId { get; set; }
        public string? SubscriptionName { get; set; }
        public decimal SubPrice { get; set; }
        public int SubCount { get; set; }
    }
}

