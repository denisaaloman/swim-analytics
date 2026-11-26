using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Domain.Entities;
public class Swimmer
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public Gender Gender { get; set; }
    public Club? Club { get; set; }
    public ICollection<Result> Results { get; set; } = new List<Result>();

}