using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VideoConferenceApp.Models;
using VideoConferenceApp.Services;

namespace VideoConferenceApp.Controllers
{
    [Authorize]
    public class MeetingController : Controller
    {
        private readonly MeetingService _meetingService;
        private readonly UserManager<ApplicationUser> _userManager;

        public MeetingController(MeetingService meetingService, UserManager<ApplicationUser> userManager)
        {
            _meetingService = meetingService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var meetings = await _meetingService.GetUserMeetingsAsync(userId);
            return View(meetings);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, string description)
        {
            if (string.IsNullOrEmpty(name))
            {
                ModelState.AddModelError("", "Meeting name is required");
                return View();
            }

            var userId = _userManager.GetUserId(User);
            var meeting = await _meetingService.CreateMeetingAsync(name, description, userId);
            
            return RedirectToAction(nameof(Join), new { id = meeting.Id });
        }

        public async Task<IActionResult> Join(int id)
        {
            var meeting = await _meetingService.GetMeetingByIdAsync(id);
            if (meeting == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var participant = await _meetingService.JoinMeetingAsync(id, userId);
            
            ViewBag.ChannelName = meeting.ChannelName;
            ViewBag.Token = participant.ParticipantToken;
            ViewBag.UserId = userId;
            ViewBag.MeetingId = id;
            ViewBag.MeetingName = meeting.Name;
            ViewBag.AppId = HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Agora:AppId"];
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Leave(int id)
        {
            var userId = _userManager.GetUserId(User);
            await _meetingService.LeaveMeetingAsync(id, userId);
            
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> End(int id)
        {
            var userId = _userManager.GetUserId(User);
            await _meetingService.EndMeetingAsync(id, userId);
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Active()
        {
            var meetings = await _meetingService.GetActiveMeetingsAsync();
            return View(meetings);
        }

        public async Task<IActionResult> Details(int id)
        {
            var meeting = await _meetingService.GetMeetingByIdAsync(id);
            if (meeting == null)
            {
                return NotFound();
            }

            var participants = await _meetingService.GetMeetingParticipantsAsync(id);
            ViewBag.Participants = participants;
            
            return View(meeting);
        }
    }
} 