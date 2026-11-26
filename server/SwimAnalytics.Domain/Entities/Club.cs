namespace SwimAnalytics.Domain.Entities;

public class Club
{
    public string Name { get; set; } = string.Empty;
    public string? City { get; set; }

    public ICollection<Swimmer> Swimmers { get; set; } = new List<Swimmer>();
}