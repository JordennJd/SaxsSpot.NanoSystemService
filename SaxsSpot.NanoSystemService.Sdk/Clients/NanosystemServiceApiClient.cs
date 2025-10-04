using Flurl;
using Flurl.Http;
using SaxsSpot.NanoSystemService.Contracts.Models;
using SaxsSpot.NanoSystemService.Sdk.Interfaces;
using SaxsSpot.Shared.Contracts.Interfaces;
using SaxsSpot.Shared.Contracts.Models;
namespace SaxsSpot.NanoSystemService.Sdk.Clients;

public class NanosystemServiceApiClient(string baseUrl, IAuthenticator authenticator) : INanosystemServiceApiClient
{
    public async Task<ResultDto<IEnumerable<NanosystemDto>>> GetNanosystemList(ApiQuery query,
        CancellationToken cancellationToken)
    {
        var request = baseUrl
            .AppendPathSegments("api", "nanosystem")
            .AppendQueryParam(query);

        var result = await request
            .WithOAuthBearerToken(await authenticator.GetAccessTokenAsync(cancellationToken))
            .GetJsonAsync<ResultDto<IEnumerable<NanosystemDto>>>(cancellationToken: cancellationToken);

        return result;
    }
}