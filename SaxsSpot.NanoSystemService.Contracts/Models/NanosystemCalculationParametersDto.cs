namespace SaxsSpot.NanoSystemService.Contracts.Models;

public record NanosystemCalculationParametersDto(
    float ParticleVolumeSum,
    float SqrParticleVolumeSum,
    Amplitude[] Amplitudes,
    float SystemSize);
    
public record Amplitude(float[] Vector, float Value);