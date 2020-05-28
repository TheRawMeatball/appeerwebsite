using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using csharpwebsite.Server.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using csharpwebsite.Server.Services;
using csharpwebsite.Server.Entities;
using System.Threading.Tasks;
using csharpwebsite.Shared.Models.Notes;
using System;
using csharpwebsite.Shared.Models.Questions;
using System.Collections.Generic;
using csharpwebsite.Shared.Models.Replies;
using csharpwebsite.Shared.Models;
using System.Linq;

namespace csharpwebsite.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private IUserService _userService;
        private INoteService _noteService;
        private IReplyService _replyService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public NotesController(
            IUserService userService,
            INoteService noteService,
            IReplyService replyService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _noteService = noteService;
            _replyService = replyService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [HttpPost("share")]
        [Authorize(Roles = "Admin, Instructor")]
        public async Task<IActionResult> ShareNote([FromForm]CreateModel model) 
        {
            var note = _mapper.Map<Note>(model);

            var author = await _userService.GetById(Guid.Parse(User.Identity.Name));

            note.Author = author;
            note.Grade = author.Grade;

            try
            {
                await _noteService.Create(note);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var note = await _noteService.GetById(id);

            if (note is null)
            {
                return NotFound();
            }

            var model = _mapper.Map<NoteModel>(note);
            return Ok(model);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm]NoteModel model) 
        {
            var note = _mapper.Map<Note>(model);
            note.Id = id;

            try
            {
                await _noteService.Update(note, User.IsInRole("Admin") ? Guid.Empty : Guid.Parse(User.Identity.Name));
                return Ok();
            }
            catch (AppException ex)
            {
                if (ex.Message == "Unauthorized.")
                {
                    return Unauthorized(new {message = "You can only update your notes."});
                }
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _noteService.Delete(id, User.IsInRole("Admin") ? Guid.Empty : Guid.Parse(User.Identity.Name));            
            }
            catch (AppException ex)
            {
                if (ex.Message == "Unauthorized.")
                {
                    return Unauthorized(new {message = "You can only delete your notes."});
                }
                return NotFound();
            }
            return Ok();
        }

        [HttpGet("{subject}/{grade}")]
        public async Task<IActionResult> GetByQuery(Subject subject, int grade)
        {
            return Ok((await _noteService.GetByQuery(subject, grade)).Select(x => x.ToBase64()));
        }

        //==================REPLY SYSTEM=====================//

        [HttpPost("{id}/reply")]
        public async Task<IActionResult> Reply(Guid id, [FromForm]ReplyRecieveModel model) 
        {
            var reply = _mapper.Map<Reply>(model);
            reply.AuthorId = Guid.Parse(User.Identity.Name);
            return Ok(await _replyService.Reply((await _noteService.GetNoteWithRepliesById(id)).Replies, reply));
        }

        [HttpGet("{id}/replies")]
        public async Task<IActionResult> GetRepliesById(Guid id) 
        {
            var replies = await _replyService.GetNoteRepliesById(id);
            var replyModels = _mapper.Map<List<ReplyModel>>(replies);
            return Ok(replyModels);
        }
    }
}