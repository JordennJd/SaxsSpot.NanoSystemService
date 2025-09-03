using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Engine.Extensions;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystemCalculationParameters;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetSphereScatteringCalculationParameters;

public class GetSphereScatteringCalculationParametersHandler(INanoSystemStorage storage, INanoSystemObjectStorage objectStorage,
    ILogger<GetNanosystemCalculationParametersHandler> logger)
    : IRequestHandler<GetSphereScatteringCalculationParametersQuery, Result<SphereScatteringCalculationParametersDto>>
{
    public async Task<Result<SphereScatteringCalculationParametersDto>> Handle(GetSphereScatteringCalculationParametersQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.Log(LogLevel.Information,
                $"requested SphereScatteringCalculationParameters with nanosystemId: {request.NanosystemId}");

            var nanosystem = await storage.FirstOrDefaultAsync(x => x.Id == request.NanosystemId);

            if (nanosystem is null)
            {
                throw new KeyNotFoundException($"Nanosystem with id: {request.NanosystemId} not found");
            }

            if (nanosystem.ParticleKind != ParticleKind.Sphere)
            {
                throw new ArgumentException("Invalid nanosystem (particle kind must be Sphere)");
            }

            var particles = objectStorage.Load(nanosystem.ObjectId);
            var sqrPariclesVolume = 0d;
            var sphereParameters = new SphereParameter[nanosystem.ParticleCount];
            var index = 0;

            await foreach (var particle in particles)
            {
                if (particle is Sphere sphere)
                {
                    sqrPariclesVolume += Math.Pow(sphere.GetVolume(), 2);
                    sphereParameters[index] = new SphereParameter(sphere, GetSpTmpConst(sphere), sphere.GetVolume());
                    index++;
                }
            }

            return FluentResults.Result
                .Ok(new SphereScatteringCalculationParametersDto(nanosystem.GlobalSize, sphereParameters, sqrPariclesVolume, nanosystem.NumericalConcentration));
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Error, ex.ToString());
            return FluentResults.Result.Fail<SphereScatteringCalculationParametersDto>(new Error(ex.Message));
        }
    }
    
    private static float GetSpTmpConst(Particle particle)
    {
        return MathF.Sqrt(particle.X * particle.X + particle.Y * particle.Y + particle.Z * particle.Z) / MathF.PI;
    }
}