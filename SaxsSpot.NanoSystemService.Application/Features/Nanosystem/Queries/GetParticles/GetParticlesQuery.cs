using FluentResults;
using MediatR;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetParticles;

/// <summary>
/// Returns a slice of particles for 3D visualization.
/// filterParticleKind: 0 = Sphere, 1 = Parallelepiped (optional; if null, uses nanosystem's kind).
/// </summary>
public record GetParticlesQuery(
    Guid NanosystemId,
    int Skip = 0,
    int Take = 10000,
    ParticleKind? FilterParticleKind = null) : IRequest<Result<object>>;
