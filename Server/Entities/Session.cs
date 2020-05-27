using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using csharpwebsite.Shared.Models;

namespace csharpwebsite.Server.Entities
{
    public class Session
    {
        public Guid Id { get; set; }
        [Required]
        public User Host { get; set; }
        public Guid HostId { get; set; }
        public List<SessionAttendance> Attendees { get; set; }

        public int MaxAttendees { get; set; } = 1;

        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        [Required]
        public ushort Subjects { get; set; }

        public int Grade { get; set; }
        public string Description { get; set; } = "";
    }
}