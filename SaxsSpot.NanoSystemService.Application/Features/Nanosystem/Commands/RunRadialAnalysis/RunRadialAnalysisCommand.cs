using FluentResults;
using MediatR;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunRadialAnalysis;

/// <summary>
/// 
/// </summary>
/// <param name="NanosystemId"></param>
/// <param name="PointCount"></param>
/// <param name="LayerCount"></param>
public record RunRadialAnalysisCommand(Guid NanosystemId, int PointCount, int LayerCount) : IRequest<IResult<Guid>>;