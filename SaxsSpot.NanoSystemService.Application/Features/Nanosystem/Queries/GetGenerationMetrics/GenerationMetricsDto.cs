namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetGenerationMetrics;

public record GenerationMetricsDto
{
    public Guid Id { get; init; }
    public Guid NanosystemId { get; init; }
    public int ParticleIndex { get; init; } // Index of the particle in nanosystem
    public int TotalAttempts { get; init; }
    public int PositiveAttempts { get; init; }
    public int TotalChangePositionAttempts { get; init; }
    public long GenerationTimeMs { get; init; }
    public double Volume { get; init; }
    public float Diameter { get; init; }
    public int ParticlesCheckedForIntersection { get; init; }
    public int OutOfZoneAttempts { get; init; }
    public int FirstNodeIntersectionFindTimes { get; init; }
    public int TotalNeighborsNodesCheckedCount { get; init; }
    public int IsInterCenterDistanceMoreThenDiagonalCheckTimesPositive { get; init; }
    public int IsInterCenterDistanceMoreThenDiagonalCheckTimesTotal { get; init; }
    public int IsInterCenterDistanceLessThenSidesCheckTimesPositive { get; init; }
    public int IsInterCenterDistanceLessThenSidesCheckTimesTotal { get; init; }
    public int ElementaryIntersectCheckOnlyBordersNewTransformationTimesPositive { get; init; }
    public int ElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal { get; init; }
    public int ElementaryIntersectCheckOnlyBordersOldTransformationTimesPositive { get; init; }
    public int ElementaryIntersectCheckOnlyBordersOldTransformationTimesTotal { get; init; }
    public int BackRotateMatrixReused { get; init; }
    public int SATCheckTimesPositive { get; init; }
    public int SATCheckTimesTotal { get; init; }
    
    // Calculated properties
    public double InsertionEfficiency => TotalAttempts > 0 ? (double)PositiveAttempts / TotalAttempts : 0;
    public double FirstNodeIntersectionEfficiency => TotalAttempts > 0 ? (double)FirstNodeIntersectionFindTimes / TotalAttempts : 0;
    public double AverageNeighborsCheckedPerAttempt => TotalAttempts > 0 ? (double)TotalNeighborsNodesCheckedCount / TotalAttempts : 0;
    public double DiagonalDistanceCheckEfficiency => IsInterCenterDistanceMoreThenDiagonalCheckTimesTotal > 0 
        ? (double)IsInterCenterDistanceMoreThenDiagonalCheckTimesPositive / IsInterCenterDistanceMoreThenDiagonalCheckTimesTotal : 0;
    public double SidesDistanceCheckEfficiency => IsInterCenterDistanceLessThenSidesCheckTimesTotal > 0 
        ? (double)IsInterCenterDistanceLessThenSidesCheckTimesPositive / IsInterCenterDistanceLessThenSidesCheckTimesTotal : 0;
    public double ElementaryIntersectionNewTransformationEfficiency => ElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal > 0 
        ? (double)ElementaryIntersectCheckOnlyBordersNewTransformationTimesPositive / ElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal : 0;
    public double ElementaryIntersectionOldTransformationEfficiency => ElementaryIntersectCheckOnlyBordersOldTransformationTimesTotal > 0 
        ? (double)ElementaryIntersectCheckOnlyBordersOldTransformationTimesPositive / ElementaryIntersectCheckOnlyBordersOldTransformationTimesTotal : 0;
    public double BackRotateMatrixReusedEfficiency => ElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal > 0 
        ? (double)BackRotateMatrixReused / ElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal : 0;
    public double SATCheckEfficiency => SATCheckTimesTotal > 0 
        ? (double)SATCheckTimesPositive / SATCheckTimesTotal : 0;
    public double AverageParticlesCheckedPerAttempt => TotalAttempts > 0 
        ? (double)ParticlesCheckedForIntersection / TotalAttempts : 0;
    public double OutOfZoneAttemptsRatio => TotalAttempts > 0 
        ? (double)OutOfZoneAttempts / TotalAttempts : 0;
    public int InZoneAttempts => TotalAttempts - OutOfZoneAttempts;
}
