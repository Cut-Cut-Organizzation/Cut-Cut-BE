using Cut_Cut.API.Auth;
using Cut_Cut.BLL.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Cut_Cut.DAL.Entities;
using Cut_Cut.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Cut_Cut.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        public AuthenticationController(IAuthService authService, IUserService userService, IConfiguration configuration)
        {
            _authService = authService;
            _userService = userService;
            _configuration = configuration;
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var res = await _authService.Login(model);
            if(res.Data.AccessToken != null)
                return Ok(res);
            return Unauthorized();
    }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO register)
        {

            var result = await _userService.AddUser(register);
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var authHeader = Request.Headers.Authorization.FirstOrDefault();
                if (authHeader != null && authHeader.StartsWith("Bearer "))
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    var res = await _authService.Logout(token);
                }
                else 
                {
                    BadRequest();
                }
                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }
    
    }
}
