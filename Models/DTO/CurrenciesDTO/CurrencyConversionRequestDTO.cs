using System;
namespace ExchangeClick.Models.DTO.CurrenciesDTO
{
	public class CurrencyConversionRequestDTO
	{
        public string? Symbol1 { get; set; }
        public string? Symbol2 { get; set; }
        public int Quantity { get; set; }
    }
}

