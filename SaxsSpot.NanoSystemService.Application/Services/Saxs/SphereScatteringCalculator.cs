using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemService.Contracts.Models;
using SaxsSpot.NanoSystemService.Domain;
using static System.Math;

namespace SaxsSpot.NanoSystemService.Application.Services.Saxs;

public static class SphereScatteringCalculator
{
    public static IReadOnlyList<IntensityResult> Calculate(
        IEnumerable<Sphere> spheres,
        double globalSize,
        double numericalConcentration,
        SpaceParametersDto qSpaceParameters,
        double excess = 0)
    {
        var system = TrimSystem(spheres.ToList(), excess, globalSize);
        if (system.Count == 0)
        {
            throw new ArgumentException("System is empty or doesn't exist");
        }

        var vectorQ = SaxsSpaceHandler.GetSpacedVector(qSpaceParameters);
        var sphereData = system.Select(s => new SphereData(
            s,
            s.GetVolume(),
            GetSpTmpConst(s))).ToList();
        var sqrOfSphereVolume = sphereData.Sum(x => x.Volume * x.Volume);
        var results = new List<IntensityResult>(vectorQ.Length);

        foreach (var q in vectorQ)
        {
            var spGlobal = SpFactor(globalSize, q) * 4.0 / 3.0 * PI * Pow(globalSize, 3);
            var spFirstSummand = 0.0;
            var s2 = 0.0;
            var spFactors = 0.0;
            var count = sphereData.Count;

            for (var j = 0; j < count; j++)
            {
                var sphere = sphereData[j];
                var sphereSp = SpFactor(sphere.Sphere.Radius, q);
                var tmp = q * sphere.SpTmpConst;
                spFirstSummand += sphere.Volume * sphereSp * Sinc(tmp);
                s2 += Pow(sphere.Volume * sphereSp, 2);

                if (j != count - 1)
                {
                    for (var i = j + 1; i < count; i++)
                    {
                        var sphereI = sphereData[i];
                        var mlt = q * TmpMatrix(
                            sphere.Sphere.X, sphereI.Sphere.X,
                            sphere.Sphere.Y, sphereI.Sphere.Y,
                            sphere.Sphere.Z, sphereI.Sphere.Z);
                        spFactors += sphereI.Volume * sphere.Volume *
                                     sphereSp *
                                     SpFactor(sphereI.Sphere.Radius, q) *
                                     Sinc(mlt);
                    }
                }
            }

            var intensity = (s2 + 2 * spFactors +
                               Pow(numericalConcentration, 2) * Pow(spGlobal, 2) -
                               2 * numericalConcentration * spFirstSummand * spGlobal) / sqrOfSphereVolume;
            results.Add(new IntensityResult(q, intensity));
        }

        return results;
    }

    private static List<Sphere> TrimSystem(List<Sphere> system, double excess, double globalSize)
    {
        if (excess <= 0 || system.Count == 0)
        {
            return system;
        }

        var trimSize = globalSize * (1 - excess);
        var halfTrim = trimSize / 2;
        return system
            .Where(s => Abs(s.X) <= halfTrim && Abs(s.Y) <= halfTrim && Abs(s.Z) <= halfTrim)
            .ToList();
    }

    private static float GetSpTmpConst(Particle particle)
    {
        return MathF.Sqrt(particle.X * particle.X + particle.Y * particle.Y + particle.Z * particle.Z) / MathF.PI;
    }

    private static double Sinc(double x)
    {
        return x != 0.0 ? Sin(PI * x) / (PI * x) : 1.0;
    }

    private static double SpFactor(double radius, double q)
    {
        var x = radius * q;
        return 3.0 * ((Sin(x) - x * Cos(x)) / Pow(x, 3));
    }

    private static double TmpMatrix(double x, double x0, double y, double y0, double z, double z0)
    {
        return Sqrt(Pow(x - x0, 2) + Pow(y - y0, 2) + Pow(z - z0, 2)) / PI;
    }

    private sealed record SphereData(Sphere Sphere, double Volume, float SpTmpConst);
}
