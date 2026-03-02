namespace Claims.Services;

/// <summary>
/// Defines operations for managing insurance claims.
/// </summary>
public interface IClaimsService
{
    /// <summary>
    /// Retrieves all claims.
    /// </summary>
    Task<IEnumerable<Claim>> GetAllAsync();

    /// <summary>
    /// Retrieves a single claim by its identifier.
    /// </summary>
    /// <param name="id">The claim identifier.</param>
    Task<Claim?> GetByIdAsync(string id);

    /// <summary>
    /// Creates a new claim after validation.
    /// </summary>
    /// <param name="claim">The claim to create.</param>
    Task<Claim> CreateAsync(Claim claim);

    /// <summary>
    /// Deletes a claim by its identifier.
    /// </summary>
    /// <param name="id">The claim identifier.</param>
    Task DeleteAsync(string id);
}
