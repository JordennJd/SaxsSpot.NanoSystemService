using FluentResults;
using MediatR;
using SaxsSpot.NanoSystemService.Contracts.Enums;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.PlotScatteringChartAverage;

public record PlotScatteringChartAverageRequest(
    string ChartTitle,
    string XAxis,
    string YAxis,
    List<Guid> ScatteringCalculationIds,
    SpaceMethod ScaleMethodsX,
    SpaceMethod ScaleMethodsY) : IRequest<IResult<string>>;
