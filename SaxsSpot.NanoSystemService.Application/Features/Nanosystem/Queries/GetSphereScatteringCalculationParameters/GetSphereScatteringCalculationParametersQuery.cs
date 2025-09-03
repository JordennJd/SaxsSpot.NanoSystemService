using FluentResults;
using MediatR;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetSphereScatteringCalculationParameters;

public record GetSphereScatteringCalculationParametersQuery(Guid NanosystemId) : IRequest<Result<SphereScatteringCalculationParametersDto>>;