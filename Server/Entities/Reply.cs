using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace csharpwebsite.Server.Entities
{    public class Reply
    {
        public Guid Id { get; set; }
        [Required]
        public User Author { get; set; }
        public Guid AuthorId { get; set; }
        [Required]
        public string Content { get; set; }
        public List<Reply> Replies { get; set; }


        public Guid? ReplyId { get; set; }
        public Reply ParentReply { get; set; }
        public Guid? TopQuestionId { get; set; }
        public Question TopQuestion { get; set; }
        public Guid? TopNoteId { get; set; }
        public Note TopNote { get; set; }
    }
}