using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Domain.Entities;

public class BaseTime
{
    public int BaseTimeMs { get; set; }
    public PoolLength PoolLength { get; set; }
    public Event Event { get; set; } = null!;
}