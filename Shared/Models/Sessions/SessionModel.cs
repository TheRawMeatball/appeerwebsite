using System;
using System.Collections.Generic;
using csharpwebsite.Shared.Models.Users;

namespace csharpwebsite.Shared.Models.Sessions
{
    public class SessionModel
    {
        public int Id { get; set; }
        public StrippedUserModel Host { get; set; }
        public List<StrippedUserModel> Attendees { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public ushort Subjects { get; set; }
        public int MaxAttendees { get; set; }
        public int Grade { get; set; }
        public string Description { get; set; }

        // override object.Equals
        public override bool Equals(object obj)
        {
            
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            // TODO: write your implementation of Equals() here
            if (((SessionModel)obj).Id == Id)
            {
                return true;
            }
            return false;
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            return (Id << 2 * 5) >> 2;
        }
    }
}