using Flurl;
using Flurl.Http;
using Gridify;
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
    
    public async Task<ResultDto<Paging<RadialAnalysisDto>>> GetRadialAnalysisList(
        int? page = null, 
        int? pageSize = null, 
        string? sortBy = null, 
        string? filter = null, 
        CancellationToken cancellationToken = default)
    {
        var request = baseUrl
            .AppendPathSegments("api", "radial-analysis", "get-radial-analysis-list");

        if (page.HasValue)
            request = request.SetQueryParam("page", page.Value);
        
        if (pageSize.HasValue)
            request = request.SetQueryParam("pageSize", pageSize.Value);
        
        if (!string.IsNullOrEmpty(sortBy))
            request = request.SetQueryParam("sortBy", sortBy);
        
        if (!string.IsNullOrEmpty(filter))
            request = request.SetQueryParam("filter", filter);

        var result = await request
            .WithOAuthBearerToken(await authenticator.GetAccessTokenAsync(cancellationToken))
            .GetJsonAsync<ResultDto<Paging<RadialAnalysisDto>>>(cancellationToken: cancellationToken);

        return result;
    }
    
    public async Task<Stream> DownloadRadialAnalysis(Guid id, CancellationToken cancellationToken = default)
    {
        var request = baseUrl
            .AppendPathSegments("api", "radial-analysis", "download-radial-analysis")
            .SetQueryParam("id", id);

        var result = await request
            .WithOAuthBearerToken(await authenticator.GetAccessTokenAsync(cancellationToken))
            .GetStreamAsync(cancellationToken: cancellationToken);

        return result;
    }
}