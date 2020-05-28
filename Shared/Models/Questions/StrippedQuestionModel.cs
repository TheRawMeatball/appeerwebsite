using System;
using System.Collections.Generic;
using csharpwebsite.Shared.Models.Users;

namespace csharpwebsite.Shared.Models.Questions
{
    public class StrippedQuestionModel 
    {
        public string Id { get; set; }
        public string Source { get; set; }
        public int? Page { get; set; }
        public Subject Subject { get; set; }
        public int Grade { get; set; }
    }
}