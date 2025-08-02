using FluentResults;
using MediatR;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystemCalculationParameters;

public record GetNanosystemCalculationParametersQuery(Guid NanosystemId) : IRequest<IResult<NanosystemCalculationParametersDto>>;