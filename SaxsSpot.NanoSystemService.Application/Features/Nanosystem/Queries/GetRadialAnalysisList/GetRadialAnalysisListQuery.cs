using FluentResults;
using Gridify;
using MediatR;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetRadialAnalysisList;

public record GetRadialAnalysisListQuery(GridifyQuery Query) : IRequest<Result<Paging<RadialAnalysisDto>>>;
