using F1Analytics.Database.Models;
using F1Analytics.DTOs;
using F1Analytics.Requests;
using F1Analytics.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace F1Analytics.Controllers
{
    [ApiController]
    [Route("api/account")]
    [SwaggerTag("Managing user data. Handles the login, register, logout and change of password process." +
                "The action of registering and logging in does not require any permissions. The action of logging out and " +
                "changing of password requires the user to be authorized. ")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IAuthService _authService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IAuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
        }

        [HttpPost("register")]
        [SwaggerOperation(
            Summary = "Enable user to register.",
            Description = "Handles the process of registering a new user.",
            OperationId = "Register"
        )]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerResponse(404, "Error in request body")]
        [SwaggerResponse(204, "No content")]
        public async Task<IActionResult> Register(UserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError(err.Code, err.Description);
                return BadRequest(ModelState);
            }

            await _userManager.AddToRoleAsync(user, "Admin");

            return NoContent();
        }

        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Enables user to login",
            Description = "Handles the process of logging in.",
            OperationId="Login")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerResponse(401,"Unauthorized")]
        [SwaggerResponse(200, "User logged in")]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized("Invalid username or password");

            var token = await _authService.GenerateJwtTokenAsync(user);

            return Ok(new { Token = token, Expiration = DateTime.UtcNow.AddMinutes(60) });
        }

        [HttpPost("change-password")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Enables user change password",
            Description = "Handles the process of changing password.",
            OperationId="ChangePassword")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerResponse(401,"Unauthorized")]
        [SwaggerResponse(404, "Bad request")]
        [SwaggerResponse(204, "No content")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized("User not found.");

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return NoContent();
        }
        
        [HttpPost("logout")]
        [SwaggerOperation(
            Summary = "Logout process.",
            Description = "Handles the process logging out.",
            OperationId="Logout")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerResponse(204, "No content")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }
    }
}
