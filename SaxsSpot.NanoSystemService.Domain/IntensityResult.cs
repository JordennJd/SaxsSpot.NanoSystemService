namespace SaxsSpot.NanoSystemService.Domain;

public class IntensityResult(double qVector, double intensity)
{
    public double QVector { get; set; } = qVector;

    public double Intensity { get; set; } = intensity;

    public override string ToString() => $"{QVector} {Intensity}";
}
