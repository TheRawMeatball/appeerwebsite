using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using csharpwebsite.Server.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using csharpwebsite.Server.Services;
using csharpwebsite.Server.Entities;
using System.Threading.Tasks;
using System;
using csharpwebsite.Shared.Models.Sessions;

namespace csharpwebsite.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private ISessionService _sessionService;
        private readonly AppSettings _appSettings;

        public SessionController(
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            ISessionService sessionService)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _sessionService = sessionService;
        }

        int userId => int.Parse(User.Identity.Name);

        [HttpPost("{id}/attend")]
        public async Task<IActionResult> Attend(int id) 
        {
            try
            {
                await _sessionService.Attend(id, userId);
                return Ok(new { message = "" });
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/leave")]
        public async Task<IActionResult> Leave(int id) 
        {
            try
            {
                await _sessionService.Leave(id, userId);
                return Ok(new { message = "" });
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("host")]
        [Authorize(Roles = "Admin, Instructor")]
        public async Task<IActionResult> HostSession ([FromBody] HostSessionModel session) 
        {
            try
            {
                var _s = _mapper.Map<Session>(session);
                _s.HostId = userId;
                _s.Grade = (await _userService.GetById(userId)).Grade;
                await _sessionService.HostSession(_s);
                return Ok(new { message = "" });
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("availablesessions")]
        public async Task<IActionResult> GetSessions([FromBody] SessionFinderModel finder)
        {
            try
            {
                Session[] sessions = await _sessionService.GetSessions(finder, userId);
                var sessionModels = _mapper.Map<SessionModel[]>(sessions);
                return Ok(sessionModels);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Instructor")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _sessionService.Delete(id, User.IsInRole("Admin") ? -1 : userId);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }

}