using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeClick.Entities;
using ExchangeClick.Models;
using ExchangeClick.Models.DTO.CurrenciesDTO;
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
                Role = u.Role.ToString(),
                SubscriptionId = u.Subscription.SubscriptionId,
                SubscriptionName = u.Subscription?.SubscriptionName,
                SubCount = u.SubCount



            }).ToList();
            
        }

        public async Task<UserProfileDTO> GetUserById(int id)
        {
            var user = await _context.Users
                .Where(c =>c.UserId  == id)
                .Include(s=>s.Subscription)
                .Select(u => new UserProfileDTO
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    LastName = u.LastName,
                    Email = u.Email,
                    Username = u.Username,
                    Role = u.Role.ToString(),
                    SubscriptionId = u.Subscription.SubscriptionId,
                    SubscriptionName = u.Subscription.SubscriptionName,
                    SubCount = u.SubCount
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                // Puedes manejar el caso en el que la moneda no se encuentra.
                throw new Exception($"No se encontró una moneda con id {id}");
            }

            return user;
        }



        public async Task<UserProfileDTO> Profile(int id)
        {
            var user = await _context.Users
                .Where(c => c.UserId == id)
                .Include(u => u.Subscription)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                var userProfile = new UserProfileDTO
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    LastName = user.LastName,
                    Email = user.Email,
                    Username = user.Username,
                    SubscriptionName = user.Subscription?.SubscriptionName,
                    SubCount = user.SubCount,
                };
                userProfile.Role = user.Role.ToString();

                return userProfile;
            }
            else
            {
                return null; // O lanzar una excepción si se prefiere
            }
        }


        public async Task<bool> CreateUser(UserForRegister dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            {
                return false;
            }

            // Obtener el valor del SubCount de forma asincrónica

            int subCount = await GetSubcount(dto.SubscriptionId = 1);
            var newAdmin = new User
            {
                Name = dto.Name,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = dto.Password,
                Username = dto.Username,
                SubscriptionId = 1,
                SubCount = subCount, // Asignar el valor obtenido
                Role = Role.User,
            };


            _context.Users.Add(newAdmin);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> CreateAdmin(UserForRegister dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            {
                return false;
            }

            // Obtener el valor del SubCount de forma asincrónica
            int subCount = await GetSubcount(dto.SubscriptionId = 1);

            var newAdmin = new User
            {
                Name = dto.Name,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = dto.Password,
                Username = dto.Username,
                SubscriptionId = dto.SubscriptionId,
                SubCount = subCount, // Asignar el valor obtenido
                Role = Role.Admin,
            };

            _context.Users.Add(newAdmin);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> EditUserOrAdmin(UserForUpdate updatedUser, int userId, Role newRole)
        {
            var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId);
            int subCount = await GetSubcount(updatedUser.SubscriptionId);

            if (existingUser == null)
            {
                return false;
            }

            // Actualiza las propiedades del usuario
            existingUser.Name = updatedUser.Name;
            existingUser.LastName = updatedUser.LastName;
            existingUser.Email = updatedUser.Email;
            existingUser.Username = updatedUser.Username;
            existingUser.SubscriptionId = updatedUser.SubscriptionId;
            existingUser.SubCount = subCount;
            existingUser.Role = newRole;
            // Actualiza el rol del usuario según el valor recibido desde el front
            

            // Guarda los cambios en la base de datos
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false; // Indica error al actualizar la moneda.
            }
        }

        public async Task<bool> DeleteUser(int userId)
        {
            try
            {
                var userDel = await _context.Users.FindAsync(userId);

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
        

        //SUBSCRIPTION CHANGES AND VALIDATIONS

        public User? ValidateUser(AuthenticationRequestDTO authRequestBody)
        {
            return _context.Users.FirstOrDefault(p => p.Email == authRequestBody.Email && p.Password == authRequestBody.Password);
        }


        public async Task<bool> IsAdmin(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user != null && user.Role == Role.Admin)
            {
                return true;
            }
            return false;
        }



        private async Task<int> GetSubcount(int id)
        {
            var subscriptionName = await _context.Subscriptions
                                    .Where(s => s.SubscriptionId == id)
                                    .Select(s => s.SubscriptionName)
                                    .FirstOrDefaultAsync();

            return subscriptionName switch
            {
                "Subscription Free" => 10,
                "Subscription Trial" => 100,
                "Subscription Pro" => 900000000,
                _ => 0,
            };
        }

        public async Task<int> getSubCountById(int UserId)
        {
            var s = await _context.Users.Include(u => u.Subscription).FirstOrDefaultAsync(x => x.UserId == UserId);

            return s.SubCount;
        }

        public async Task<bool> EditSub(int subscriptionId, int userId)
        {
            var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId);
            int subCount = await GetSubcount(subscriptionId);

            if (existingUser == null)
            {
                return false;
            }
            existingUser.SubscriptionId = subscriptionId;
            existingUser.SubCount = subCount;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false; // Indica error al actualizar la moneda.
            }
        }

    }
}
