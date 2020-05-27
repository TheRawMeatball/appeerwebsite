using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using csharpwebsite.Server.Entities;
using csharpwebsite.Server.Helpers;

namespace csharpwebsite.Server.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<User> GetById(Guid id);
        Task<List<Question>> GetQuestionsById(Guid id);
        Task<List<Note>> GetNotesById(Guid id);
        Task<User> GetByUsername(string username);
        Task<User> Create(User user, string password, IFormFileCollection formFiles);
        Task Update(User user, string password = null, IFormFileCollection formFiles = null);
        Task Delete(Guid id);
        Task<string> GetAvatarPathById(Guid id);
        Task MakeInstructor(Guid id);
        Task MakeAdmin(Guid id);
    }

    public class UserService : IUserService
    {
        private DataContext _context;
        private IImageService _imageService;

        public UserService(DataContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == username);
            

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public Task<User> GetById(Guid id)
        {
            return _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<List<Question>> GetQuestionsById(Guid id)
        {
            return _context.Users.Where(x => x.Id == id)
            .Select(x => x.Questions)
            .FirstOrDefaultAsync();
        }

        public Task<List<Note>> GetNotesById(Guid id)
        {
            return _context.Users.Where(x => x.Id == id)
            .Select(x => x.Notes)
            .FirstOrDefaultAsync();
        }

        public async Task<string> GetAvatarPathById(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user is null)
            {
                return null;
            }

            return _imageService.AvatarPath + (user.AvatarPath ?? "person.svg+xml");
        }

        public Task<User> GetByUsername(string username)
        {
            return _context.Users.FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<User> Create(User user, string password, IFormFileCollection formFiles)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password)) {
                throw new AppException("Password is required");
            }

            if (await _context.Users.AnyAsync(x => x.Username == user.Username)) {
                throw new AppException("Username \"" + user.Username + "\" is already taken");
            }
            
            user.AvatarPath = await _imageService.GetSingleImage(formFiles, "avatar");
            
            if(user.AvatarPath != null)
            {   
                await _imageService.SquareImage(_imageService.AvatarPath + user.AvatarPath);
            }

            byte[]  passwordHash, passwordSalt;
   
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            _context.SaveChanges();

            return user;
        }

        public async Task Update(User userParam, string password = null, IFormFileCollection formFiles = null)
        {
            var user = await _context.Users.FindAsync(userParam.Id);

            if (user == null) {
                throw new AppException("User not found");
            }

            // update username if it has changed
            if (!string.IsNullOrWhiteSpace(userParam.Username) && userParam.Username != user.Username)
            {
                // throw error if the new username is already taken
                if (_context.Users.Any(x => x.Username == userParam.Username))
                    throw new AppException("Username " + userParam.Username + " is already taken");

                user.Username = userParam.Username;
            }

            // update user properties if provided
            if (!string.IsNullOrWhiteSpace(userParam.FirstName)){
                user.FirstName = userParam.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(userParam.LastName)){
                user.LastName = userParam.LastName;
            }

            // update password if provided
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            var f = await _imageService.GetSingleImage(formFiles, "avatar", user.AvatarPath);

            if (f != null)
            {
                await _imageService.SquareImage(_imageService.AvatarPath + f);
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                await _imageService.Delete(_imageService.AvatarPath + user.AvatarPath);
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            else
            {
                throw new AppException("User not found");
            }
        }

        // private helper methods

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        public async Task MakeInstructor(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            user.Instructor = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task MakeAdmin(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            user.Admin = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}