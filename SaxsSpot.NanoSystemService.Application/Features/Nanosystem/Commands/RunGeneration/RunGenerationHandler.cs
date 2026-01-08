using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemService.Contracts.Services;
using SaxsSpot.Shared.ProgressTrackerClient.Contracts.Services;
using JobModels = SaxsSpot.Shared.ProgressTrackerClient.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunGeneration;

/// <summary>
/// Run single nanosystem generation
/// </summary>
public class RunGenerationHandler(
    INanoSystemService nanoSystemService,
    ILogger<RunGenerationHandler> logger,
    IMapper mapper,
    IJobServiceClient jobServiceClient) : IRequestHandler<RunGenerationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RunGenerationCommand request, CancellationToken cancellationToken)
    {
        var operationGuid = request.OperationId;

        try
        {
            var startResult =
                await jobServiceClient.StartJobAsync(new JobModels.StartJobQuery(operationGuid.ToString()));
            if (!startResult.IsSuccessful)
            {
                throw new InvalidOperationException(startResult.ErrorMessage);
            }

            logger.Log(LogLevel.Information, $"Run generation started with operation id: {operationGuid}, series id: {request.SeriesId}");
            switch (request.Parameters.GetParticleKind())
            {
                case ParticleKind.Parallelepiped:
                    await nanoSystemService.RunGeneration(mapper.Map<ParallelepipedGenerationParameters>(request.Parameters), seriesId: request.SeriesId, cancellationToken: cancellationToken);
                    break;
                case ParticleKind.Sphere:
                    await nanoSystemService.RunGeneration(mapper.Map<SphereGenerationParameters>(request.Parameters), seriesId: request.SeriesId, cancellationToken: cancellationToken);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var endResult =
                await jobServiceClient.CompleteJobAsync(new JobModels.CompleteJobQuery(operationGuid.ToString(),
                    "Generation completed"));

            return operationGuid;
        }
        catch (OperationCanceledException e)
        {

            logger.Log(LogLevel.Information, $"Run generation canceled with operationId: {operationGuid}");
            throw;
        }
        catch (Exception e)
        {
            await jobServiceClient.CompleteJobAsync(new JobModels.CompleteJobQuery(operationGuid.ToString(),
                $"Generation failed with error {e.Message}", IsFailed: true));
            throw;
        }
    }
}