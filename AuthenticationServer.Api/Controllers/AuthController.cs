using AuthenticationServer.Api.Models;
using AuthenticationServer.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationServer.Api.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
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
                var (accessToken, refreshToken) = await service.Login(request.UserName, request.Password);
                return Ok(new { accessToken, refreshToken });
            }
            catch (Exception ex)
            {
                return Problem("LOGIN FAILED");
            }
        }

        [HttpPost]
        [Route("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                var token = await service.RefreshToken(request.Username, request.RefreshToken);
                return Ok(token);
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        [Route("validateToken")]
        public async Task<IActionResult> ValidateToken([FromBody] ValidateTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                var isValid = await service.ValidateToken(request.Token);
                return Ok(isValid);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                await service.Logout(request.Username);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }

}
