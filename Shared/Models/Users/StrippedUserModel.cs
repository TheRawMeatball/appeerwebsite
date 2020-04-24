namespace csharpwebsite.Shared.Models.Users
{
  public class StrippedUserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public bool Admin { get; set; }
        public bool Instructor { get; set; }
        public int Grade { get; set; }

        public string FullName { get => FirstName + " " + LastName; }
    }
}