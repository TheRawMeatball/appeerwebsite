using System;
using System.ComponentModel.DataAnnotations;

namespace csharpwebsite.Shared.Models.Users
{
  public class StrippedUserModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public bool Admin { get; set; }
        public bool Instructor { get; set; }
        public int Grade { get; set; }

        public string FullName { get => FirstName + " " + LastName; }
    }
}