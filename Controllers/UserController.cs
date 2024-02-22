using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeClick.Services;
using ExchangeClick.Services.Implementations;
using Microsoft.AspNetCore.Mvc;
using ExchangeClick.Models;
using Microsoft.AspNetCore.Authorization;
using ExchangeClick.Models.Enum;
using Microsoft.EntityFrameworkCore;
using ExchangeClick.Models.DTO.UsersDTO;
using ExchangeClick.Entities;

namespace ExchangeClick.Controllers
{
    [Route("api/[controller]")]

    public class UserController : Controller
    {
        private readonly ExchangeClickContext _context;
        private readonly UserServices _service;

        public UserController(ExchangeClickContext context, UserServices service)
        {
            _context = context;
            _service = service;
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> getUsers()
        {
            var users = await _service.GetUsers();
            return Ok(users);
        }

        [HttpGet("Profile/{userId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetProfile()
        {
            int userId = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("nameidentifier"))!.Value);
            var user = await _service.Profile(userId);
            return Ok(user);
        }

        [HttpGet("GetUserById/{userId}")]
        public async Task<IActionResult> GetById(int userId)
        {
            var user = await _service.GetUserById(userId);

            if (user == null)
            {
                return BadRequest();
            }

            return Ok(user);
        }

        [HttpGet("isAdmin/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> IsAdmin()
        {
            try
            {
                var userId = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("nameidentifier"))!.Value);
                var user = await _service.IsAdmin(userId);
                if (user)
                {
                    return Ok();
                }
                return BadRequest("es usuario no administrador");
                
            }
            catch(Exception ex)
            {
                return BadRequest("error en admin");
            }
            
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetSubCountById()
        {
            try
            {
                // Obtener el ID del usuario desde los claims
                int userId = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("nameidentifier"))!.Value);

                // Obtener la cantidad de suscriptores por ID
                int subCount = await _service.getSubCountById(userId);

                return Ok(subCount);
            }
            catch (Exception ex)
            {
                // Manejar errores según sea necesario
                return BadRequest("error inesperado");
            }
        }


        [HttpPost("userCreation")]
        [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public async Task<IActionResult> userCreation( [FromBody]UserForRegister u)
        {
            var res = await _service.CreateUser(u);
            if (res)
            {
                return Created("getUsers", null); // Cambiar a Created si deseas proporcionar una URL para la nueva moneda
            }
            return Conflict("usuario ya creado!");
        }
        [HttpPost("adminCreation")]
        [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public async Task<IActionResult> adminCreation([FromBody] UserForRegister a)
        {
            var res = await _service.CreateAdmin(a);
            if (res)
            {
                return Created("getUsers", null); // Cambiar a Created si deseas proporcionar una URL para la nueva moneda
            }
            return Conflict("admin ya creado!");
        }
        [HttpPut]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> updateUser([FromBody]UserForUpdate updatedUser ,int userId)
        {
            var result = await _service.EditUserOrAdmin(updatedUser,userId);
            if (result)
            {
                return NoContent();
            }

            return NotFound("El usuario con el id proporcionado no se encontró en la base de datos.");
    }

        [HttpPut("UpdateSubscription")]
        [Authorize(Roles = ("User,Admin"))]
        public async Task<IActionResult> UpdateSub([FromBody]string subscriptionName)
        {
            int userId = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("nameidentifier"))!.Value);
            var result = await _service.EditSub(subscriptionName, userId);
            if (result)
            {
                return NoContent();
            }

            return NotFound("No se pudo actualizar tu suscripcion");
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> delete(int userId)
        {
            var userDelete = await _service.DeleteUser(userId);
            if (userDelete)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}

