using FluentResults;
using MediatR;
using SaxsSpot.NanoSystemService.Contracts.Enums;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.PlotScatteringChart;

public record PlotScatteringChartRequest(
    string ChartTitle,
    string XAxis,
    string YAxis,
    List<Guid> ScatteringCalculationIds,
    SpaceMethod ScaleMethodsX,
    SpaceMethod ScaleMethodsY) : IRequest<IResult<string>>;
