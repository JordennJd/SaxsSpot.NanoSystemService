using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Application.Services;

public static class ScatteringCalculationWriter
{
    public static async Task<MemoryStream> Write(IReadOnlyList<IntensityResult> results)
    {
        var memoryStream = new MemoryStream();
        await using (var writer = new StreamWriter(memoryStream, leaveOpen: true))
        {
            foreach (var point in results.OrderBy(r => r.QVector))
            {
                await writer.WriteLineAsync(point.ToString());
            }

            await writer.FlushAsync();
        }

        memoryStream.Position = 0;
        return memoryStream;
    }
}
