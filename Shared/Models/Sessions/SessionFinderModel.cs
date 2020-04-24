using System;
using System.ComponentModel.DataAnnotations;
using csharpwebsite.Shared.Models.Users;

namespace csharpwebsite.Shared.Models.Sessions
{
    public class SessionFinderModel
    {
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        [Required]
        public ushort Subjects { get; set; }
        public bool GetBooked { get; set; }
        public int? HostId { get; set; }
        public int Grade { get; set; }
    }
}