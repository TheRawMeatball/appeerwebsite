using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using csharpwebsite.Shared.Models;

namespace csharpwebsite.Server.Entities
{
    public class Question
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public int? Page { get; set; }
        [Required]
        public Subject Subject { get; set; }
        public int Grade { get; set; }
        [Required]
        public User Author { get; set; }
        public int AuthorId { get; set; }
        [Required]
        public string Content { get; set; }
        public List<Reply> Replies { get; set; }
    }
}