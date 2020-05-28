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
            CreateMap<User, UserModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToBase64()));
            CreateMap<User, AuthUserModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToBase64()));
            CreateMap<RegisterModel, User>();
            CreateMap<UpdateModel, User>();
            CreateMap<User, StrippedUserModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToBase64()));

            CreateMap<Note, NoteModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToBase64()));
            CreateMap<NoteModel, Note>();
                //.ForMember(dest => dest.Id, )
            CreateMap<CreateModel, Note>();
            CreateMap<Note, StrippedNoteModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToBase64()));

            CreateMap<AskModel, Question>();
            CreateMap<Question, QuestionModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToBase64()));
            CreateMap<QuestionModel, Question>();
            CreateMap<Question, StrippedQuestionModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToBase64()));
            CreateMap<FindModel, Question>();

            CreateMap<ReplyRecieveModel, Reply>();
            CreateMap<Reply, ReplyModel>() 
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToBase64()));

            CreateMap<Session, SessionModel>()
                .ForMember(dest => dest.Attendees, opt => opt.MapFrom(src => src.Attendees.Select(x => x.Attendee)))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToBase64()));
            CreateMap<HostSessionModel, Session>();
        }
    }
}