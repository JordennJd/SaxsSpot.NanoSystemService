using Gridify;
using SaxsSpot.Core.Contracts.Services;
using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Application.Interfaces;

public interface INanoSystemSeriesStorage : IGenericStorage<NanosystemSeries>
{
    Task<Paging<NanosystemSeries>> Gridify(GridifyQuery query);
    
    Task DeleteAsync(NanosystemSeries entity);
}