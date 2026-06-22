using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Contracts.ExternalDto;
using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Application.Services;

public static class ScatteringIntensityDatasetBuilder
{
    public static async Task<List<Dataset>> LoadDatasetsAsync(
        IEnumerable<Guid> calculationIds,
        IScatteringCalculationStorage storage,
        IScatteringResultObjectStorage objectStorage,
        string legendLabel,
        CancellationToken cancellationToken)
    {
        var datasets = new List<Dataset>();
        var calculations = (await storage.WhereAsync(x => calculationIds.Contains(x.Id))).ToList();

        for (var i = 0; i < calculations.Count; i++)
        {
            var calculation = calculations[i];
            var points = new List<IntensityResult>();
            await foreach (var point in objectStorage.Load(calculation.ObjectId, cancellationToken))
            {
                points.Add(point);
            }

            if (points.Count == 0)
            {
                continue;
            }

            datasets.Add(new Dataset
            {
                id = calculations.Count == 1 ? legendLabel : $"{legendLabel} ({i + 1})",
                x = points.Select(p => p.QVector).ToArray(),
                y = points.Select(p => p.Intensity).ToArray()
            });
        }

        return datasets;
    }

    public static Dataset? BuildAverageDataset(IReadOnlyList<Dataset> datasets, string label)
    {
        if (datasets.Count == 0)
        {
            return null;
        }

        if (datasets.Count == 1)
        {
            return new Dataset
            {
                id = label,
                x = (double[])datasets[0].x.Clone(),
                y = (double[])datasets[0].y.Clone()
            };
        }

        var xRef = datasets[0].x;
        var n = xRef.Length;
        var ySum = new double[n];

        for (var i = 0; i < n; i++)
        {
            var sum = 0.0;
            for (var k = 0; k < datasets.Count; k++)
            {
                if (datasets[k].y.Length != n)
                {
                    return null;
                }

                sum += datasets[k].y[i];
            }

            ySum[i] = sum / datasets.Count;
        }

        return new Dataset
        {
            id = $"{label} (n={datasets.Count})",
            x = (double[])xRef.Clone(),
            y = ySum
        };
    }
}
