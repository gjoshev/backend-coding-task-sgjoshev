using Claims.Data;
using Microsoft.EntityFrameworkCore;

namespace Claims.Services;

/// <summary>
/// Service responsible for claim CRUD operations and business rule enforcement.
/// </summary>
public class ClaimsService : IClaimsService
{
    private readonly ClaimsDbContext _dbContext;
    private readonly IAuditService _auditService;

    public ClaimsService(ClaimsDbContext dbContext, IAuditService auditService)
    {
        _dbContext = dbContext;
        _auditService = auditService;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Claim>> GetAllAsync()
    {
        return await _dbContext.Claims.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Claim?> GetByIdAsync(string id)
    {
        return await _dbContext.Claims
            .Where(c => c.Id == id)
            .SingleOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<Claim> CreateAsync(Claim claim)
    {
        var cover = await _dbContext.Covers
            .Where(c => c.Id == claim.CoverId)
            .SingleOrDefaultAsync();

        if (cover is null)
        {
            throw new ValidationException("Cover does not exist.");
        }

        var errors = ClaimValidator.Validate(claim, cover);
        if (errors.Count > 0)
        {
            throw new ValidationException(string.Join(" ", errors));
        }

        claim.Id = Guid.NewGuid().ToString();
        _dbContext.Claims.Add(claim);
        await _dbContext.SaveChangesAsync();

        _auditService.EnqueueAudit(new Auditing.ClaimAudit
        {
            ClaimId = claim.Id,
            Created = DateTime.UtcNow,
            HttpRequestType = "POST"
        });

        return claim;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string id)
    {
        var claim = await GetByIdAsync(id);
        if (claim is not null)
        {
            _dbContext.Claims.Remove(claim);
            await _dbContext.SaveChangesAsync();
        }

        _auditService.EnqueueAudit(new Auditing.ClaimAudit
        {
            ClaimId = id,
            Created = DateTime.UtcNow,
            HttpRequestType = "DELETE"
        });
    }
}
