using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace csharpwebsite.Server.Entities
{    public class Reply
    {
        public int Id { get; set; }
        [Required]
        public User Author { get; set; }
        public int AuthorId { get; set; }
        [Required]
        public string Content { get; set; }
        public List<Reply> Replies { get; set; }


        public int? ReplyId { get; set; }
        public Reply ParentReply { get; set; }
        public int? TopQuestionId { get; set; }
        public Question TopQuestion { get; set; }
        public int? TopNoteId { get; set; }
        public Note TopNote { get; set; }
    }
}