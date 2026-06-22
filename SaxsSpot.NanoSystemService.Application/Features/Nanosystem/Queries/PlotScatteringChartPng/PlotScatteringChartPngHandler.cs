using FluentResults;
using MediatR;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Application.Services;
using SaxsSpot.NanoSystemService.Contracts.Enums;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.PlotScatteringChartPng;

public record PlotScatteringChartPngRequest(
    string ChartTitle,
    string XAxis,
    string YAxis,
    List<Guid> ScatteringCalculationIds,
    SpaceMethod ScaleMethodsX,
    SpaceMethod ScaleMethodsY) : IRequest<IResult<string>>;

public class PlotScatteringChartPngHandler(
    IScatteringCalculationStorage storage,
    IScatteringResultObjectStorage objectStorage,
    IChartService chartService)
    : IRequestHandler<PlotScatteringChartPngRequest, IResult<string>>
{
    public async Task<IResult<string>> Handle(PlotScatteringChartPngRequest request, CancellationToken cancellationToken)
    {
        var datasets = await ScatteringIntensityDatasetBuilder.LoadDatasetsAsync(
            request.ScatteringCalculationIds,
            storage,
            objectStorage,
            cancellationToken);

        if (datasets.Count == 0)
        {
            return FluentResults.Result.Fail<string>("No scattering data found for the selected calculations.");
        }

        return await chartService.BuildChartPngAsync(
            request.ChartTitle,
            request.XAxis,
            request.YAxis,
            datasets.ToArray(),
            request.ScaleMethodsX,
            request.ScaleMethodsY,
            cancellationToken);
    }
}
