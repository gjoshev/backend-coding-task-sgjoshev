using Claims.Domain.Entities;
using Claims.Domain.Enums;

namespace Claims.Application.Interfaces;

/// <summary>
/// Defines operations for managing insurance covers.
/// </summary>
public interface ICoversService
{
    /// <summary>
    /// Retrieves all covers.
    /// </summary>
    Task<IEnumerable<Cover>> GetAllAsync();

    /// <summary>
    /// Retrieves a single cover by its identifier.
    /// </summary>
    /// <param name="id">The cover identifier.</param>
    Task<Cover?> GetByIdAsync(string id);

    /// <summary>
    /// Creates a new cover after validation and premium computation.
    /// </summary>
    /// <param name="cover">The cover to create.</param>
    Task<Cover> CreateAsync(Cover cover);

    /// <summary>
    /// Deletes a cover by its identifier.
    /// </summary>
    /// <param name="id">The cover identifier.</param>
    Task DeleteAsync(string id);

    /// <summary>
    /// Computes the premium for the given parameters.
    /// </summary>
    decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverType coverType);
}
