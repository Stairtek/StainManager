using StainManager.Domain.Species;

namespace StainManager.Infrastructure;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Species> Species { get; set; }
}