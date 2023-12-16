using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeClick.Models.DTO.SubscriptionDTO;
using ExchangeClick.Services.Implementations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExchangeClick.Controllers
{
    [Route("api/[controller]")]
    
    public class SubscriptionController : Controller
    {
        private readonly ExchangeClickContext _context;
        private readonly SubscriptionServices _service;
        public SubscriptionController(ExchangeClickContext context, SubscriptionServices service)
        {
            _context = context;
            _service = service;
        }
        [HttpGet("show-subs")]
        public async Task<IActionResult> getSubs()
        {
            var subs = await _service.GetSubscriptionsAvaible();
            return Ok(subs);
        }
        [HttpPost("crear-sub")]
        public async Task<IActionResult> AddSub([FromBody] SubscriptionForSelectDTO dto)
        {
            var res = await _service.AddSubs(dto);
            if (res)
            {
                return Ok("Creado con exito");
            }
             return Conflict("no se pudo crear");
        }
        
    }
}

