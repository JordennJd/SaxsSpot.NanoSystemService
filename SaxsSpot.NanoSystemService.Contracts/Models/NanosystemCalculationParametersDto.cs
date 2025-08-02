namespace SaxsSpot.NanoSystemService.Contracts.Models;

public record NanosystemCalculationParametersDto(
    float ParticleVolumeSum,
    Amplitude[] Amplitudes,
    float SystemSize);
    
public record Amplitude(float[] Vector, float Value);