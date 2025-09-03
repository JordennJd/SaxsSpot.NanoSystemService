using SaxsSpot.NanoSystemGeneration.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Contracts.Models;

public record SphereScatteringCalculationParametersDto(
    double GlobalSize,
    SphereParameter[] Spheres,
    double SqrVolumeOfSpheres,
    double NumericalConcentration
    );

public record SphereParameter(Sphere Sphere, float SpTmpConst, double Volume);