using SaxsSpot.NanoSystemService.Contracts.Enums;
using SaxsSpot.NanoSystemService.Contracts.ExternalDto;

namespace SaxsSpot.NanoSystemService.Application.Interfaces;

public interface IChartService
{
    Task<FluentResults.Result<string>> BuildChartAsync(
        string chartTitle,
        string xAxis,
        string yAxis,
        Dataset[] datasets,
        SpaceMethod scaleMethodsX,
        SpaceMethod scaleMethodsY,
        CancellationToken cancellationToken = default);

    Task<FluentResults.Result<string>> BuildChartPngAsync(
        string chartTitle,
        string xAxis,
        string yAxis,
        Dataset[] datasets,
        SpaceMethod scaleMethodsX,
        SpaceMethod scaleMethodsY,
        CancellationToken cancellationToken = default);
}
