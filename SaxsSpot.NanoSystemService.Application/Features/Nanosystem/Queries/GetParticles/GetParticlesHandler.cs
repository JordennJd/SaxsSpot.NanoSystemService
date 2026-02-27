using FluentResults;
using MediatR;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetParticles;

public class GetParticlesHandler(INanoSystemStorage storage, INanoSystemObjectStorage objectStorage)
    : IRequestHandler<GetParticlesQuery, Result<object>>
{
    public async Task<Result<object>> Handle(GetParticlesQuery request, CancellationToken cancellationToken)
    {
        var nanosystem = await storage.FirstOrDefaultAsync(x => x.Id == request.NanosystemId);
        if (nanosystem is null)
            return FluentResults.Result.Fail<object>($"Nanosystem with id {request.NanosystemId} not found.");

        var particleKind = request.FilterParticleKind ?? nanosystem.ParticleKind;
        if (particleKind != nanosystem.ParticleKind)
            return FluentResults.Result.Fail<object>($"Nanosystem has particle kind {nanosystem.ParticleKind}, requested {particleKind}.");

        var particles = objectStorage.Load(nanosystem.ObjectId, cancellationToken);
        var list = await CollectWithSkipTake(particles, request.Skip, request.Take, nanosystem.Id, particleKind, cancellationToken);
        return FluentResults.Result.Ok<object>(list);
    }

    private static async Task<object> CollectWithSkipTake(
        IAsyncEnumerable<Particle> particles,
        int skip,
        int take,
        Guid nanosystemId,
        ParticleKind particleKind,
        CancellationToken cancellationToken)
    {
        if (particleKind == ParticleKind.Parallelepiped)
        {
            var result = new List<ParallelepipedParticleDto>();
            var index = 0;
            await foreach (var p in particles.WithCancellation(cancellationToken))
            {
                if (index < skip) { index++; continue; }
                if (result.Count >= take) break;
                if (p is Parallelepiped par)
                    result.Add(new ParallelepipedParticleDto
                    {
                        Id = $"{nanosystemId}_{index}",
                        X = par.X,
                        Y = par.Y,
                        Z = par.Z,
                        Fi = par.Phi,
                        Theta = par.Theta,
                        Zenit = par.Zenit,
                        A = par.A,
                        E = par.E
                    });
                index++;
            }
            return result;
        }

        var spheres = new List<SphereParticleDto>();
        var i = 0;
        await foreach (var p in particles.WithCancellation(cancellationToken))
        {
            if (i < skip) { i++; continue; }
            if (spheres.Count >= take) break;
            if (p is Sphere s)
                spheres.Add(new SphereParticleDto
                {
                    Id = $"{nanosystemId}_{i}",
                    X = s.X,
                    Y = s.Y,
                    Z = s.Z,
                    Radius = s.Radius
                });
            i++;
        }
        return spheres;
    }
}
