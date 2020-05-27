using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace csharpwebsite.Server.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Username { get; set; }
        public string AvatarPath {get; set; }
        public int Grade { get; set; }

        public bool Admin { get; set; }
        public bool Instructor { get; set; }

        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public List<Note> Notes { get; set; }
        public List<Question> Questions { get; set; }
        
        [InverseProperty("Host")]
        public List<Session> HostedSessionSlots { get; set; }

        public List<SessionAttendance> AttendedSessions { get; set; }
    }
}