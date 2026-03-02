using Claims.Domain.Entities;

namespace Claims.Application.Interfaces;

/// <summary>
/// Defines data access operations for <see cref="Claim"/> entities.
/// </summary>
public interface IClaimsRepository
{
    /// <summary>Retrieves all claims.</summary>
    Task<IEnumerable<Claim>> GetAllAsync();

    /// <summary>Retrieves a claim by its identifier.</summary>
    Task<Claim?> GetByIdAsync(string id);

    /// <summary>Adds a new claim to the store.</summary>
    Task AddAsync(Claim claim);

    /// <summary>Removes a claim from the store.</summary>
    Task RemoveAsync(Claim claim);
}
