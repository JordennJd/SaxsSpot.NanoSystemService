using Gridify;
using SaxsSpot.Core.Contracts.Services;
using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Storage.Contracts;

public interface INanoSystemStorage : IGenericStorage<Nanosystem>
{
    Task<Paging<Nanosystem>> Gridify(GridifyQuery query);
}