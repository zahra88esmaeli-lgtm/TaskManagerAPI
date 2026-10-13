using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerAPI.Services.Interface;
using TaskManagerAPI.Dtos;
namespace TaskManagerAPI.Controllers
   
{
    [ApiController]
    [Route("api/[controller]")]
   
    public class UserController : ControllerBase
    {
     
        private readonly IUserService _userService;
       
        public UserController(IUserService userService )
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
          var result=await _userService.RegisterAsync(dto);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _userService.LoginAsync(dto);
            if (!result.Success)
                return Unauthorized(result.Message);
            return Ok(new {token=result.Token});
        }


        [Authorize]
        [HttpPut("me/ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
             var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(idClaim, out var userId))
                return Unauthorized("Invalid token.");

            var result = await _userService.ChangePasswordAsync(userId,dto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
        [Authorize]
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteUser()
        {

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(idClaim, out var userId))
                return Unauthorized("Invalid token.");

            var result = await _userService.DeleteAsync(userId);

            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result.Message);

        }
    }
}
