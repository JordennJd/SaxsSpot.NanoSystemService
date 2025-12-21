namespace SaxsSpot.NanoSystemService.Contracts.Models;

public record RadialAnalysisDto
{
    public Guid Id { get; set; }
    
    public Guid NanosystemId { get; set; }
    
    public Guid ObjectId { get; set; }
    
    public int LayerCount { get; set; }
    
    public int PointCount { get; set; }
    
    public DateTime InputDate { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
}
