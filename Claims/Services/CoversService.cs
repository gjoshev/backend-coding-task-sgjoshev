using Claims.Data;
using Microsoft.EntityFrameworkCore;

namespace Claims.Services;

/// <summary>
/// Service responsible for cover CRUD operations, validation, and premium computation.
/// </summary>
public class CoversService : ICoversService
{
    private readonly ClaimsDbContext _dbContext;
    private readonly IAuditService _auditService;
    private readonly IPremiumCalculator _premiumCalculator;

    public CoversService(ClaimsDbContext dbContext, IAuditService auditService, IPremiumCalculator premiumCalculator)
    {
        _dbContext = dbContext;
        _auditService = auditService;
        _premiumCalculator = premiumCalculator;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Cover>> GetAllAsync()
    {
        return await _dbContext.Covers.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Cover?> GetByIdAsync(string id)
    {
        return await _dbContext.Covers
            .Where(c => c.Id == id)
            .SingleOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<Cover> CreateAsync(Cover cover)
    {
        var errors = CoverValidator.Validate(cover);
        if (errors.Count > 0)
        {
            throw new ValidationException(string.Join(" ", errors));
        }

        cover.Id = Guid.NewGuid().ToString();
        cover.Premium = _premiumCalculator.Compute(cover.StartDate, cover.EndDate, cover.Type);

        _dbContext.Covers.Add(cover);
        await _dbContext.SaveChangesAsync();

        _auditService.EnqueueAudit(new Auditing.CoverAudit
        {
            CoverId = cover.Id,
            Created = DateTime.UtcNow,
            HttpRequestType = "POST"
        });

        return cover;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string id)
    {
        var cover = await GetByIdAsync(id);
        if (cover is not null)
        {
            _dbContext.Covers.Remove(cover);
            await _dbContext.SaveChangesAsync();
        }

        _auditService.EnqueueAudit(new Auditing.CoverAudit
        {
            CoverId = id,
            Created = DateTime.UtcNow,
            HttpRequestType = "DELETE"
        });
    }

    /// <inheritdoc />
    public decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        return _premiumCalculator.Compute(startDate, endDate, coverType);
    }
}
