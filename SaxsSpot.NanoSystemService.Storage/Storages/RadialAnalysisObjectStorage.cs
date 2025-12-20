using Microsoft.Extensions.Configuration;
using SaxsSpot.Core.CommonObjectStorage.Engine;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.AnalyzeModels;
using SaxsSpot.NanoSystemService.Application.Interfaces;

namespace SaxsSpot.NanoSystemService.Storage.Storages;

public class RadialAnalysisObjectStorage(IConfiguration configuration)
    : CommonObjectStorage<ZoneConcentrationAnalyze>(configuration), IRadialAnalysisObjectStorage
{
    protected override Stream GetStream(IEnumerable<ZoneConcentrationAnalyze> data)
    {
        var stream = new MemoryStream();
        using var streamWriter = new StreamWriter(stream, leaveOpen: true);

        foreach (var zoneConcentrationAnalyze in data)
        {
            streamWriter.WriteLine(zoneConcentrationAnalyze.ToString());
        }

        streamWriter.Flush();
        return stream;
    }

    protected override async IAsyncEnumerable<ZoneConcentrationAnalyze> FromStreamAsync(Stream data)
    {
        using var streamReader = new StreamReader(data, leaveOpen: true);
        
        string? str;
        while ((str = await streamReader.ReadLineAsync()) != null)
        {
            yield return ZoneConcentrationAnalyze.FromString(str);
        }
    }
}