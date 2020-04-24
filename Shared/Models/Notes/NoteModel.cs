using csharpwebsite.Shared.Models.Users;

namespace csharpwebsite.Shared.Models.Notes 
{
    public class NoteModel : IForumObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Subject Subject { get; set; }
        public string Content { get; set; }
        public int Grade { get; set; }
        public StrippedUserModel Author { get; set; }

        public string DisplayTitle => Name;
    }
}