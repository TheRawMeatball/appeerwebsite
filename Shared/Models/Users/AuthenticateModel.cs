using System.ComponentModel.DataAnnotations;

namespace csharpwebsite.Shared.Models.Users
{
    public class AuthenticateModel
    {
        [Required(ErrorMessage = "Username is needed")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is needed")]
        public string Password { get; set; }
    }
}