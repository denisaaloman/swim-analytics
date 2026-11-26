using SwimAnalytics.Domain.Enums;
namespace SwimAnalytics.Domain.Entities;

public class Meet
{
    public string Name { get; set; } = string.Empty;
    public string? City { get; set; }
    public DateOnly Date { get; set; }
    public PoolLength PoolLength { get; set; }
    public ICollection<Result> Results { get; set; } = new List<Result>();
}

