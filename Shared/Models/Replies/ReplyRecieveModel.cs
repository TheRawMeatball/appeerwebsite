using System.ComponentModel.DataAnnotations;

namespace csharpwebsite.Shared.Models.Replies 
{
    public class ReplyRecieveModel 
    {
        [Required]
        public string Content { get; set; }
    }
}