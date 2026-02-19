using Gridify;
using SaxsSpot.Core.Contracts.Services;
using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Application.Interfaces;

public interface IRadialAnalysisStorage : IGenericStorage<RadialAnalysis>
{
    Task<Paging<RadialAnalysis>> Gridify(GridifyQuery query);
    
    Task DeleteRangeAsync(IEnumerable<RadialAnalysis> entities);
}