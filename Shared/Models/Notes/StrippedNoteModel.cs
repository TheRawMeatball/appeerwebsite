using System;

namespace csharpwebsite.Shared.Models.Notes 
{
    public class StrippedNoteModel 
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public Subject Subject { get; set; }
        public int Grade { get; set; }
    }
}