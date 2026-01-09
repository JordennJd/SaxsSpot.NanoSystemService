using FluentResults;
using MediatR;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunGeneration;

public record RunGenerationCommand(
    CommonParticleGenerationParameters Parameters, 
    Guid OperationId, 
    Guid SeriesId = default,
    int ZoneCount = 20,
    int PointCount = 5_000_000,
    bool NeedAnalysis = true,
    bool NeedMetrics = false) : IRequest<Result<Guid>>;