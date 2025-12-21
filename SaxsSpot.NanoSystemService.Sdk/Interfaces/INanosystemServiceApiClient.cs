using Gridify;
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
    
    /// <summary>
    /// Get radial analysis list by gridify query parameters
    /// </summary>
    /// <param name="page">Page number (starts from 1)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="sortBy">Sort by field (e.g., "inputDate" or "-inputDate" for descending)</param>
    /// <param name="filter">Filter expression (e.g., "layerCount>5")</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ResultDto<Paging<RadialAnalysisDto>>> GetRadialAnalysisList(
        int? page = null, 
        int? pageSize = null, 
        string? sortBy = null, 
        string? filter = null, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Download radial analysis file by id
    /// </summary>
    /// <param name="id">Radial analysis id</param>
    /// <param name="cancellationToken"></param>
    /// <returns>File stream</returns>
    Task<Stream> DownloadRadialAnalysis(Guid id, CancellationToken cancellationToken = default);
}