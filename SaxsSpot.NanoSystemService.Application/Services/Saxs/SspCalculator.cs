using MathNet.Numerics;
using MathNet.Numerics.Integration;
using static System.Math;

namespace SaxsSpot.NanoSystemService.Application.Services.Saxs;

public class SspCalculator
{
    public double CalculateForParallelepiped(double c1, double c2)
    {
        return 2.0 / PI * GaussLegendreRule.Integrate(
            (x, y) => Pow(Sin(c1 * x) * (Sin(c2 * Sqrt(1 - Pow(x, 2)) * Sin(y))
                                         * Sin(c2 * Sqrt(1 - Pow(x, 2)) * Cos(y))
                                         / (c1 * c2 * c2 * x * (1 - Pow(x, 2)) * Sin(y) *
                                            Cos(y))), 2),
            0, 1, 0, PI / 2, 5);
    }
}
