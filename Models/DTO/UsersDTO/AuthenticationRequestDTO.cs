using System;
using System.ComponentModel.DataAnnotations;

namespace ExchangeClick.Models.DTO.UsersDTO
{
	public class AuthenticationRequestDTO
	{
            [Required]
            public string? Email { get; set; }
            [Required]
            public string? Pass { get; set; } 
    }
	
}

