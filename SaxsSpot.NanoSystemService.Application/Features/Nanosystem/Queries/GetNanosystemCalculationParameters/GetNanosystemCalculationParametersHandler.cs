using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Engine.Extensions;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystemCalculationParameters;

public class GetNanosystemCalculationParametersHandler(INanoSystemStorage storage, INanoSystemObjectStorage objectStorage,
    ILogger<GetNanosystemCalculationParametersHandler> logger)
    : IRequestHandler<GetNanosystemCalculationParametersQuery, Result<NanosystemCalculationParametersDto>>
{
    public async Task<Result<NanosystemCalculationParametersDto>> Handle(GetNanosystemCalculationParametersQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.Log(LogLevel.Information,
                $"requested NanosystemCalculationParameters with nanosystemId: {request.NanosystemId}");

            var nanosystem = await storage.FirstOrDefaultAsync(x => x.Id == request.NanosystemId);

            if (nanosystem is null)
            {
                throw new KeyNotFoundException($"Nanosystem with id: {request.NanosystemId} not found");
            }

            if (nanosystem.ParticleKind != ParticleKind.Parallelepiped)
            {
                throw new ArgumentException("Invalid nanosystem (particle kind must be Parallelepiped)");
            }

            var particles = objectStorage.Load(nanosystem.ObjectId);
            var pariclesVolume = 0f;
            var sqrPariclesVolume = 0f;
            var amplitudes = new Amplitude[nanosystem.ParticleCount];
            int index = 0;

            await foreach (var particle in particles)
            {
                if (particle is Parallelepiped parallelepiped)
                {
                    sqrPariclesVolume += parallelepiped.GetVolume();
                    pariclesVolume += MathF.Pow(parallelepiped.GetVolume(), 2);
                    amplitudes[index] = new Amplitude([parallelepiped.X, parallelepiped.Y, parallelepiped.Z],
                        parallelepiped.GetAmplitude());
                    index++;
                }
            }

            return FluentResults.Result
                .Ok(new NanosystemCalculationParametersDto(pariclesVolume, sqrPariclesVolume, amplitudes, nanosystem.GlobalSize));
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Error, $"requested invalid nanosystem: {request.NanosystemId} (particle kind must be Parallelepiped)");
            return FluentResults.Result.Fail<NanosystemCalculationParametersDto>(new Error("Invalid nanosystem"));
        }
    }
    

}