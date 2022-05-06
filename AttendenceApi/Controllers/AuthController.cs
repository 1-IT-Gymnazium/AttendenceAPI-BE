
using AbsenceProjektSDarou.Models.Identity;
using AttendenceApi.Data;
using AttendenceApi.Data.Indentity;
using AttendenceApi.Services;
using AttendenceApi.ViewModels;
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
        private readonly AppDbContext _context;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IUserService userService, AppDbContext context, ILogger<AuthController> logger)
        {
            _userService = userService;
            _context = context;
            _logger = logger;
        }

        [HttpPost("Authenticate")]
        public async Task<IActionResult> LoginAsync ([FromBody] LoginViewModel model)
        {
             var result = await _userService.LoginAsync(model);
            if (result)
            {

                var User = _context.Users.Single(s => s.UserName == model.Email);
                var role = _context.UserRoles.Single(s => s.UserId == User.Id);
                if (role.RoleId == 2)
                {
                    return Ok(true);
                }
               if (role.RoleId == 1)
                {
                    return Ok(false);
                }
    

                return BadRequest();


            }


            return Ok(result);
        }
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateVM model)
        {
            var user = new User { ClassId = model.ClassId, Email = model.Email, InSchool = false, UserName = model.UserName };
            var result = await _userService.CreateUser(user, model.Password);
            return Ok(result);
        }

             
    }
}
