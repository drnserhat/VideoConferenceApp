using Microsoft.EntityFrameworkCore;
using VideoConferenceApp.Data;
using VideoConferenceApp.Models;

namespace VideoConferenceApp.Services
{
    public class MeetingService
    {
        private readonly ApplicationDbContext _context;
        private readonly AgoraTokenService _tokenService;

        public MeetingService(ApplicationDbContext context, AgoraTokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<Meeting> CreateMeetingAsync(string name, string description, string userId)
        {
            // Generate a unique channel name
            var channelName = $"meeting_{Guid.NewGuid():N}";
            
            // Generate a token for the meeting
            var token = _tokenService.GenerateToken(channelName, userId);

            var meeting = new Meeting
            {
                Name = name,
                Description = description,
                ChannelName = channelName,
                MeetingToken = token,
                UserId = userId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            return meeting;
        }

        public async Task<MeetingParticipant> JoinMeetingAsync(int meetingId, string userId)
        {
            var meeting = await _context.Meetings
                .FirstOrDefaultAsync(m => m.Id == meetingId && m.IsActive);

            if (meeting == null)
            {
                throw new Exception("Meeting not found or is no longer active");
            }

            // Check if the user is already a participant
            var existingParticipant = await _context.MeetingParticipants
                .FirstOrDefaultAsync(p => p.MeetingId == meetingId && p.UserId == userId && p.IsActive);

            if (existingParticipant != null)
            {
                return existingParticipant;
            }

            // Generate a token for the participant
            var token = _tokenService.GenerateToken(meeting.ChannelName, userId);

            var participant = new MeetingParticipant
            {
                MeetingId = meetingId,
                UserId = userId,
                ParticipantToken = token,
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.MeetingParticipants.Add(participant);
            await _context.SaveChangesAsync();

            return participant;
        }

        public async Task<bool> LeaveMeetingAsync(int meetingId, string userId)
        {
            var participant = await _context.MeetingParticipants
                .FirstOrDefaultAsync(p => p.MeetingId == meetingId && p.UserId == userId && p.IsActive);

            if (participant == null)
            {
                return false;
            }

            participant.IsActive = false;
            participant.LeftAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EndMeetingAsync(int meetingId, string userId)
        {
            var meeting = await _context.Meetings
                .FirstOrDefaultAsync(m => m.Id == meetingId && m.UserId == userId && m.IsActive);

            if (meeting == null)
            {
                return false;
            }

            meeting.IsActive = false;
            meeting.EndedAt = DateTime.UtcNow;

            // Mark all active participants as left
            var activeParticipants = await _context.MeetingParticipants
                .Where(p => p.MeetingId == meetingId && p.IsActive)
                .ToListAsync();

            foreach (var participant in activeParticipants)
            {
                participant.IsActive = false;
                participant.LeftAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Meeting>> GetUserMeetingsAsync(string userId)
        {
            return await _context.Meetings
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Meeting>> GetActiveMeetingsAsync()
        {
            return await _context.Meetings
                .Where(m => m.IsActive)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<Meeting?> GetMeetingByIdAsync(int id)
        {
            return await _context.Meetings
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<MeetingParticipant>> GetMeetingParticipantsAsync(int meetingId)
        {
            return await _context.MeetingParticipants
                .Where(p => p.MeetingId == meetingId)
                .Include(p => p.User)
                .ToListAsync();
        }
    }
} 