using System;
using System.ComponentModel.DataAnnotations;

namespace ExchangeClick.Models.DTO.CurrenciesDTO
{
	public class CurrencyForConvesionDTO
	{
		[Required]
		public string CurrencySymbol { get; set; }
	}
}

