using System;
using Blazored.LocalStorage.JsonConverters;
using Blazored.LocalStorage.StorageOptions;
using Microsoft.Extensions.DependencyInjection;

namespace Blazored.LocalStorage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBlazoredLocalStorageSingleton(this IServiceCollection services)
        {
            return services
                .AddSingleton<ILocalStorageService, LocalStorageService>()
                .AddSingleton<ISyncLocalStorageService, LocalStorageService>()
                .Configure<LocalStorageOptions>(configureOptions =>
                {
                    configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
                });
        }

        public static IServiceCollection AddBlazoredLocalStorageSingleton(this IServiceCollection services, Action<LocalStorageOptions> configure)
        {
            return services
                .AddSingleton<ILocalStorageService, LocalStorageService>()
                .AddSingleton<ISyncLocalStorageService, LocalStorageService>()
                .Configure<LocalStorageOptions>(configureOptions =>
                {
                    configure?.Invoke(configureOptions);
                    configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
                });
        }
    }
}