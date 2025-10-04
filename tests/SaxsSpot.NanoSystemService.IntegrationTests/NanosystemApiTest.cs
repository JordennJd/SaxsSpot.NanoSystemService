using Microsoft.Extensions.DependencyInjection;
using SaxsSpot.NanoSystemService.Contracts.Services;
using SaxsSpot.NanoSystemService.Sdk.Interfaces;
using SaxsSpot.Shared.Contracts.Models;

namespace SaxsSpot.NanoSystemService.IntegrationTests;

public class NanosystemApiTest : BaseFixture
{
    [Test]
    public async Task CanGetNanoSystems()
    {
        var client = _serviceProvider.GetRequiredService<INanosystemServiceApiClient>();

        var systems = await client.GetNanosystemList(new ApiQuery(""));
        
        Assert.That(true);
    }
}