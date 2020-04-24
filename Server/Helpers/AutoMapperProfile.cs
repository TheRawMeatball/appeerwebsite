using System.Linq;
using AutoMapper;
using csharpwebsite.Server.Entities;
using csharpwebsite.Shared.Models.Notes;
using csharpwebsite.Shared.Models.Questions;
using csharpwebsite.Shared.Models.Replies;
using csharpwebsite.Shared.Models.Sessions;
using csharpwebsite.Shared.Models.Users;

namespace csharpwebsite.Server.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<RegisterModel, User>();
            CreateMap<UpdateModel, User>();
            CreateMap<User, StrippedUserModel>();

            CreateMap<Note, NoteModel>();
            CreateMap<NoteModel, Note>();
            CreateMap<CreateModel, Note>();
            CreateMap<Note, StrippedNoteModel>();

            CreateMap<AskModel, Question>();
            CreateMap<Question, QuestionModel>();
            CreateMap<QuestionModel, Question>();
            CreateMap<Question, StrippedQuestionModel>();
            CreateMap<FindModel, Question>();

            CreateMap<ReplyRecieveModel, Reply>();
            CreateMap<Reply, ReplyModel>();

            CreateMap<Session, SessionModel>()
                .ForMember(dest => dest.Attendees, opt => opt.MapFrom(src => src.Attendees.Select(x => x.Attendee)));
            CreateMap<HostSessionModel, Session>();
        }
    }
}