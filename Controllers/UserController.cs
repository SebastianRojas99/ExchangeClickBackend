using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeClick.Services;
using ExchangeClick.Services.Implementations;
using Microsoft.AspNetCore.Mvc;
using ExchangeClick.Models;
using Microsoft.AspNetCore.Authorization;


namespace ExchangeClick.Controllers
{
    [Route("api/[controller]")]
    

    public class UserController : Controller
    {
        private readonly ExchangeClickContext _context;
        private readonly UserServices _service;

        public UserController( ExchangeClickContext context, UserServices service)
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
        public async Task<IActionResult> getUserById()
        {
            int userId = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("nameidentifier"))!.Value);
            var user = await _service.Profile(userId);
            return Ok(user);
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> update(string uname,UserForLoginDTO u)
        {
            var user = await _service.UpdateUser(uname,u);
            return Ok(user);
        }
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> delete(UserForLoginDTO dto)
        {
            var userDelete = await _service.DeleteUser(dto);
            if (!userDelete)
            {
                return Conflict();
            }
            return Ok(userDelete);
        }
    }
}

