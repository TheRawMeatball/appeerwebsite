using System;
using System.Collections.Generic;


namespace csharpwebsite.Server.Entities
{
    public class SessionAttendance
    {
        public Guid Id { get; set; }
        public Guid AttendeeId { get; set; }
        public User Attendee { get; set; }

        public Guid SessionId { get; set; }
        public Session Session { get; set; }
    }
}