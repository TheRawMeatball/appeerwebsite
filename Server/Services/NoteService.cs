using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using csharpwebsite.Server.Entities;
using csharpwebsite.Server.Helpers;
using csharpwebsite.Shared.Models;

namespace csharpwebsite.Server.Services
{
    public interface INoteService
    {
        Task<Note> GetById(Guid id);
        Task<Guid[]> GetByQuery(Subject subject, int grade);
        Task<Note> GetNoteWithRepliesById(Guid id);
        Task<Note> Create(Note note);
        Task Update(Note note, Guid userId);
        Task Delete(Guid id, Guid userId);
    }

    public class NoteService : INoteService
    {
        private DataContext _context;
        private IImageService _imageService;

        public NoteService(DataContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public Task<Note> GetById(Guid id)
        {
            return _context.Notes
            .Where(x => x.Id == id)
            .Include(x => x.Author)
            .FirstOrDefaultAsync();
        }

        public Task<Guid[]> GetByQuery(Subject subject, int grade)
        {
            return _context.Notes
            .Where(x => ((subject == Subject.All) || (x.Subject == subject)) && (x.Grade == grade || grade > 12))
            .Select(x => x.Id)
            .ToArrayAsync();
        }

        public async Task<Note> GetNoteWithRepliesById(Guid id)
        {
            return await _context.Notes
            .Where(x => x.Id == id)
            .Include(x => x.Replies)
            .FirstOrDefaultAsync();
        }

        public async Task<Note> Create(Note note)
        {                        
            if (note.Subject == Subject.All)
            {
                throw new AppException("Not a valid subject.");
            }


            await _context.Notes.AddAsync(note);
            await _context.SaveChangesAsync();

            return note;
        }

        public async Task Update(Note noteParam, Guid userId)
        {
            var note = await _context.Notes.FindAsync(noteParam.Id);

            if (note == null) {
                throw new AppException("Note not found");
            }

            if (userId != note.AuthorId && userId != Guid.Empty)
            {
                throw new AppException("Unauthorized.");
            }

            // update name if it has changed
            if (!string.IsNullOrWhiteSpace(noteParam.Name) && noteParam.Name != note.Name)
            {            
                note.Name = noteParam.Name;
            }

            // update content if provided
            if (!string.IsNullOrWhiteSpace(noteParam.Content)){
                note.Content = noteParam.Content;
            }

            _context.Notes.Update(note);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id, Guid userId)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note != null)
            {
                if (note.AuthorId != userId && userId != Guid.Empty)
                {
                    throw new AppException("Unauthorized.");
                }
                _context.Notes.Remove(note);
                _context.SaveChanges();
            }
            else
            {
                throw new AppException("Note not found");
            }
        }

    }
}