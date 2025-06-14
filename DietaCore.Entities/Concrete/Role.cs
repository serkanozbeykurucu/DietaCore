using Microsoft.AspNetCore.Identity;

namespace DietaCore.Entities.Concrete
{
    public class Role : IdentityRole<int>
    {
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
