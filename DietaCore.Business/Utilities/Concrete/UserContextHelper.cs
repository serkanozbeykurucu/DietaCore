using DietaCore.Business.Utilities.Abstract;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DietaCore.Business.Utilities.Concrete
{
    public class UserContextHelper : IUserContextHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public int GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");

            if (!int.TryParse(userId, out int parsedUserId))
                throw new InvalidOperationException("Invalid user ID format");

            return parsedUserId;
        }
    }
}
