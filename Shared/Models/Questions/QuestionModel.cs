using System;
using System.Collections.Generic;
using csharpwebsite.Shared.Models.Users;

namespace csharpwebsite.Shared.Models.Questions
{
    public class QuestionModel : IForumObject
    {
        public Guid Id { get; set; }
        public string Source { get; set; }
        public int? Page { get; set; }
        public Subject Subject { get; set; }
        public int Grade { get; set; }
        public StrippedUserModel Author { get; set; }

        public string DisplayTitle => $"{Source}, page {Page}";
    }
}