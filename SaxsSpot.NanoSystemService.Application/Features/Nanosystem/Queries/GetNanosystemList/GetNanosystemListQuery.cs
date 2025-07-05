using FluentResults;
using Gridify;
using MediatR;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystemList;

public record GetNanosystemListQuery(GridifyQuery Query) : IRequest<Result<Paging<NanosystemDto>>>;