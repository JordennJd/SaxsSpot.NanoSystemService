using FluentResults;
using MathNet.Numerics;
using MediatR;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystemGenerationOptions;

/// <summary>
/// Get nanosystem mass generation options
/// </summary>
public class GetNanosystemGenerationOptionsHandler : IRequestHandler<GetNanosystemGenerationOptionsQuery, Result<MassGenerateNanoSystemOptions>>
{
    public Task<Result<MassGenerateNanoSystemOptions>> Handle(GetNanosystemGenerationOptionsQuery request,
        CancellationToken cancellationToken)
    {
        var r = request;
        var count = request.Count;

        var parametersList = new List<CommonParticleGenerationParameters>();

        var particleCounts = Generate.LinearSpaced(count, r.ParticleCountFrom, r.ParticleCountTo).Select(v => (int)v)
            .ToArray();
        var minSizes = Generate.LinearSpaced(count, r.MinParticleSizeFrom, r.MinParticleSizeTo);
        var maxSizes = Generate.LinearSpaced(count, r.MaxParticleSizeFrom, r.MaxParticleSizeTo);
        var thetas = Generate.LinearSpaced(count, r.ThetaFrom, r.ThetaTo);
        var ks = Generate.LinearSpaced(count, r.KFrom, r.KTo);
        
        var globalSizes = new double?[count];
        if (r.GlobalSizeFrom.HasValue && r.GlobalSizeTo.HasValue)
        {
            globalSizes = Generate.LinearSpaced(count, r.GlobalSizeFrom.Value, r.GlobalSizeTo.Value)
                .Select(x => (double?)x).ToArray();
            
        }
        
        var numericalConcentrations = new double?[count];
        if (r.NumericalConcentrationFrom.HasValue && r.NumericalConcentrationTo.HasValue)
        {
            numericalConcentrations = Generate.LinearSpaced(count, r.NumericalConcentrationFrom.Value, r.NumericalConcentrationTo.Value)
                .Select(x => (double?)x).ToArray();
            
        }
        
        var excesses = new double[count];
        
        if (r.ExcessFrom.HasValue && r.ExcessTo.HasValue)
        {
            excesses = Generate.LinearSpaced(count, r.ExcessFrom.Value, r.ExcessTo.Value);
        }
        
        var epsilons = new double?[count];
        if (request.ParticleKind is ParticleKind.Parallelepiped)
        {
            epsilons = Generate.LinearSpaced(count, r.EpsilonFrom.Value, r.EpsilonTo.Value)
                .Select(x => (double?)x).ToArray();
        }

        for (int i = 0; i < count; i++)
        {
            parametersList.Add(new CommonParticleGenerationParameters( particleCounts[i],
                (float?)numericalConcentrations[i], (float?)globalSizes[i],
                (float)minSizes[i], (float)maxSizes[i], (float)thetas[i], (float)ks[i], (float)excesses[i], (float?)epsilons[i]));

        }

        return Task.FromResult(FluentResults.Result.Ok(new MassGenerateNanoSystemOptions(parametersList, request.ParticleKind)));
    }
}
