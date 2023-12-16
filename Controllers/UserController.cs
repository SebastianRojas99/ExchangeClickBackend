using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeClick.Services;
using ExchangeClick.Services.Implementations;
using Microsoft.AspNetCore.Mvc;
using ExchangeClick.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        public async Task<IActionResult> getUsers()
        {
            var users = await _service.GetUsers();
            return Ok(users);
        }

        [HttpPost("userCreation")]
        //[AllowAnonymous]
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
        //[AllowAnonymous]
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
        public async Task<IActionResult> update([FromBody] string uname,UserForLoginDTO u)
        {
            var user = await _service.UpdateUser(uname,u);
            return Ok(user);
        }
        [HttpDelete]
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

