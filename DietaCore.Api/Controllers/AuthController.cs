using DietaCore.Business.Abstract;
using DietaCore.Dto.AuthDTOs;
using DietaCore.Shared.Common.Responses.Concrete;
using DietaCore.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DietaCore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost]
        [Route("Register")]
        public async Task<Response<AuthResponseDto>> Register([FromBody] RegisterRequestDto model)
        {
            return await _authService.RegisterAsync(model);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<Response<AuthResponseDto>> Login([FromBody] LoginRequestDto model)
        {
            return await _authService.LoginAsync(model);
        }

        [HttpPost]
        [Route("ConfirmEmail")]
        public async Task<Response<bool>> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            return await _authService.ConfirmEmailAsync(userId, token);
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<Response<string>> ForgotPassword([FromQuery] string email)
        {
            return await _authService.GeneratePasswordResetTokenAsync(email);
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<Response<bool>> ResetPassword([FromQuery] string email, [FromQuery] string token, [FromQuery] string newPassword)
        {
            return await _authService.ResetPasswordAsync(email, token, newPassword);
        }
    }
}
