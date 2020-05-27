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
    public interface IQuestionService
    {
        Task<Question> GetById(Guid id);
        Task<Guid[]> GetByQuery(Subject subject, int grade);
        Task<Question> GetQuestionWithRepliesById(Guid id);
        Task<Question> Create(Question note, IFormFileCollection files);
        Task Update(Question note, Guid userId);
        Task Delete(Guid id, Guid userId);
        Task<string> GetImagePathById(Guid id);
        Task<string[]> GetSources(int grade, Subject subject, string searchString);
        Task<Question[]> Find(Question question);
    }

    public class QuestionService : IQuestionService
    {
        private DataContext _context;
        private IImageService _imageService;

        public QuestionService(DataContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public Task<Question> GetById(Guid id)
        {
            return _context.Questions
            .Where(x => x.Id == id)
            .Include(x => x.Author)
            .FirstOrDefaultAsync();
        }

        public Task<Question> GetQuestionWithRepliesById(Guid id)
        {
            return _context.Questions
            .Where(x => x.Id == id)
            .Include(x => x.Replies)
            .FirstOrDefaultAsync();
        }

        public async Task<Question> Create(Question question,IFormFileCollection files)
        {                        
            try
            {
                if (question.Subject == Subject.All)
                {
                    throw new AppException("Not a valid subject.");
                }

                question.Content = await _imageService.GetSingleImage(files, "question");
                if (question.Content == null) 
                {
                    throw new AppException("Please send an image.");
                }
            }
            catch (AppException ex)
            {
                throw ex;
            }

            await _context.Questions.AddAsync(question);
            _context.SaveChanges();

            return question;
        }

        public async Task Update(Question questionParam, Guid userId)
        {
            var question = await _context.Questions.FindAsync(questionParam.Id);

            if (question == null) {
                throw new AppException("Question not found");
            }

            if (userId != question.AuthorId)
            {
                throw new AppException("Unauthorized.");
            }


            //_context.Questions.Update(question);
            //await _context.SaveChangesAsync();
            throw new NotImplementedException();
        }

        public async Task Delete(Guid id, Guid userId)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                if (question.AuthorId != userId)
                {
                    throw new AppException("Unauthorized.");
                }
                await _imageService.Delete(_imageService.QuestionPath + question.Source);
                _context.Questions.Remove(question);
                _context.SaveChanges();
            }
            else
            {
                throw new AppException("Question not found");
            }
        }

        public async Task<string> GetImagePathById(Guid id)
        {
            return _imageService.QuestionPath + (await _context.Questions.FindAsync(id)).Content;
        }

        public Task<Guid[]> GetByQuery(Subject subject, int grade)
        {
            return _context.Questions
            .Where(x => ((subject == Subject.All) || (x.Subject == subject)) && (x.Grade == grade || grade > 12))
            .Select(x => x.Id)
            .ToArrayAsync();
        }

        public Task<string[]> GetSources(int grade, Subject subject, string searchString)
        {
            return _context.Questions
            .Where(x => x.Grade == grade)
            .Where(x => subject == Subject.All || x.Subject == subject)
            .Where(x => x.Source.ToLower().Contains(searchString.ToLower()))
            .Select(x => x.Source)
            .Distinct()
            .ToArrayAsync();
        }

        public Task<Question[]> Find(Question question)
        {
            return _context.Questions
            .Where(x => x.Grade == question.Grade)
            .Where(x => x.Page == question.Page     )
            .Where(x => question.Subject == Subject.All || x.Subject == question.Subject)
            .Where(x => x.Source.ToLower().Contains(question.Source.ToLower()))
            .ToArrayAsync();
        }
    }
}