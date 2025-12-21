using FluentResults;
using MediatR;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Application.Services;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.DownloadRadialAnalysis;

public class DownloadRadialAnalysisHandler(
    IRadialAnalysisObjectStorage storage, 
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

        var data = storage.Load(analysis.ObjectId, cancellationToken);
        var stream = await RadialAnalysisWriter.Write(data, analysis);

        return FluentResults.Result.Ok(stream);
    }
}
