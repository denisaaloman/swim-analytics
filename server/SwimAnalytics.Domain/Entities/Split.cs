namespace SwimAnalytics.Domain.Entities;

public class Split
{
    public int DistanceMeters { get; set; }
    public int CumulativeTimeMs { get; set; } 
    public Result Result { get; set; } = null!;
}