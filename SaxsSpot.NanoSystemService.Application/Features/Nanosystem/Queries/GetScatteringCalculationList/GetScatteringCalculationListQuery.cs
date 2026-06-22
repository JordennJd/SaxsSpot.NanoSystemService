using FluentResults;
using Gridify;
using MediatR;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetScatteringCalculationList;

public record GetScatteringCalculationListQuery(GridifyQuery Query) : IRequest<Result<Paging<Contracts.Models.ScatteringCalculationDto>>>;
