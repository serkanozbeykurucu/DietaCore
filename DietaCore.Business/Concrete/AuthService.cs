using DietaCore.Business.Abstract;
using DietaCore.Dto.AuthDTOs;
using DietaCore.Entities.Concrete;
using DietaCore.Shared.Common.Responses.ComplexTypes;
using DietaCore.Shared.Common.Responses.Concrete;
using DietaCore.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DietaCore.Business.Concrete
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        public AuthService(UserManager<User> userManager,SignInManager<User> signInManager,IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }
        public async Task<Response<AuthResponseDto>> RegisterAsync(RegisterRequestDto model)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return new Response<AuthResponseDto>(ResponseCode.BadRequest, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(user, RoleConstants.Client);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            var authResponse = new AuthResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = roles.FirstOrDefault(),
                Token = GenerateJwtToken(user, roles)
            };

            return new Response<AuthResponseDto>(ResponseCode.Success, authResponse, "User registered successfully.");
        }
        public async Task<Response<AuthResponseDto>> LoginAsync(LoginRequestDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new Response<AuthResponseDto>(ResponseCode.NotFound, "Invalid email or password.");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return new Response<AuthResponseDto>(ResponseCode.BadRequest, "Email not confirmed.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (!result.Succeeded)
            {
                return new Response<AuthResponseDto>(ResponseCode.BadRequest, "Invalid email or password.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);
            var tokenExpiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JWT:ExpiresInMinutes"] ?? "60"));

            var authResponse = new AuthResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = roles.FirstOrDefault(),
                Token = token,
                ExpiresAt = tokenExpiration
            };

            return new Response<AuthResponseDto>(ResponseCode.Success, authResponse, "Login successful.");
        }
        public async Task<Response<string>> GenerateEmailConfirmationTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new Response<string>(ResponseCode.NotFound, "User not found.");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return new Response<string>(ResponseCode.Success, token, "Email confirmation token generated successfully.");
        }
        public async Task<Response<bool>> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new Response<bool>(ResponseCode.NotFound, "User not found.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return new Response<bool>(ResponseCode.BadRequest, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return new Response<bool>(ResponseCode.Success, true, "Email confirmed successfully.");
        }
        public async Task<Response<string>> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new Response<string>(ResponseCode.NotFound, "User not found.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return new Response<string>(ResponseCode.Success, token, "Password reset token generated successfully.");
        }
        public async Task<Response<bool>> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new Response<bool>(ResponseCode.NotFound, "User not found.");
            }

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                return new Response<bool>(ResponseCode.BadRequest, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return new Response<bool>(ResponseCode.Success, true, "Password reset successfully.");
        }
        private string GenerateJwtToken(User user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
               new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
               new Claim(ClaimTypes.Email, user.Email),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JWT:ExpiresInMinutes"] ?? "60"));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}