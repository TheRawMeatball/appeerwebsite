using csharpwebsite.Shared.Models.Users;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BlazorInputFile;
using System.IO;
using System.Collections.Generic;

namespace csharpwebsite.Client.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;
        private readonly State _state;

        public AuthService(HttpClient httpClient,
                           AuthenticationStateProvider authenticationStateProvider,
                           ILocalStorageService localStorage,
                           State state)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
            _state = state;
        }

        public async Task<AuthUserModel> Register(RegisterModel registerModel, IFileListEntry avatar)
        {
            var req = new MultipartFormDataContent();
            
            if (avatar != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    await avatar.Data.CopyToAsync(ms);
                    req.Add(new ByteArrayContent(ms.GetBuffer()), "avatar", avatar.Name);
                }
            }

            req.Add(new StringContent(registerModel.Username), "username");
            req.Add(new StringContent(registerModel.FirstName), "FirstName");
            req.Add(new StringContent(registerModel.LastName), "LastName");
            req.Add(new StringContent(registerModel.Password), "Password");
            req.Add(new StringContent(registerModel.Grade.ToString()), "Grade");

            var response = await _httpClient.PostAsync("api/users/register", req);
            var result = JsonSerializer.Deserialize<AuthUserModel>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!response.IsSuccessStatusCode)
            {
                return result;
            }
            return await SetAuthState(result);
        }

        public async Task<AuthUserModel> Login(AuthenticateModel loginModel)
        {
            var loginAsJson = JsonSerializer.Serialize(loginModel);
            var response = await _httpClient.PostAsync("api/users/authenticate", new StringContent(loginAsJson, Encoding.UTF8, "application/json"));
            var responseMessage = await response.Content.ReadAsStringAsync();
            var loginResult = JsonSerializer.Deserialize<AuthUserModel>(responseMessage, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!response.IsSuccessStatusCode)
            {
                return loginResult;
            }
            return await SetAuthState(loginResult);
        }

        private async Task<AuthUserModel> SetAuthState (AuthUserModel result)
        {
            lock (_state)
            {
                _state.User = new UserModel {
                    Id = result.Id,
                    FirstName = result.FirstName,
                    LastName = result.LastName,
                    Username = result.Username,
                    Grade = result.Grade,
                    Admin = result.Admin,
                    Instructor = result.Instructor
                };
            }
            await _localStorage.SetItemAsync("userId", result.Id);
            await _localStorage.SetItemAsync("authToken", result.Token);
            await _localStorage.SetItemAsync("authTokenExpiry", result.Expiry);
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(result.Username);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Token);

            return result;
        } 

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("authTokenExpiry");
            lock (_state)
            {
                _state.User = null;
            }
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
