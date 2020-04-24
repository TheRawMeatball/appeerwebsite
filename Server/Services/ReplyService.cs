using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using csharpwebsite.Server.Entities;
using csharpwebsite.Server.Helpers;

namespace csharpwebsite.Server.Services
{
    public interface IReplyService
    {
        Task<Reply> GetById(int id);
        Task<Reply> GetNestedRepliesById(int id);
        Task<List<Reply>> GetNoteRepliesById(int id);
        Task<List<Reply>> GetQuestionRepliesById(int id);
        Task<Reply> Reply(List<Reply> target, Reply reply);
        Task Update(Reply reply);
        Task Delete(int id, int userId);
    }

    public class ReplyService : IReplyService
    {
        private DataContext _context;
        private IImageService _imageService;

        public ReplyService(DataContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<Reply> GetById(int id)
        {
            var reply = await _context.Replies
            .Include(x => x.Replies)
            .FirstOrDefaultAsync(x => id == x.Id);

            if(reply == null) 
            {
                throw new AppException("Not Found");
            }
            else
            {
                return reply;
            }
        }

        public async Task<Reply> GetNestedRepliesById(int id)
        {
            var replies = await _context.Replies
            .Include(x => x.Replies)
            .ThenInclude(x => x.Replies)
            .ThenInclude(x => x.Replies)
            .ThenInclude(x => x.Replies)
            .ThenInclude(x => x.Replies)
            .ThenInclude(x => x.Replies)
            .FirstOrDefaultAsync(x => x.Id == id);

            if(replies == null) 
            {
                throw new AppException("Not Found");
            }
            else
            {
                return replies;
            }
        }

        public Task<List<Reply>> GetNoteRepliesById(int id)
        {
            return _context.Replies
            .Where(x => x.TopNoteId == id && x.ReplyId == null)
            .Include(x => x.Author)
            .Include(x => x.Replies)
                .ThenInclude(x => x.Author)
            .Include(x => x.Replies)
                .ThenInclude(x => x.Replies)
                    .ThenInclude(x => x.Author)
            .Include(x => x.Replies)
                .ThenInclude(x => x.Replies)
                    .ThenInclude(x => x.Replies)
                        .ThenInclude(x => x.Author)
            .Include(x => x.Replies)
                .ThenInclude(x => x.Replies)
                    .ThenInclude(x => x.Replies)
                        .ThenInclude(x => x.Replies)
                            .ThenInclude(x => x.Author)
            .Include(x => x.Replies)
                .ThenInclude(x => x.Replies)
                    .ThenInclude(x => x.Replies)
                        .ThenInclude(x => x.Replies)
                            .ThenInclude(x => x.Replies)
            .Take(10)
            .ToListAsync();
        }

        public Task<List<Reply>> GetQuestionRepliesById(int id)
        {
            return _context.Replies
            .Where(x => x.TopQuestionId == id && x.ReplyId == null)
            .Include(x => x.Author)
            .Include(x => x.Replies)
                .ThenInclude(x => x.Author)
            .Include(x => x.Replies)
                .ThenInclude(x => x.Replies)
                    .ThenInclude(x => x.Author)
            .Include(x => x.Replies)
                .ThenInclude(x => x.Replies)
                    .ThenInclude(x => x.Replies)
                        .ThenInclude(x => x.Author)
            .Include(x => x.Replies)
                .ThenInclude(x => x.Replies)
                    .ThenInclude(x => x.Replies)
                        .ThenInclude(x => x.Replies)
                            .ThenInclude(x => x.Author)
            .Include(x => x.Replies)
                .ThenInclude(x => x.Replies)
                    .ThenInclude(x => x.Replies)
                        .ThenInclude(x => x.Replies)
                            .ThenInclude(x => x.Replies)
            .ToListAsync();
        }

        public async Task<Reply> Reply(List<Reply> target, Reply reply)
        {              
            target.Add(reply);
            await _context.SaveChangesAsync();
            return reply;
        }

        public async Task Update(Reply replyParam)
        {
            var reply = await _context.Replies.FindAsync(replyParam.Id);

            if (reply == null) {
                throw new AppException("Reply not found");
            }

            if (replyParam.AuthorId != reply.AuthorId && replyParam.AuthorId > 0)
            {
                throw new AppException("Unauthorized.");
            }

            // update content if provided
            if (!string.IsNullOrWhiteSpace(replyParam.Content)){
                reply.Content = replyParam.Content;
            }

            _context.Replies.Update(reply);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id, int userId)
        {
            var reply = await _context.Replies.FindAsync(id);
            if (reply != null)
            {
                if (reply.AuthorId != userId && userId > 0)
                {
                    throw new AppException("Unauthorized.");
                }
                _context.Replies.Remove(reply);
                _context.SaveChanges();
            }
            else
            {
                throw new AppException("Reply not found");
            }
        }
    }
}