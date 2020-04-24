using System.ComponentModel.DataAnnotations;

namespace csharpwebsite.Shared.Models.Questions 
{
    public class AskModel 
    {
        [Required]
        public Subject Subject { get; set; }
        public string Source { get; set; }
        public int? Page { get; set; }
    }
}