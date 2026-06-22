using MathNet.Numerics;
using MathNet.Numerics.Integration;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemService.Contracts.Models;
using SaxsSpot.NanoSystemService.Domain;
using static System.Math;

namespace SaxsSpot.NanoSystemService.Application.Services.Saxs;

public static class StrictParallelepipedScatteringCalculator
{
    private const double DefaultEpsilon = 1;
    private const double K = 6;
    private const double Theta = 1 / 0.4;

    public static IReadOnlyList<IntensityResult> Calculate(
        IEnumerable<Parallelepiped> particles,
        SpaceParametersDto qSpaceParameters)
    {
        var system = particles.ToList();
        if (system.Count == 0)
        {
            throw new ArgumentException("System is empty or doesn't exist");
        }

        var vectorQ = SaxsSpaceHandler.GetSpacedVector(qSpaceParameters);
        var sspCalculator = new SspCalculator();
        var minA = system.Min(x => x.A);
        var maxA = system.Max(x => x.A);
        var results = new List<IntensityResult>(vectorQ.Length);

        foreach (var q in vectorQ)
        {
            var intensity = GaussLegendreRule.Integrate(
                x => Pow(x, 3) * GammaDistribution(x) * sspCalculator.CalculateForParallelepiped(q * x * DefaultEpsilon, q * x),
                minA,
                maxA,
                5);

            results.Add(new IntensityResult(q, intensity));
        }

        return results;
    }

    private static double GammaDistribution(double x)
    {
        return 1 / SpecialFunctions.Gamma(K) * Pow(x, K - 1) * Exp(-x / Theta);
    }
}
