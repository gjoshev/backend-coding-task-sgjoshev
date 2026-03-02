using Claims.Application.Interfaces;
using Claims.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Data;

/// <summary>
/// EF Core implementation of <see cref="ICoversRepository"/> backed by MongoDB via <see cref="ClaimsDbContext"/>.
/// </summary>
public class CoversRepository : ICoversRepository
{
    private readonly ClaimsDbContext _dbContext;

    public CoversRepository(ClaimsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Cover>> GetAllAsync()
    {
        return await _dbContext.Covers.ToListAsync();
    }

    public async Task<Cover?> GetByIdAsync(string id)
    {
        return await _dbContext.Covers
            .Where(c => c.Id == id)
            .SingleOrDefaultAsync();
    }

    public async Task AddAsync(Cover cover)
    {
        _dbContext.Covers.Add(cover);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(Cover cover)
    {
        _dbContext.Covers.Remove(cover);
        await _dbContext.SaveChangesAsync();
    }
}
