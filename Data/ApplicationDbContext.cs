using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VideoConferenceApp.Models;

namespace VideoConferenceApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<MeetingParticipant> MeetingParticipants { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// Configure unique token for meetings
			builder.Entity<Meeting>()
				.HasIndex(m => m.MeetingToken)
				.IsUnique();

			// Configure unique token for participants
			builder.Entity<MeetingParticipant>()
				.HasIndex(p => p.ParticipantToken)
				.IsUnique();

			// Relationship: MeetingParticipants -> Meeting
			builder.Entity<MeetingParticipant>()
				.HasOne(mp => mp.Meeting)
				.WithMany(m => m.Participants)
				.HasForeignKey(mp => mp.MeetingId)
				.OnDelete(DeleteBehavior.Restrict);  // Prevent cascading delete

			// Relationship: MeetingParticipants -> User
			builder.Entity<MeetingParticipant>()
				.HasOne(mp => mp.User)
				.WithMany()
				.HasForeignKey(mp => mp.UserId)
				.OnDelete(DeleteBehavior.Restrict);  // Prevent cascading delete
		}


	}
} 