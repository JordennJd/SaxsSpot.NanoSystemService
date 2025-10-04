using Microsoft.Extensions.DependencyInjection;
using SaxsSpot.NanoSystemService.Sdk.Clients;
using SaxsSpot.NanoSystemService.Sdk.Interfaces;
using SaxsSpot.Shared.Contracts.Interfaces;

namespace SaxsSpot.NanoSystemService.Sdk.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddNanoSystemServices(this IServiceCollection services, string baseUrl)
    {
        services.AddScoped<INanosystemServiceApiClient>(provider => new NanosystemServiceApiClient(baseUrl, provider.GetRequiredService<IAuthenticator>()));

        return services;
    }
}