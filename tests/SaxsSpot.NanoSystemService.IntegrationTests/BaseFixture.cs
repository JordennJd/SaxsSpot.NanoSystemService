using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaxsSpot.NanoSystemService.Sdk.Clients;
using SaxsSpot.NanoSystemService.Sdk.Extensions;
using SaxsSpot.NanoSystemService.Sdk.Interfaces;
using SaxsSpot.Shared.Authenticator.Extensions;
using SaxsSpot.Shared.Contracts.Interfaces;

namespace SaxsSpot.NanoSystemService.IntegrationTests;

[TestFixture]
public abstract class BaseFixture
{
    protected IServiceProvider _serviceProvider { get; set; }
    
    [OneTimeSetUp]
    public virtual void OneTimeSetUp()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
        
        
        var services = new ServiceCollection();

        services.AddAuthenticator(configuration);
        services.AddNanoSystemServices(configuration.GetValue<string>("baseUrl"));
        
        _serviceProvider = services.BuildServiceProvider();
    }

    [OneTimeTearDown]
    public virtual void OneTimeTearDown()
    {
        (_serviceProvider as IDisposable)?.Dispose();
    }
}