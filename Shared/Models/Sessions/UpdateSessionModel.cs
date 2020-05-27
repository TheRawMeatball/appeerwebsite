using System;
using System.ComponentModel.DataAnnotations;

namespace csharpwebsite.Shared.Models.Sessions
{
    public class UpdateSessionModel
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public ushort? Subjects { get; set; }
        public int? MaxAttendees { get; set; }
        public string Description { get; set; }
    }
}