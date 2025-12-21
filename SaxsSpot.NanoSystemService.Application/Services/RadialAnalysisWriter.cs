using System.Text.Json;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.AnalyzeModels;
using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Application.Services;

public static class RadialAnalysisWriter
{
    public static async Task<MemoryStream> Write(IAsyncEnumerable<ZoneConcentrationAnalyze> analysis, RadialAnalysis radialAnalysis)
    {
        var memoryStream = new MemoryStream();
        
        var json = JsonSerializer.Serialize(radialAnalysis);
        await using (var writer = new StreamWriter(memoryStream, leaveOpen: true))
        {
            await writer.WriteLineAsync(json);
            await foreach (var zone in analysis)
            {
                await writer.WriteLineAsync(zone.ToString());
            }

            await writer.FlushAsync();
        }

        memoryStream.Position = 0;

        return memoryStream;
    }
}
