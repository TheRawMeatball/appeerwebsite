using csharpwebsite.Shared.Models.Notes;
using csharpwebsite.Shared.Models.Questions;

namespace csharpwebsite.Shared.Models.Users
{
  public class UserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public bool Admin { get; set; }
        public bool Instructor { get; set; }
        public int Grade { get; set; }
        public StrippedNoteModel[] noteModels { get; set; }
        public StrippedQuestionModel[] questionModels { get; set; }
    }
}