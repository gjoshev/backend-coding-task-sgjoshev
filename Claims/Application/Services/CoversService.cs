using Claims.Application.Interfaces;
using Claims.Application.Validators;
using Claims.Domain.Auditing;
using Claims.Domain.Entities;
using Claims.Domain.Enums;
using Claims.Domain.Exceptions;

namespace Claims.Application.Services;

/// <summary>
/// Service responsible for cover CRUD operations, validation, and premium computation.
/// </summary>
public class CoversService : ICoversService
{
    private readonly ICoversRepository _coversRepository;
    private readonly IAuditService _auditService;
    private readonly IPremiumCalculator _premiumCalculator;

    public CoversService(ICoversRepository coversRepository, IAuditService auditService, IPremiumCalculator premiumCalculator)
    {
        _coversRepository = coversRepository;
        _auditService = auditService;
        _premiumCalculator = premiumCalculator;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Cover>> GetAllAsync()
    {
        return await _coversRepository.GetAllAsync();
    }

    /// <inheritdoc />
    public async Task<Cover?> GetByIdAsync(string id)
    {
        return await _coversRepository.GetByIdAsync(id);
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

        await _coversRepository.AddAsync(cover);

        _auditService.EnqueueAudit(new CoverAudit
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
            await _coversRepository.RemoveAsync(cover);
        }

        _auditService.EnqueueAudit(new CoverAudit
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
