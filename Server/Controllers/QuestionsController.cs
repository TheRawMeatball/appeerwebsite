using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using csharpwebsite.Server.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using csharpwebsite.Server.Services;
using csharpwebsite.Server.Entities;
using System.Threading.Tasks;
using csharpwebsite.Shared.Models.Notes;
using csharpwebsite.Shared.Models.Questions;
using System.IO;
using System;
using csharpwebsite.Shared.Models.Replies;
using System.Collections.Generic;
using csharpwebsite.Shared.Models;

namespace csharpwebsite.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private IUserService _userService;
        private IQuestionService _questionService;
        private IReplyService _replyService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public QuestionsController(
            IUserService userService,
            IQuestionService questionService,
            IReplyService replyService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _questionService = questionService;
            _replyService = replyService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromForm]AskModel model) 
        {
            var question = _mapper.Map<Question>(model);
            
            var author = await _userService.GetById(int.Parse(User.Identity.Name));

            question.Author = author;
            question.Grade = author.Grade;

            try
            {
                await _questionService.Create(question, Request.Form.Files);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var question = await _questionService.GetById(id);

            if (question is null)
            {
                return NotFound();
            }

            var model = _mapper.Map<QuestionModel>(question);

            return Ok(model);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm]QuestionModel model) 
        {
            var question = _mapper.Map<Question>(model);
            question.Id = id;

            try
            {
                await _questionService.Update(question, int.Parse(User.Identity.Name));
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

        [HttpGet("{id}/content")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvatarById(int id)
        {
            var path = await _questionService.GetImagePathById(id);

            if (path is null)
            {
                return NotFound();
            }
            
            return PhysicalFile(path, "image/" + Path.GetExtension(path).Substring(1));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _questionService.Delete(id, int.Parse(User.Identity.Name));            
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
            return Ok(await _questionService.GetByQuery(subject, grade));
        }

        [HttpPost("find")]
        public async Task<IActionResult> Find([FromBody] FindModel model)
        {
            var question = _mapper.Map<Question>(model);
            var matchingQuestions = await _questionService.Find(question);

            return Ok(_mapper.Map<QuestionModel[]>(matchingQuestions));
        }

        [HttpGet("sources/{grade}/{subject}/{searchString}")]
        public async Task<IActionResult> GetSources(int grade, Subject subject, string searchString)
        {
            return Ok(await _questionService.GetSources(grade, subject, searchString));
        }

        //==================REPLY SYSTEM=====================//

        [HttpPost("{id}/reply")]
        public async Task<IActionResult> Reply(int id, [FromForm]ReplyRecieveModel model) 
        {
            var reply = _mapper.Map<Reply>(model);
            reply.AuthorId = int.Parse(User.Identity.Name);
            return Ok(await _replyService.Reply((await _questionService.GetQuestionWithRepliesById(id)).Replies, reply));
        }

        [HttpGet("{id}/replies")]
        public async Task<IActionResult> GetRepliesById(int id) 
        {
            var replies = await _replyService.GetQuestionRepliesById(id);
            var replyModels = _mapper.Map<List<ReplyModel>>(replies);
            return Ok(replyModels);
        }
    }
}