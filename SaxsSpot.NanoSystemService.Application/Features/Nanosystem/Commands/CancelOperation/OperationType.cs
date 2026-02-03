namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.CancelOperation;

public static class OperationType
{
    private static readonly HashSet<string> SupportedTypes =
    [
        "RunRadialAnalysis",
        "RunGeneration"
    ];

    public static bool IsSupported(string? operationType) =>
        string.IsNullOrEmpty(operationType) || SupportedTypes.Contains(operationType);
}
