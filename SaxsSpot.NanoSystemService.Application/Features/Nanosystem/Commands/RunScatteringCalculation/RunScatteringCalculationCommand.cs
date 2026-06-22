using FluentResults;
using MediatR;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunScatteringCalculation;

public record RunScatteringCalculationCommand(
    Guid NanosystemId,
    SpaceParametersDto QSpaceParameters,
    double? Excess = null) : IRequest<IResult<Guid>>;
