using System;
using System.Collections.Generic;
using csharpwebsite.Shared.Models.Users;

namespace csharpwebsite.Shared.Models.Replies 
{
    public class ReplyModel 
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public StrippedUserModel Author { get; set; }
        public List<ReplyModel> Replies { get; set; } 
    }
}