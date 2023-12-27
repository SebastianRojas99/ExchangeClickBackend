using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeClick.Entities;
using ExchangeClick.Models;
using ExchangeClick.Models.DTO.UsersDTO;
using ExchangeClick.Models.Enum;
using ExchangeClick.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExchangeClick.Services
{
    public class UserServices:IUserServices
    {
        private readonly ExchangeClickContext _context;

        public UserServices(ExchangeClickContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateUser(UserForRegister dto)
        {
            if (await _context.Users.AnyAsync(u=>u.Username == dto.Username))
            {
                return false;
            }            
            var newUser = new User
            {
                Name = dto.Name,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = dto.Password,
                Username = dto.Username,
                SubscriptionId = dto.SubscriptionId,

                Role = Role.User,
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return true;
            }


        public async Task<bool> CreateAdmin(UserForRegister dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            {
                return false;
            }
            var newAdmin = new User
            {
                Name = dto.Name,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = dto.Password,
                Username = dto.Username,
                SubscriptionId = dto.SubscriptionId,


                Role = Role.Admin,
            };
            _context.Users.Add(newAdmin);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<List<UserForGetDTO>> GetUsers()
        {


            var users = await _context.Users.Include(u => u.Subscription).ToListAsync();


            return users.Select(u => new UserForGetDTO
            {
                UserId = u.UserId,
                Name = u.Name,
                LastName = u.LastName,
                Email = u.Email,
                Username = u.Username,
                SubscriptionId = u.SubscriptionId,
                SubscriptionName = u.Subscription?.SubscriptionName,
                SubCount = u.Subscription.SubCount
                
            }).ToList();
            
        }

        public async Task<List<UserProfileDTO>> Profile(int id)
        {
            var users = await _context.Users.Where(c => c.UserId == id).Include(u => u.Subscription).ToListAsync();

            return users.Select(u => new UserProfileDTO
            {
                UserId = u.UserId,
                Name = u.Name,
                LastName = u.LastName,
                Email = u.Email,
                Username = u.Username,
                SubscriptionName = u.Subscription.SubscriptionName,
                SubCount = u.Subscription.SubCount,
            }).ToList();

        }


        public async Task<bool> UpdateUser(string uname, UserForLoginDTO dto)
        {
            try
            {
                var update = await _context.Users.SingleOrDefaultAsync(u => u.Username == uname);

                if (update == null)
                {
                    return false; 
                }

                if (update.Username != dto.Username || update.Password != dto.Password)
                {
                    if (dto.Username != null)
                    {
                        update.Username = dto.Username;
                    }

                    if (dto.Password != null)
                    {
                        update.Password = dto.Password;
                    }
                }

                await _context.SaveChangesAsync();
                return true; // Update successful
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                return false; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error updating user: {ex.Message}");
                return false; 
            }
        }
        

        public async Task<bool> DeleteUser(UserForLoginDTO dto)
        {
            try
            {
                var userDel = await _context.Users.SingleOrDefaultAsync(u => u.Username == dto.Username);

                if (userDel != null)
                {
                    _context.Users.Remove(userDel);
                    await _context.SaveChangesAsync();
                    return true; // Deletion successful
                }

                return false; // User not found for deletion
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
                return false; // Error deleting the user.
            }
        }

        public async Task<int> getSubCountById(int UserId)
        {
            var s = await _context.Users.Include(u => u.Subscription).FirstOrDefaultAsync(x => x.UserId == UserId);

            return s.Subscription.SubCount;
        }

        public async Task<bool> ChangeSub(UserForGetDTO dto, int subNum)
        {
            var change = await _context.Users.SingleOrDefaultAsync(s => s.SubscriptionId == subNum);
            if (change != null)
            {
                // Perform the necessary changes to the 'change' object
                // ...
                await _context.SaveChangesAsync();
                return true; // Change successful
            }
            return false; // Subscription not found
        }

        public User? ValidateUser(AuthenticationRequestDTO authRequestBody)
        {
            return _context.Users.FirstOrDefault(p => p.Email == authRequestBody.Email && p.Password == authRequestBody.Password);
        }

    }
}
