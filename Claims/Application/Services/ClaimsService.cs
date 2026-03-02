using Claims.Application.Interfaces;
using Claims.Application.Validators;
using Claims.Domain.Auditing;
using Claims.Domain.Entities;
using Claims.Domain.Exceptions;

namespace Claims.Application.Services;

/// <summary>
/// Service responsible for claim CRUD operations and business rule enforcement.
/// </summary>
public class ClaimsService : IClaimsService
{
    private readonly IClaimsRepository _claimsRepository;
    private readonly ICoversRepository _coversRepository;
    private readonly IAuditService _auditService;

    public ClaimsService(IClaimsRepository claimsRepository, ICoversRepository coversRepository, IAuditService auditService)
    {
        _claimsRepository = claimsRepository;
        _coversRepository = coversRepository;
        _auditService = auditService;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Claim>> GetAllAsync()
    {
        return await _claimsRepository.GetAllAsync();
    }

    /// <inheritdoc />
    public async Task<Claim?> GetByIdAsync(string id)
    {
        return await _claimsRepository.GetByIdAsync(id);
    }

    /// <inheritdoc />
    public async Task<Claim> CreateAsync(Claim claim)
    {
        var cover = await _coversRepository.GetByIdAsync(claim.CoverId);

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
        await _claimsRepository.AddAsync(claim);

        _auditService.EnqueueAudit(new ClaimAudit
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
            await _claimsRepository.RemoveAsync(claim);
        }

        _auditService.EnqueueAudit(new ClaimAudit
        {
            ClaimId = id,
            Created = DateTime.UtcNow,
            HttpRequestType = "DELETE"
        });
    }
}
