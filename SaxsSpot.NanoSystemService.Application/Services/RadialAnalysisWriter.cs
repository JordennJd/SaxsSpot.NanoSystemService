using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Application.Services;

public static class RadialAnalysisWriter
{
    /// <summary>
    /// Writes one line per layer: index (layerFrom - layerTo) NC.
    /// </summary>
    public static async Task<MemoryStream> Write(RadialAnalysis radialAnalysis, IReadOnlyList<RadialAnalysisLayer> layers)
    {
        var memoryStream = new MemoryStream();
        await using (var writer = new StreamWriter(memoryStream, leaveOpen: true))
        {
            foreach (var layer in layers.OrderBy(l => l.LayerIndex))
            {
                await writer.WriteLineAsync($"{layer.LayerIndex} ({layer.LayerFrom} - {layer.LayerTo}) {layer.NumericalConcentration}");
            }
            await writer.FlushAsync();
        }

        memoryStream.Position = 0;
        return memoryStream;
    }
}
