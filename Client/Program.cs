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

namespace csharpwebsite.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddOptions();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddSingleton<JSLogger>();
            builder.Services.AddSingleton<State>();
            builder.Services.AddSingleton<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<DateTimeFormatInfo>();

            builder.RootComponents.Add<App>("app");

            builder.Services.AddBaseAddressHttpClient();

            await builder
                .Build()
                .UseLocalTimeZone()
                .RunAsync();
        }

    }
}
