using Claims.Domain.Entities;

namespace Claims.Application.Interfaces;

/// <summary>
/// Defines data access operations for <see cref="Cover"/> entities.
/// </summary>
public interface ICoversRepository
{
    /// <summary>Retrieves all covers.</summary>
    Task<IEnumerable<Cover>> GetAllAsync();

    /// <summary>Retrieves a cover by its identifier.</summary>
    Task<Cover?> GetByIdAsync(string id);

    /// <summary>Adds a new cover to the store.</summary>
    Task AddAsync(Cover cover);

    /// <summary>Removes a cover from the store.</summary>
    Task RemoveAsync(Cover cover);
}
