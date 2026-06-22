using Microsoft.Extensions.DependencyInjection;
using SaxsSpot.NanoSystemService.Sdk.Interfaces;

namespace SaxsSpot.NanoSystemService.IntegrationTests;

public class ScatteringCalculationApiTest : BaseFixture
{
    [Test]
    public async Task CanGetScatteringCalculationList()
    {
        var client = _serviceProvider.GetRequiredService<INanosystemServiceApiClient>();
        var result = await client.GetScatteringCalculationList(page: 1, pageSize: 1);

        Assert.That(result.IsSuccess, Is.True);
    }
}
