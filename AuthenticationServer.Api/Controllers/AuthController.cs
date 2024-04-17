using AuthenticationServer.Api.Models;
using AuthenticationServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationServer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthenticationService service) : ControllerBase
    {
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid || !request.ConfirmPassword.Equals(request.Password))
            {
                return BadRequest();
            }
            try
            {
                await service.Register(request.UserName, request.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                var token = await service.Login(request.UserName, request.Password);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return Problem("LOGIN FAILED");
            }
        }
    }

}
