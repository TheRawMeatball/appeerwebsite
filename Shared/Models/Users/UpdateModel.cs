using System.ComponentModel.DataAnnotations;

namespace csharpwebsite.Shared.Models.Users
{
  public class UpdateModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Grade { get; set; }
    }
}