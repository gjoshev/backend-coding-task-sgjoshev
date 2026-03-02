using Claims.Application.Interfaces;
using Claims.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Data;

/// <summary>
/// EF Core implementation of <see cref="IClaimsRepository"/> backed by MongoDB via <see cref="ClaimsDbContext"/>.
/// </summary>
public class ClaimsRepository : IClaimsRepository
{
    private readonly ClaimsDbContext _dbContext;

    public ClaimsRepository(ClaimsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Claim>> GetAllAsync()
    {
        return await _dbContext.Claims.ToListAsync();
    }

    public async Task<Claim?> GetByIdAsync(string id)
    {
        return await _dbContext.Claims
            .Where(c => c.Id == id)
            .SingleOrDefaultAsync();
    }

    public async Task AddAsync(Claim claim)
    {
        _dbContext.Claims.Add(claim);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(Claim claim)
    {
        _dbContext.Claims.Remove(claim);
        await _dbContext.SaveChangesAsync();
    }
}
