using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemService.Application.Services.Saxs;
using SaxsSpot.NanoSystemService.Contracts.Enums;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Tests;

public class SaxsSpaceHandlerTests
{
    [Test]
    public void GetSpacedVector_LinearByCount_ReturnsExpectedLength()
    {
        var parameters = new SpaceParametersDto(SpaceMethod.Linear, ScaleMethod.Length, 5, 0.1, 0.5);
        var vector = SaxsSpaceHandler.GetSpacedVector(parameters);

        Assert.That(vector, Has.Length.EqualTo(5));
        Assert.That(vector[0], Is.EqualTo(0.1).Within(1e-9));
        Assert.That(vector[^1], Is.EqualTo(0.5).Within(1e-9));
    }
}

public class StrictParallelepipedScatteringCalculatorTests
{
    [Test]
    public void Calculate_WithSingleParticle_ReturnsPositiveIntensity()
    {
        var particles = new List<Parallelepiped>
        {
            new(1f, 1f, 0, 0, 0)
        };

        var qParams = new SpaceParametersDto(SpaceMethod.Linear, ScaleMethod.Length, 3, 0.05, 0.15);
        var results = StrictParallelepipedScatteringCalculator.Calculate(particles, qParams);

        Assert.That(results, Has.Count.EqualTo(3));
        Assert.That(results.All(r => double.IsFinite(r.Intensity)), Is.True);
    }

    [Test]
    public void Calculate_WithEmptySystem_Throws()
    {
        var qParams = new SpaceParametersDto(SpaceMethod.Linear, ScaleMethod.Length, 3, 0.05, 0.15);

        Assert.Throws<ArgumentException>(() =>
            StrictParallelepipedScatteringCalculator.Calculate([], qParams));
    }
}

public class SphereScatteringCalculatorTests
{
    [Test]
    public void Calculate_WithTwoSpheres_ReturnsResultsForEachQ()
    {
        var spheres = new List<Sphere>
        {
            new(1f, 0, 0, 0),
            new(1f, 2, 0, 0)
        };

        var qParams = new SpaceParametersDto(SpaceMethod.Linear, ScaleMethod.Length, 2, 0.02, 0.04);
        var results = SphereScatteringCalculator.Calculate(spheres, 10, 0.001, qParams);

        Assert.That(results, Has.Count.EqualTo(2));
        Assert.That(results.All(r => double.IsFinite(r.Intensity)), Is.True);
    }
}

public class RunScatteringCalculationCommandValidatorTests
{
    [Test]
    public void Validator_RejectsInvalidQRange()
    {
        var validator = new Features.Nanosystem.Commands.RunScatteringCalculation.RunScatteringCalculationCommandValidator();
        var command = new Features.Nanosystem.Commands.RunScatteringCalculation.RunScatteringCalculationCommand(
            Guid.NewGuid(),
            new SpaceParametersDto(SpaceMethod.Linear, ScaleMethod.Length, 5, 1, 0.5));

        var result = validator.Validate(command);

        Assert.That(result.IsValid, Is.False);
    }
}
