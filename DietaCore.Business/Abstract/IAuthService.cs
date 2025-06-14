using DietaCore.Dto.AuthDTOs;
using DietaCore.Shared.Common.Responses.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietaCore.Business.Abstract
{
    public interface IAuthService
    {
        Task<Response<AuthResponseDto>> RegisterAsync(RegisterRequestDto model);
        Task<Response<AuthResponseDto>> LoginAsync(LoginRequestDto model);
        Task<Response<string>> GenerateEmailConfirmationTokenAsync(string userId);
        Task<Response<bool>> ConfirmEmailAsync(string userId, string token);
        Task<Response<string>> GeneratePasswordResetTokenAsync(string email);
        Task<Response<bool>> ResetPasswordAsync(string email, string token, string newPassword);
    }
}
