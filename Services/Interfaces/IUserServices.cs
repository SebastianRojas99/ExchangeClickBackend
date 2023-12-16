using ExchangeClick.Entities;
using ExchangeClick.Models.DTO.UsersDTO;
using System;
namespace ExchangeClick.Services.Interfaces
{
	public interface IUserServices
	{
       
            User ValidateUser(AuthenticationRequestDTO authRequestBody);
        
    }
}

