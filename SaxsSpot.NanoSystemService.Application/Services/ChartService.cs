using System.Net.Http.Json;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Contracts.Enums;
using SaxsSpot.NanoSystemService.Contracts.ExternalDto;

namespace SaxsSpot.NanoSystemService.Application.Services;

public class ChartService(IConfiguration configuration, ILogger<ChartService> logger) : IChartService
{
    public async Task<Result<string>> BuildChartAsync(
        string chartTitle,
        string xAxis,
        string yAxis,
        Dataset[] datasets,
        SpaceMethod scaleMethodsX,
        SpaceMethod scaleMethodsY,
        CancellationToken cancellationToken = default)
    {
        var plotRequest = new PlotRequest
        {
            title = chartTitle,
            x_label = xAxis,
            y_label = yAxis,
            x_log_scale = scaleMethodsX == SpaceMethod.Log,
            y_log_scale = scaleMethodsY == SpaceMethod.Log,
            datasets = datasets
        };

        foreach (var dataset in datasets)
        {
            dataset.SortByX();
        }

        using var client = new HttpClient();
        try
        {
            var chartUri = configuration.GetValue<string>("chart:uri");
            if (string.IsNullOrEmpty(chartUri))
            {
                return FluentResults.Result.Fail<string>("Chart URI is not configured");
            }

            var response = await client.PostAsJsonAsync($"{chartUri}/plot", plotRequest, cancellationToken);
            response.EnsureSuccessStatusCode();
            return FluentResults.Result.Ok(await response.Content.ReadAsStringAsync(cancellationToken));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to build chart: {Message}", e.Message);
            return FluentResults.Result.Fail<string>("Build chart failed");
        }
    }

    public async Task<Result<string>> BuildChartPngAsync(
        string chartTitle,
        string xAxis,
        string yAxis,
        Dataset[] datasets,
        SpaceMethod scaleMethodsX,
        SpaceMethod scaleMethodsY,
        CancellationToken cancellationToken = default)
    {
        var plotRequest = new PlotRequest
        {
            title = chartTitle,
            x_label = xAxis,
            y_label = yAxis,
            x_log_scale = scaleMethodsX == SpaceMethod.Log,
            y_log_scale = scaleMethodsY == SpaceMethod.Log,
            datasets = datasets
        };

        foreach (var dataset in datasets)
        {
            dataset.SortByX();
        }

        using var client = new HttpClient();
        try
        {
            var chartUri = configuration.GetValue<string>("chart:uri");
            if (string.IsNullOrEmpty(chartUri))
            {
                return FluentResults.Result.Fail<string>("Chart URI is not configured");
            }

            using var response = await client.PostAsJsonAsync($"{chartUri}/plot/png", plotRequest, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                return FluentResults.Result.Fail<string>($"Build chart PNG failed: HTTP {(int)response.StatusCode}. {body}");
            }

            var pngBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            return FluentResults.Result.Ok(Convert.ToBase64String(pngBytes));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to build chart PNG: {Message}", e.Message);
            return FluentResults.Result.Fail<string>($"Build chart PNG failed: {e.Message}");
        }
    }
}
