
using AbsenceProjektSDarou.Models.Identity;
using AttendenceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendenceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
       
        public AuthController(IUserService userService)
        {
            _userService = userService;
            
        }

        [HttpPost("Authenticate")]
        public async Task<IActionResult> LoginAsync ([FromBody] LoginViewModel model)
        {
            var result = await _userService.LoginAsync(model);
            return Ok(result);
        }

             
    }
}
