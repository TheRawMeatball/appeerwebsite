using System.ComponentModel.DataAnnotations;

namespace csharpwebsite.Shared.Models.Notes 
{
    public class CreateModel 
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public Subject Subject { get; set; }
    }
}