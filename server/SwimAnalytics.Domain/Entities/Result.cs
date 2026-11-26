namespace SwimAnalytics.Domain.Entities;

public class Result
{
    public int TimeMs { get; set; }

    public Swimmer Swimmer { get; set; } = null!;
    public Event Event { get; set; } = null!;

    public Meet Meet { get; set; } = null!;

    public ICollection<Split> Splits { get; set; } = new List<Split>();
}