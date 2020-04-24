using BlazorInputFile;
using csharpwebsite.Shared;
using csharpwebsite.Shared.Models.Users;
using System.Threading.Tasks;

namespace csharpwebsite.Client.Services
{
    public interface IAuthService
    {
        Task<AuthUserModel> Login(AuthenticateModel loginModel);
        Task Logout();
        Task<AuthUserModel> Register(RegisterModel registerModel, IFileListEntry avatar);
    }
}
