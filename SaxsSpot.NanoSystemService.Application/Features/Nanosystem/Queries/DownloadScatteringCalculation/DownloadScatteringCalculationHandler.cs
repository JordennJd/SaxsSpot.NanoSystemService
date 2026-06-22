using FluentResults;
using MediatR;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Application.Services;
using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.DownloadScatteringCalculation;

public class DownloadScatteringCalculationHandler(
    IScatteringCalculationStorage storage,
    IScatteringResultObjectStorage objectStorage)
    : IRequestHandler<DownloadScatteringCalculationQuery, IResult<Stream>>
{
    public async Task<IResult<Stream>> Handle(DownloadScatteringCalculationQuery request, CancellationToken cancellationToken)
    {
        var calculation = await storage.FirstOrDefaultAsync(x => x.Id == request.Id);
        if (calculation == null)
        {
            return FluentResults.Result.Fail<Stream>($"Scattering calculation with ID {request.Id} not found");
        }

        if (calculation.ObjectId == Guid.Empty)
        {
            return FluentResults.Result.Fail<Stream>($"No result data found for scattering calculation {request.Id}");
        }

        var memoryStream = new MemoryStream();
        await using (var writer = new StreamWriter(memoryStream, leaveOpen: true))
        {
            var hasData = false;
            await foreach (var point in objectStorage.Load(calculation.ObjectId, cancellationToken))
            {
                hasData = true;
                await writer.WriteLineAsync(point.ToString());
            }

            if (!hasData)
            {
                await memoryStream.DisposeAsync();
                return FluentResults.Result.Fail<Stream>($"No intensity data found for scattering calculation {request.Id}");
            }

            await writer.FlushAsync();
        }

        memoryStream.Position = 0;
        return FluentResults.Result.Ok<Stream>(memoryStream);
    }
}
