using SaxsSpot.Core.Contracts.Services;
using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Application.Interfaces;

public interface IScatteringResultObjectStorage : ICommonObjectStorage<IntensityResult>
{
    Task Delete(Guid objectId);
}
