namespace SaxsSpot.NanoSystemService.Contracts.Models;

/// <summary>
/// DTO for parallelepiped particle used by 3D viewer (get-particles endpoint).
/// </summary>
public record ParallelepipedParticleDto
{
    public string Id { get; init; } = string.Empty;
    public float X { get; init; }
    public float Y { get; init; }
    public float Z { get; init; }
    public float Fi { get; init; }
    public float Theta { get; init; }
    public float Zenit { get; init; }
    public float A { get; init; }
    public float E { get; init; }
}
