using System;
using System.ComponentModel.DataAnnotations;

namespace ExchangeClick.Models.DTO.SubscriptionDTO
{
	public class SubscriptionForAddDTO
	{

        [Required]
        public  string? SubscriptionName { get; set; }
        public decimal SubPrice { get; set; }
        public int SubCount { get; set; }
        
	}
}

