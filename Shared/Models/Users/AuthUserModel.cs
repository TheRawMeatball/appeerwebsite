using System;
using System.ComponentModel.DataAnnotations;

namespace csharpwebsite.Shared.Models.Users
{
    public class AuthUserModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public int Grade { get; set; }
        public bool Admin { get; set; }
        public bool Instructor { get; set; }
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
        public string Error { get; set; }
    }
}