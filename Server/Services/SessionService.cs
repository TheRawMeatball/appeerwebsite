using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using csharpwebsite.Server.Entities;
using csharpwebsite.Server.Helpers;
using csharpwebsite.Shared.Models;
using csharpwebsite.Shared.Models.Sessions;

namespace csharpwebsite.Server.Services
{
    public interface ISessionService
    {
        Task Delete(int id, int userId);
        Task<Session[]> GetSessions(SessionFinderModel finder);
        Task HostSession(Session sessionToHost);
        Task Leave(int id, int userId);
        Task Attend(int id, int userId);
    }

    public class SessionService : ISessionService
    {
        private DataContext _context;

        public SessionService(DataContext context)
        {
            _context = context;
        }

        public async Task Attend(int id, int userId)
        {
            var session = await _context.SessionSlots
            .Where(x => x.Id == id)
            .Where(x => x.Attendees.Find(a => a.AttendeeId == id) == null)
            .Where(x => x.Attendees.Count < x.MaxAttendees)
            .FirstOrDefaultAsync();

            _ = session ?? throw new AppException("Session not found or already full.");

            session.Attendees.Add(new SessionAttendance { AttendeeId = userId, SessionId = id });
            _context.SessionSlots.Update(session);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id, int userId)
        {
            var session = await _context.SessionSlots
            .Where(x => x.Id == id)
            .Where(x => x.HostId == userId || userId < 0)
            .FirstOrDefaultAsync();

            _ = session ?? throw new AppException("Session not found or unauthorized.");

            _context.Remove(session);
            await _context.SaveChangesAsync();
        }

        public async Task<Session[]> GetSessions(SessionFinderModel finder)
        {
            if (finder.End - finder.Start < TimeSpan.FromDays(32))
            {
                return await _context.SessionSlots
                .Where(x => x.Start > finder.Start)
                .Where(x => x.End < finder.End)
                .Where(x => (x.Attendees.Count < x.MaxAttendees) || finder.GetBooked)
                .Where(x => (x.Subjects & finder.Subjects) > 0)
                .Where(x => x.HostId == finder.HostId || finder.HostId == null)
                .Where(x => x.Grade == finder.Grade)
                .Include(x => x.Host)
                .Include(x => x.Attendees)
                    .ThenInclude(x => x.Attendee)
                .ToArrayAsync();
            }
            else
            {
                throw new AppException("Requested timespan too large.");
            }
        }

        public async Task HostSession(Session sessionToHost)
        {
            var overlap = await _context.SessionSlots
            .Where(x => x.HostId == sessionToHost.HostId)
            .Where(x => x.Start < sessionToHost.End)
            .Where(x => sessionToHost.Start < x.End)
            .CountAsync() > 0;
            
            //bool overlap = x.start < b.end && b.start < x.end;

            if (overlap)
            {
                throw new AppException("Multiple sessions intersect");
            }

            if (sessionToHost.Start.Date != sessionToHost.End.Date)
            {
                throw new AppException("Crossing day boundaries");
            }

            if (sessionToHost.End - sessionToHost.Start > TimeSpan.FromHours(10))
            {
                throw new AppException("Session too large");
            }

            _ = sessionToHost.Subjects == 0 ? throw new AppException("Select a subject.") : 0;

            await _context.SessionSlots.AddAsync(sessionToHost);
            await _context.SaveChangesAsync();
        }

        public async Task Leave(int id, int userId)
        {
            var a = await _context.SessionSlots
            .Where(x => x.Id == id)
            .Select(x => new { sa = x.Attendees.Find(a => a.AttendeeId == userId), session = x })
            .FirstOrDefaultAsync();

            var session = a.session;
            var attendance = a.sa;

            _ = session ?? throw new AppException("Session not found.");
            _ = attendance ?? throw new AppException("Session not attended by you.");

            session.Attendees.Remove(attendance);
            _context.SessionSlots.Update(session);
            await _context.SaveChangesAsync();
        }
    }
}