using Gridify;
using SaxsSpot.Core.Contracts.Services;
using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Application.Interfaces;

public interface INanoSystemStorage : IGenericStorage<Nanosystem>
{
    Task<Paging<Nanosystem>> Gridify(GridifyQuery query);

    Task<IEnumerable<Nanosystem>> WhereByGridifyStringAsync(string filter);
    
    Task DeleteAsync(Nanosystem entity);
}