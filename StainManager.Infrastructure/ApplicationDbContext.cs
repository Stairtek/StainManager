using StainManager.Domain.Species;
using StainManager.Domain.Textures;

namespace StainManager.Infrastructure;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Species> Species { get; set; }
    
    public DbSet<Texture> Textures { get; set; }
}