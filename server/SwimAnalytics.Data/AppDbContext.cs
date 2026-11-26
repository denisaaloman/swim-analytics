using Microsoft.EntityFrameworkCore;
using SwimAnalytics.Domain.Entities;

namespace SwimAnalytics.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Club> Clubs => Set<Club>();
    public DbSet<Swimmer> Swimmers => Set<Swimmer>();
    public DbSet<Meet> Meets => Set<Meet>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Result> Results => Set<Result>();
    public DbSet<Split> Splits => Set<Split>();
    public DbSet<BaseTime> BaseTimes => Set<BaseTime>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

}