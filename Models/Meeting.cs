using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoConferenceApp.Models
{
    public class Meeting
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        public string ChannelName { get; set; } = string.Empty;
        
        [Required]
        public string MeetingToken { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? EndedAt { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
        
        public ICollection<MeetingParticipant>? Participants { get; set; }
    }
} 