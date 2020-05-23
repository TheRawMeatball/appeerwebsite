using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using csharpwebsite.Client.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Globalization;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using System.Net.Http;

namespace csharpwebsite.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services
            .AddSingleton<State>()
            .AddOptions()
            .AddBlazoredLocalStorageSingleton()
            .AddAuthorizationCore()
            .AddSingleton<JSLogger>()
            .AddSingleton<AuthenticationStateProvider, ApiAuthenticationStateProvider>()
            .AddSingleton<IAuthService, AuthService>()
            .AddSingleton<DateTimeFormatInfo>()
            .AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.RootComponents.Add<App>("app");


            await builder
                .Build()
                .UseLocalTimeZone()
                .RunAsync();
        }

    }
}
