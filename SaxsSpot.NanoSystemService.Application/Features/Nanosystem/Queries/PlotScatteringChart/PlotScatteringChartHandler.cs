using FluentResults;
using MediatR;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Application.Services;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.PlotScatteringChart;

public class PlotScatteringChartHandler(
    IScatteringCalculationStorage storage,
    IScatteringResultObjectStorage objectStorage,
    IChartService chartService)
    : IRequestHandler<PlotScatteringChartRequest, IResult<string>>
{
    public async Task<IResult<string>> Handle(PlotScatteringChartRequest request, CancellationToken cancellationToken)
    {
        var datasets = await ScatteringIntensityDatasetBuilder.LoadDatasetsAsync(
            request.ScatteringCalculationIds,
            storage,
            objectStorage,
            "Theory",
            cancellationToken);

        if (datasets.Count == 0)
        {
            return FluentResults.Result.Fail<string>("No scattering data found for the selected calculations.");
        }

        return await chartService.BuildChartAsync(
            request.ChartTitle,
            request.XAxis,
            request.YAxis,
            datasets.ToArray(),
            request.ScaleMethodsX,
            request.ScaleMethodsY,
            cancellationToken);
    }
}
