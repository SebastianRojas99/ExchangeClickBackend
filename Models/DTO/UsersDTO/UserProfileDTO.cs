using System;
namespace ExchangeClick.Models.DTO.UsersDTO
{
	public class UserProfileDTO
	{
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; }
        public int SubscriptionId { get; set; }
        public string? SubscriptionName { get; set; }
        public int SubCount { get; set; }
    }
}

