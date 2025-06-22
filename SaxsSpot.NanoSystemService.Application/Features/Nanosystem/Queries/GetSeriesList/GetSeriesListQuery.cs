using FluentResults;
using Gridify;
using MediatR;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetSeriesList;

public record GetSeriesListQuery(GridifyQuery Query) : IRequest<Result<Paging<NanosystemSeriesDto>>>;