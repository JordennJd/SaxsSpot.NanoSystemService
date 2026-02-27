namespace SaxsSpot.NanoSystemService.Contracts.Models;

/// <summary>
/// DTO for sphere particle used by 3D viewer (get-particles endpoint).
/// </summary>
public record SphereParticleDto
{
    public string Id { get; init; } = string.Empty;
    public float X { get; init; }
    public float Y { get; init; }
    public float Z { get; init; }
    public float Radius { get; init; }
}
