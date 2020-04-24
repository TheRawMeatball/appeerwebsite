using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using csharpwebsite.Server.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using csharpwebsite.Server.Services;
using csharpwebsite.Server.Entities;
using System.Threading.Tasks;
using System;
using csharpwebsite.Shared.Models.Questions;
using csharpwebsite.Shared.Models.Replies;

namespace csharpwebsite.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RepliesController : ControllerBase
    {
        private IUserService _userService;
        private IReplyService _replyService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public RepliesController(
            IUserService userService,
            IReplyService replyService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _replyService = replyService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> ReplyToReply(int id, [FromForm]ReplyRecieveModel model) 
        {
            var reply = _mapper.Map<Reply>(model);
            reply.AuthorId = int.Parse(User.Identity.Name);

            try
            {   var target = await _replyService.GetById(id);
                reply.TopQuestionId = target.TopQuestionId;
                reply.TopNoteId = target.TopNoteId;
                return Ok(await _replyService.Reply(target.Replies, reply));
            } 
            catch (AppException ex)
            {
                return NotFound(new {message = ex.Message});
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> EditReply (int id, [FromForm]ReplyRecieveModel model) 
        {
            var reply = _mapper.Map<Reply>(model);
            reply.Id = id;
            reply.AuthorId = User.IsInRole("Admin") ? -1 : int.Parse(User.Identity.Name);

            try
            {
                await _replyService.Update(reply);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReply(int id)
        {
            try
            {
                var reply = await _replyService.GetNestedRepliesById(id); 
                return Ok(_mapper.Map<ReplyModel>(reply));
            }
            catch (AppException ex)
            {
                return NotFound(new {message = ex.Message});
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _replyService.Delete(id, User.IsInRole("Admin") ? -1 : int.Parse(User.Identity.Name));
            return Ok();
        }

    }

}