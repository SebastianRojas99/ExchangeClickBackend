using System;
namespace ExchangeClick.Models
{
	public class CurrenciesForGetDTO
	{
		
		public string? CurrencyName { get; set; }
		public string? CurrencySymbol { get; set; }
		public decimal CurrencyValue { get; set; }
		public string? UserName { get; set; }
	}
}

