using SaxsSpot.Core.Contracts.Services;
using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Application.Interfaces;

public interface IRadialAnalysisLayerStorage : IGenericStorage<RadialAnalysisLayer>
{
    Task AddRangeAsync(IEnumerable<RadialAnalysisLayer> layers, CancellationToken cancellationToken = default);
}
