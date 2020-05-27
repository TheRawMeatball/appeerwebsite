using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using csharpwebsite.Shared.Models;

namespace csharpwebsite.Server.Entities
{
    public class Note
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public Subject Subject { get; set; }
        public int Grade { get; set; }
        [Required]
        public User Author { get; set; }
        public Guid AuthorId { get; set; }
        [Required]
        public string Content { get; set; }
        public List<Reply> Replies { get; set; }

    }
}