using SaxsSpot.NanoSystemService.Contracts.Models;
using SaxsSpot.Shared.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Sdk.Interfaces;

/// <summary>
/// interface for access to nanosystem service API
/// </summary>
public interface INanosystemServiceApiClient
{
    /// <summary>
    /// Get nanosystems by gridify query
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ResultDto<IEnumerable<NanosystemDto>>> GetNanosystemList(ApiQuery query, CancellationToken cancellationToken = default);
}