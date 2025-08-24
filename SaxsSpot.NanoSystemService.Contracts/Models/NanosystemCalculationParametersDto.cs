namespace SaxsSpot.NanoSystemService.Contracts.Models;

public record NanosystemCalculationParametersDto(
    double ParticleVolumeSum,
    double SqrParticleVolumeSum,
    Amplitude[] Amplitudes,
    double SystemSize);
    
public record Amplitude(double[] Vector, double Value);