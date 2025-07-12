using FintechTestTask.Domain;
using FintechTestTask.Domain.Entities;
using FintechTestTask.Domain.Models;
using FintechTestTask.Infrastructure.EntitiesConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FintechTestTask.Infrastructure;

public class MainDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<GameEntity> Games { get; set; }
    public DbSet<MoveEntity> Moves { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenEntityConfiguration());
        modelBuilder.ApplyConfiguration(new GameEntityConfiguration());
        modelBuilder.ApplyConfiguration(new MoveEntityConfiguration());
    }
    public static ILoggerFactory CreateLoggerFactory() => LoggerFactory.Create(conf => { conf.AddConsole(); });
    
    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var b in
                 ChangeTracker.Entries<BaseEntity>())
        {
            switch (b.State)
            {
                case EntityState.Deleted:
                    b.Entity.IsDeleted = true;
                    b.Entity.DeletedAt = DateTime.UtcNow;

                    b.State = EntityState.Modified;
                    break;
            }
        }
        return base.SaveChangesAsync(ct);
    }
}