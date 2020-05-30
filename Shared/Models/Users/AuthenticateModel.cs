using System.ComponentModel.DataAnnotations;

namespace csharpwebsite.Shared.Models.Users
{
    public class AuthenticateModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Username is needed")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is needed")]
        public string Password { get; set; }
    }
}