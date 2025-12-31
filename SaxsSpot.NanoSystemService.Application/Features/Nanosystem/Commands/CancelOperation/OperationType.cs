namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.CancelOperation;

public static class OperationType
{
    public const string MassGeneration = "manual-series-run";
    public const string RadialAnalysis = "RunRadialAnalysis";
    
    private static readonly HashSet<string> SupportedTypes = new()
    {
        MassGeneration,
        RadialAnalysis
    };
    
    public static bool IsSupported(string operationType)
    {
        return SupportedTypes.Contains(operationType);
    }
    
    public static IReadOnlySet<string> GetSupportedTypes()
    {
        return SupportedTypes;
    }
}
