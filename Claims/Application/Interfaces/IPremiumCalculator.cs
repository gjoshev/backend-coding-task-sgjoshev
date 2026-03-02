using Claims.Domain.Enums;

namespace Claims.Application.Interfaces;

/// <summary>
/// Defines the contract for computing insurance cover premiums.
/// </summary>
public interface IPremiumCalculator
{
    /// <summary>
    /// Computes the total premium for a cover based on date range and cover type.
    /// </summary>
    /// <param name="startDate">The start date of the cover.</param>
    /// <param name="endDate">The end date of the cover.</param>
    /// <param name="coverType">The type of the covered object.</param>
    /// <returns>The total premium amount.</returns>
    decimal Compute(DateOnly startDate, DateOnly endDate, CoverType coverType);
}
