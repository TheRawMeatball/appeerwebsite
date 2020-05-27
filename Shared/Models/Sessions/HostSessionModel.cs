using System;
using System.ComponentModel.DataAnnotations;

namespace csharpwebsite.Shared.Models.Sessions
{
    public class HostSessionModel
    {
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        [Required]
        public ushort Subjects { get; set; }
        [Required]
        public int MaxAttendees { get; set; }
        public string Description { get; set; } = "";
    }
}