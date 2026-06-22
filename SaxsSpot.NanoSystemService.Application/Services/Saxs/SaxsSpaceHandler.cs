using MathNet.Numerics;
using SaxsSpot.NanoSystemService.Contracts.Enums;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Services.Saxs;

public static class SaxsSpaceHandler
{
    public static double[] GetSpacedVector(SpaceParametersDto parameters)
    {
        return parameters.ScaleMethod switch
        {
            ScaleMethod.Length when parameters.SpaceMethod == SpaceMethod.Linear =>
                Generate.LinearSpaced(Convert.ToInt32(parameters.SpaceParameter), parameters.Start, parameters.End),
            ScaleMethod.Step when parameters.SpaceMethod == SpaceMethod.Linear =>
                Generate.LinearRange(parameters.Start, parameters.SpaceParameter, parameters.End),
            ScaleMethod.Length when parameters.SpaceMethod == SpaceMethod.Log =>
                Generate.LogSpaced(Convert.ToInt32(parameters.SpaceParameter), parameters.Start, parameters.End),
            ScaleMethod.Step when parameters.SpaceMethod == SpaceMethod.Log =>
                throw new ArgumentException("Logarithmic scale with step is not implemented"),
            _ => throw new ArgumentException("Invalid space parameters")
        };
    }
}
