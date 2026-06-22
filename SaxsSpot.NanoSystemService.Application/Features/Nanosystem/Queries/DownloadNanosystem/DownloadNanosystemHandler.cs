using FluentResults;
using MediatR;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Application.Services;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.DownloadNanosystem;

public class DownloadNanosystemHandler(INanoSystemObjectStorage storage, INanoSystemStorage systemStorage) : IRequestHandler<DownloadNanosystemQuery, IResult<Stream>>
{
    public async Task<IResult<Stream>> Handle(DownloadNanosystemQuery request, CancellationToken cancellationToken)
    {
        var system = await systemStorage.FirstOrDefaultAsync(x => x.Id == request.Id);
        if (system == null)
        {
            return FluentResults.Result.Fail<Stream>($"Nanosystem with ID {request.Id} not found");
        }

        var data = storage.Load(system.ObjectId, cancellationToken);
        var stream = await NanosystemWriter.Write(data, system);

        return FluentResults.Result.Ok(stream);
    }
}