using AuthOpenAPIs.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthOpenAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /*

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            try
            {
                User user = await _userService.LoginAsync(request.Email, request.Password, cancellationToken);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LoginAsync");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            try
            {
                bool ret = await _userService.ChangePasswordAsync(request.Email, request.OldPassword, request.NewPassword, cancellationToken);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ChangePasswordAsync");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }*/
    }
}
