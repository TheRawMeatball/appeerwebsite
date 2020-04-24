using System;
using System.Collections.Generic;


namespace csharpwebsite.Server.Entities
{
    public class SessionAttendance
    {
        public int Id { get; set; }
        public int AttendeeId { get; set; }
        public User Attendee { get; set; }

        public int SessionId { get; set; }
        public Session Session { get; set; }
    }
}