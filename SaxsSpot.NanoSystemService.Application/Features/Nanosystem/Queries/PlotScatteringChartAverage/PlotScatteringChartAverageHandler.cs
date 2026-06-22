using FluentResults;
using MediatR;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Application.Services;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.PlotScatteringChartAverage;

public class PlotScatteringChartAverageHandler(
    IScatteringCalculationStorage storage,
    IScatteringResultObjectStorage objectStorage,
    IChartService chartService)
    : IRequestHandler<PlotScatteringChartAverageRequest, IResult<string>>
{
    public async Task<IResult<string>> Handle(PlotScatteringChartAverageRequest request, CancellationToken cancellationToken)
    {
        var datasets = await ScatteringIntensityDatasetBuilder.LoadDatasetsAsync(
            request.ScatteringCalculationIds,
            storage,
            objectStorage,
            cancellationToken);

        var average = ScatteringIntensityDatasetBuilder.BuildAverageDataset(datasets, "SAXS (new)");
        if (average is null)
        {
            return FluentResults.Result.Fail<string>("No scattering data found or datasets have different point counts.");
        }

        return await chartService.BuildChartAsync(
            request.ChartTitle,
            request.XAxis,
            request.YAxis,
            [average],
            request.ScaleMethodsX,
            request.ScaleMethodsY,
            cancellationToken);
    }
}
