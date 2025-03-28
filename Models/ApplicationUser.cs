using Microsoft.AspNetCore.Identity;

namespace VideoConferenceApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ProfilePictureUrl { get; set; }
    }
} 