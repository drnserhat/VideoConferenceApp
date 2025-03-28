using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoConferenceApp.Models
{
    public class MeetingParticipant
    {
        [Key]
        public int Id { get; set; }
        
        public int MeetingId { get; set; }
        
        [ForeignKey("MeetingId")]
        public Meeting? Meeting { get; set; }
        
        public string UserId { get; set; } = string.Empty;
        
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
        
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? LeftAt { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public string ParticipantToken { get; set; } = string.Empty;
    }
} 