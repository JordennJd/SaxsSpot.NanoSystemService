using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones.Enums;

namespace SaxsSpot.NanoSystemService.Contracts.Models;

public class NanosystemDto
{
    public Guid Id { get; set; }
    
    public ParticleKind ParticleKind { get; set; }
    
    public Guid SeriesId { get; set; }
    
    public Guid ObjectId { get; set; }
    
    public long UserId { get; set; }
    
    public int ParticleCount { get; set; }
    
    public float GlobalSize { get; set; }
    
    public float GenerationZoneVolume { get; set; }
    
    public GenerationZoneForm GenerationZoneForm { get; set; }

    public float NumericalConcentration { get; set; }
    
    public float MaxParticleSize { get; set; }
    
    public float MinParticleSize { get; set; }
    
    public float Excess { get; set; }
    
    public float K { get; set; }
    
    public float Theta { get; set; }
    
    public DateTime GenerationStart { get; set; }
    
    public DateTime GenerationEnd { get; set; }
    
    public DateTime InputDate { get; set; }
}