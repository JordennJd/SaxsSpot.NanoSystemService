namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.CancelOperation;

public static class OperationType
{
    public const string MassGeneration = "manual-series-run";
    
    private static readonly HashSet<string> SupportedTypes = new()
    {
        MassGeneration,
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
