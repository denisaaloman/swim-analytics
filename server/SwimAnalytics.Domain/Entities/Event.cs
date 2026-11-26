using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Domain.Entities;

// ex: 100m Freestyle Male
public class Event
{
    public Stroke Stroke { get; set; }
    public int DistanceMeters { get; set; }
    public Gender Gender { get; set; }

    public ICollection<Result> Results { get; set; } = new List<Result>();
    public ICollection<BaseTime> BaseTimes { get; set; } = new List<BaseTime>();
}