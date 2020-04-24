using csharpwebsite.Shared.Models.Users;

namespace csharpwebsite.Shared.Models
{
    public enum Subject {All, Math, Physics, Chemistry}
    public interface IForumObject
    {
        int Id { get; set; }
        string DisplayTitle { get; }
        Subject Subject { get; set; }
        int Grade { get; set; }
        StrippedUserModel Author { get; set; }
    }

    public class Error 
    {
        public string message { get; set; }
    }
}