using SaxsSpot.NanoSystemService.Contracts.Messages;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunGeneration;

public static class RunGenerationCommandFactory
{
    public static RunGenerationCommand Create(RunGenerationRequest request)
    {
        var parameters = new CommonParticleGenerationParameters(
            request.Parameters.Count,
            request.Parameters.NumericalConcentration,
            request.Parameters.GlobalSize,
            MinSize: 1.0f,
            MaxSize: 3.0f,
            request.Parameters.Theta,
            request.Parameters.K,
            request.Parameters.Excess,
            request.Parameters.Epsilon,
            request.Parameters.DisableIntersectionOptimizations);

        return new RunGenerationCommand(
            parameters,
            request.OperationId,
            request.SeriesId,
            request.ZoneCount ?? 20,
            request.Parameters.PointCount ?? 0,
            request.NeedAnalysis ?? true,
            request.NeedMetrics ?? false);
    }
}
