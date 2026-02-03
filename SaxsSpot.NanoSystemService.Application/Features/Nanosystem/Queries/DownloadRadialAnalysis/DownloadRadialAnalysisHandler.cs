using FluentResults;
using MediatR;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Application.Services;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.DownloadRadialAnalysis;

public class DownloadRadialAnalysisHandler(
    IRadialAnalysisLayerStorage layerStorage,
    IRadialAnalysisStorage analysisStorage)
    : IRequestHandler<DownloadRadialAnalysisQuery, IResult<Stream>>
{
    public async Task<IResult<Stream>> Handle(DownloadRadialAnalysisQuery request, CancellationToken cancellationToken)
    {
        var analysis = await analysisStorage.FirstOrDefaultAsync(x => x.Id == request.Id);
        if (analysis == null)
        {
            return FluentResults.Result.Fail<Stream>($"Radial analysis with ID {request.Id} not found");
        }

        var layers = (await layerStorage.WhereAsync(x => x.RadialAnalysisId == request.Id))
            .OrderBy(x => x.LayerIndex)
            .ToList();
        if (layers.Count == 0)
        {
            return FluentResults.Result.Fail<Stream>($"No layer data found for radial analysis {request.Id}");
        }

        var stream = await RadialAnalysisWriter.Write(analysis, layers);
        return FluentResults.Result.Ok(stream);
    }
}
