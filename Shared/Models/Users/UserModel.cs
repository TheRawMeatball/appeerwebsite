using System;
using System.ComponentModel.DataAnnotations;
using csharpwebsite.Shared.Models.Notes;
using csharpwebsite.Shared.Models.Questions;

namespace csharpwebsite.Shared.Models.Users
{
  public class UserModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public bool Admin { get; set; }
        public bool Instructor { get; set; }
        public int Grade { get; set; }
        public StrippedNoteModel[] noteModels { get; set; }
        public StrippedQuestionModel[] questionModels { get; set; }
    }
}