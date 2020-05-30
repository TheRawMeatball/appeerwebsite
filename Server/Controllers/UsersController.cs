using System;
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
            User user;
            try
            {
                user = await _userService.Authenticate(model.Email, model.Password);
            }
            catch (AppException ex)
            {
                return BadRequest(new AuthUserModel{ Error = ex.Message });
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
            var aum = _mapper.Map<AuthUserModel>(user);

            aum.Expiry = DateTime.Now.AddDays(5);
            aum.Token = GenerateJWT(user.Id.ToString(), DateTime.Now.AddDays(5), new string[] 
            {
                user.Admin ? "Admin" : null,
                user.Instructor ? "Instructor" : null
            });

            return aum;
        }

        [AllowAnonymous]
        [HttpPost("google-signin")]
        public async Task<IActionResult> GoogleSignin([FromBody]string jwt)
        {
            try
            {
                // create user
                var user = await _userService.GoogleSignin(jwt);
                return Ok(AuthMapper(user));
            }
            catch (AppException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
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
                var _user = await _userService.Create(user, model.Password, Request.Form.Files);
                return Ok(AuthMapper(_user));
            }
            catch (AppException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetById(id);
            var model = _mapper.Map<UserModel>(user);

            return Ok(model);
        }

        [HttpGet("{id}/questions")]
        public async Task<IActionResult> GetQuestionsById(Guid id)
        {
            var user = await _userService.GetQuestionsById(id);
            var model = _mapper.Map<List<StrippedQuestionModel>>(user);

            return Ok(model);
        }

        [HttpGet("{id}/notes")]
        public async Task<IActionResult> GetNotesById(Guid id)
        {
            var user = await _userService.GetNotesById(id);
            var model = _mapper.Map<List<StrippedNoteModel>>(user);

            return Ok(model);
        }

        [HttpGet("{id}/avatar")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvatarById(Guid id)
        {
            var path = await _userService.GetAvatarPathById(id);

            if (path is null)
            {
                return NotFound();
            }
            
            return PhysicalFile(path, "image/" + Path.GetExtension(path).Substring(1));
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm]UpdateModel model)
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
        public async Task<IActionResult> Delete(Guid id)
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
        public async Task<IActionResult> MakeAdmin(Guid id)
        {
            await _userService.MakeAdmin(id);
            return Ok();
        }

        [HttpPatch("{id}/makeinstructor")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MakeInstructor(Guid id)
        {
            await _userService.MakeInstructor(id);
            return Ok();
        }
    }
}
