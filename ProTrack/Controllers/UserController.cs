using Domain.ProTrack.DTO;
using Domain.ProTrack.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ProTrack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServiceInterface _userService;
        private readonly IAuthServiceInterface _authService;
        private readonly ICustomeEmailServiceInterface _customeEmail;
        public UserController( IUserServiceInterface userService, IAuthServiceInterface authService, ICustomeEmailServiceInterface customeEmail)
        {
            _userService = userService;
            _authService = authService;
            _customeEmail = customeEmail;
        }
        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto registerUser)
        {
            try
            {
                var result = await _userService.CreateUserAsync(registerUser);
                if (!result.Item1.Succeeded)
                {
                    var errors = result.Item1.Errors.Select(e => e.Description).ToList();
                    return BadRequest(new {Message = "User registration failed", Error = errors});
                }
                return Ok(new {Message = "User registered successfully", User = registerUser, ConfirmationToken = result.Item2});
            }
            catch(Exception ex)
            {
                return StatusCode(500, new {Message="Internal Server Error! failed to register user", Error = ex.Message });
            }
        }
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync([FromQuery]string userId, [FromQuery]string token)
        {
            try
            {
                var result = await _customeEmail.ConfirmEmailAsync(userId, token);
                if (result)
                {
                    return Ok("Email Confirmed");
                }
                return BadRequest(new { Message = "Confirmation Error", Error = "Failed to confirm email" });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error! Failed to confirm email", Error = ex.Message });
            }
        }
        [HttpPost("login-user")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserDto loginUserDto)
        {
            try
            {
                var result = await _authService.LoginUserAsync(loginUserDto);
                return Ok(new {Message = "User loggedin Successfully", Token = result });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error Login Failed", Error = ex.Message });
            }
        }

    }
}
