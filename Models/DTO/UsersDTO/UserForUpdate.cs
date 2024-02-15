using System;
using ExchangeClick.Models.Enum;

namespace ExchangeClick.Models.DTO.UsersDTO
{
	public class UserForUpdate
	{
        
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int SubscriptionId { get; set; }
        public Role Role { get; set; }
        
    }
}

