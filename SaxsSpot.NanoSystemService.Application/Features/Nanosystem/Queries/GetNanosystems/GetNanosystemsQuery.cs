using FluentResults;
using Gridify;
using MediatR;
using SaxsSpot.NanoSystemService.Contracts.Models;
using SaxsSpot.Shared.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystems;

public record GetNanosystemsQuery(ApiQuery Query) : IRequest<Result<IEnumerable<NanosystemDto>>>;