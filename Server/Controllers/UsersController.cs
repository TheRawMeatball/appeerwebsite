﻿using System;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using csharpwebsite.Server.Helpers;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using csharpwebsite.Server.Services;
using csharpwebsite.Server.Entities;
using csharpwebsite.Shared.Models.Users;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using csharpwebsite.Shared.Models.Questions;
using csharpwebsite.Shared.Models.Notes;

namespace csharpwebsite.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController(
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]AuthenticateModel model)
        {
            var user = await _userService.Authenticate(model.Username, model.Password);

            if (user == null){
                return BadRequest(new AuthUserModel{Error = "Username or password incorrect."});
            }

            // return basic user info and authentication token
            return Ok(AuthMapper(user));
        }

        private string GenerateJWT(string id, DateTime expiry, string[] roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_appSettings.Secret);

            var claimList = new List<Claim>();

            claimList.Add(new Claim(ClaimTypes.Name, id));

            foreach (var item in roles)
            {
                if (item != null) { claimList.Add(new Claim(ClaimTypes.Role, item)); }
            }

            var token = new JwtSecurityToken(
                _appSettings.Issuer,
                _appSettings.Audience,
                claimList,
                expires: expiry.Subtract(new TimeSpan(0,5,0)),
                signingCredentials:  new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );

            return tokenHandler.WriteToken(token);
        }

        private AuthUserModel AuthMapper(User user)
        {
            return new AuthUserModel
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Grade = user.Grade,
                Token = GenerateJWT(user.Id.ToString(), DateTime.Now.AddDays(5), new string[] 
                {
                    user.Admin ? "Admin" : null,
                    user.Instructor ? "Instructor" : null
                }),
                Expiry = DateTime.Now.AddDays(5),
                Admin = user.Admin,
                Instructor = user.Instructor
            };
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm]RegisterModel model)
        {
            // map model to entity
            var user = _mapper.Map<User>(model);

            try
            {
                // create user
                await _userService.Create(user, model.Password, Request.Form.Files);
                return Ok(AuthMapper(user));
            }
            catch (AppException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetById(id);
            var model = _mapper.Map<UserModel>(user);

            return Ok(model);
        }

        [HttpGet("{id}/questions")]
        public async Task<IActionResult> GetQuestionsById(int id)
        {
            var user = await _userService.GetQuestionsById(id);
            var model = _mapper.Map<List<StrippedQuestionModel>>(user);

            return Ok(model);
        }

        [HttpGet("{id}/notes")]
        public async Task<IActionResult> GetNotesById(int id)
        {
            var user = await _userService.GetNotesById(id);
            var model = _mapper.Map<List<StrippedNoteModel>>(user);

            return Ok(model);
        }

        [HttpGet("{id}/avatar")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvatarById(int id)
        {
            var path = await _userService.GetAvatarPathById(id);

            if (path is null)
            {
                return NotFound();
            }
            
            return PhysicalFile(path, "image/" + Path.GetExtension(path).Substring(1));
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm]UpdateModel model)
        {
            if (id.ToString() != User.Identity.Name && !User.IsInRole("Admin"))
            {
                return Unauthorized(new {message = "You can only update your account."});
            }

            // map model to entity and set id
            var user = _mapper.Map<User>(model);
            user.Id = id;

            try
            {
                // update user 
                await _userService.Update(user, model.Password, Request.Form.Files);
                return Ok(new string[0]);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new string[] { ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id.ToString() != HttpContext.User.Identity.Name && !User.IsInRole("Admin"))
            {
                return Unauthorized(new {message = "You can only delete your account."});
            }

            await _userService.Delete(id);
            return Ok();
        }

        [HttpPatch("{id}/makeadmin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MakeAdmin(int id)
        {
            await _userService.MakeAdmin(id);
            return Ok();
        }

        [HttpPatch("{id}/makeinstructor")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MakeInstructor(int id)
        {
            await _userService.MakeInstructor(id);
            return Ok();
        }
    }
}