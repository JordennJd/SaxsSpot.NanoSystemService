using System.Text.Json;
using FluentResults;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Application.Services.Saxs;
using SaxsSpot.NanoSystemService.Contracts.Enums;
using SaxsSpot.NanoSystemService.Domain;
using SaxsSpot.Shared.ProgressTrackerClient.Contracts.Services;
using JobModels = SaxsSpot.Shared.ProgressTrackerClient.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunScatteringCalculation;

public class RunScatteringCalculationHandler(
    INanoSystemStorage nanosystemStorage,
    IServiceScopeFactory scopeFactory,
    ILogger<RunScatteringCalculationHandler> logger)
    : IRequestHandler<RunScatteringCalculationCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(RunScatteringCalculationCommand request, CancellationToken cancellationToken)
    {
        var operationGuid = Guid.NewGuid();

        try
        {
            logger.LogInformation(
                "Starting scattering calculation for nanosystem {NanosystemId} with operation id {OperationId}",
                request.NanosystemId, operationGuid);

            var nanosystem = await nanosystemStorage.FirstOrDefaultAsync(x => x.Id == request.NanosystemId);
            if (nanosystem == null)
            {
                throw new ArgumentException($"Nanosystem with ID {request.NanosystemId} does not exist.");
            }

            if (nanosystem.ParticleKind is not (ParticleKind.Parallelepiped or ParticleKind.Sphere))
            {
                throw new ArgumentException($"Unsupported particle kind: {nanosystem.ParticleKind}");
            }

            var inputDate = DateTime.UtcNow;
            var calculationKind = nanosystem.ParticleKind == ParticleKind.Sphere
                ? ScatteringCalculationKind.Sphere
                : ScatteringCalculationKind.StrictParallelepiped;
            var jobType = calculationKind == ScatteringCalculationKind.Sphere
                ? "RunSphereScatteringCalculation"
                : "RunStrictScatteringCalculation";

            _ = Task.Run(async () =>
            {
                using var scope = scopeFactory.CreateScope();
                var scatteringStorage = scope.ServiceProvider.GetRequiredService<IScatteringCalculationStorage>();
                var resultObjectStorage = scope.ServiceProvider.GetRequiredService<IScatteringResultObjectStorage>();
                var objectStorage = scope.ServiceProvider.GetRequiredService<INanoSystemObjectStorage>();
                var jobService = scope.ServiceProvider.GetService<IJobServiceClient>();

                var createResult = await jobService!.CreateJobAsync(new JobModels.CreateJobQuery(
                    operationGuid.ToString(),
                    jobType,
                    "scattering calculation started",
                    JsonSerializer.Serialize(request)));

                if (createResult.IsSuccessful is false)
                {
                    throw new InvalidOperationException(
                        $"Operation not started with id {operationGuid}: {createResult.ErrorMessage}");
                }

                var startDate = DateTime.UtcNow;
                try
                {
                    await jobService.StartJobAsync(new JobModels.StartJobQuery(operationGuid.ToString()));
                    await jobService.ChangeJobMessageAsync(new JobModels.ChangeJobMessageQuery(
                        operationGuid.ToString(),
                        "Scattering calculation in progress..."));

                    var particles = objectStorage.Load(nanosystem.ObjectId, CancellationToken.None);
                    IReadOnlyList<IntensityResult> intensityResults;

                    if (calculationKind == ScatteringCalculationKind.StrictParallelepiped)
                    {
                        var parallelepipeds = particles
                            .ToBlockingEnumerable()
                            .Select(x => (Parallelepiped)x)
                            .ToList();
                        intensityResults = StrictParallelepipedScatteringCalculator.Calculate(
                            parallelepipeds,
                            request.QSpaceParameters);
                    }
                    else
                    {
                        var spheres = particles
                            .ToBlockingEnumerable()
                            .Select(x => (Sphere)x)
                            .ToList();
                        intensityResults = SphereScatteringCalculator.Calculate(
                            spheres,
                            nanosystem.GlobalSize,
                            nanosystem.NumericalConcentration,
                            request.QSpaceParameters);
                    }

                    var objectId = Guid.NewGuid();
                    await resultObjectStorage.Save(intensityResults, objectId);

                    await scatteringStorage.UpdateOrInsertAsync(new ScatteringCalculation
                    {
                        Id = operationGuid,
                        NanosystemId = nanosystem.Id,
                        ObjectId = objectId,
                        CalculationKind = (int)calculationKind,
                        QVectorFrom = request.QSpaceParameters.Start,
                        QVectorTo = request.QSpaceParameters.End,
                        QSpaceMethod = (int)request.QSpaceParameters.SpaceMethod,
                        QScaleMethod = (int)request.QSpaceParameters.ScaleMethod,
                        QSpaceParameter = request.QSpaceParameters.SpaceParameter,
                        Excess = null,
                        InputDate = inputDate,
                        StartDate = startDate,
                        EndDate = DateTime.UtcNow,
                    });

                    await jobService.CompleteJobAsync(new JobModels.CompleteJobQuery(
                        operationGuid.ToString(),
                        "scattering calculation completed"));
                }
                catch (Exception e)
                {
                    logger.LogError(e,
                        "Error during scattering calculation for operation {OperationId}: {ErrorMessage}",
                        operationGuid, e.Message);
                    await jobService.CompleteJobAsync(new JobModels.CompleteJobQuery(
                        operationGuid.ToString(),
                        e.Message,
                        true));
                }
            });

            return FluentResults.Result.Ok(operationGuid);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to start scattering calculation operation {OperationId} for nanosystem {NanosystemId}",
                operationGuid, request.NanosystemId);
            throw;
        }
    }
}
