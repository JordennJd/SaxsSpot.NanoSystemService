using Gridify;
using SaxsSpot.Core.Contracts.Services;
using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Application.Interfaces;

public interface IScatteringCalculationStorage : IGenericStorage<ScatteringCalculation>
{
    Task<Paging<ScatteringCalculation>> Gridify(GridifyQuery query);

    Task DeleteRangeAsync(IEnumerable<ScatteringCalculation> entities);
}
