using System.Text.Json;
using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Application.Services;

public static class RadialAnalysisWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>
    /// Writes radial analysis and all layer data (as in DB) to stream: first line = metadata JSON, then one JSON line per layer.
    /// </summary>
    public static async Task<MemoryStream> Write(RadialAnalysis radialAnalysis, IReadOnlyList<RadialAnalysisLayer> layers)
    {
        var memoryStream = new MemoryStream();
        await using (var writer = new StreamWriter(memoryStream, leaveOpen: true))
        {
            var metadataJson = JsonSerializer.Serialize(radialAnalysis, JsonOptions);
            await writer.WriteLineAsync(metadataJson);

            foreach (var layer in layers.OrderBy(l => l.LayerIndex))
            {
                var layerJson = JsonSerializer.Serialize(layer, JsonOptions);
                await writer.WriteLineAsync(layerJson);
            }

            await writer.FlushAsync();
        }

        memoryStream.Position = 0;
        return memoryStream;
    }
}
