using Claims.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims.Infrastructure.Data;

/// <summary>
/// Entity Framework DbContext for the MongoDB-backed claims and covers data store.
/// </summary>
public class ClaimsDbContext : DbContext
{
    public DbSet<Claim> Claims { get; init; }
    public DbSet<Cover> Covers { get; init; }

    public ClaimsDbContext(DbContextOptions<ClaimsDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Claim>().ToCollection("claims");
        modelBuilder.Entity<Cover>().ToCollection("covers");
    }
}
