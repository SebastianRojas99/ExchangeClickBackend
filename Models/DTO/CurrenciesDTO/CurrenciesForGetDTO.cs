using System;
namespace ExchangeClick.Models
{
	public class CurrenciesForGetDTO
	{
		public int CurrencyId { get; set; }
		public string? CurrencyName { get; set; }
		public string? CurrencySymbol { get; set; }
		public decimal CurrencyValue { get; set; }
		
	}
}

