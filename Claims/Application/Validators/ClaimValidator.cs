using Claims.Domain.Entities;
using Claims.Domain.Enums;

namespace Claims.Application.Validators;

/// <summary>
/// Validates <see cref="Claim"/> entities against business rules.
/// </summary>
public static class ClaimValidator
{
    private const decimal MaxDamageCost = 100_000m;

    /// <summary>
    /// Validates a claim and returns a list of error messages. An empty list indicates the claim is valid.
    /// </summary>
    /// <param name="claim">The claim to validate.</param>
    /// <param name="cover">The related cover.</param>
    /// <returns>A list of validation error messages.</returns>
    public static List<string> Validate(Claim claim, Cover cover)
    {
        var errors = new List<string>();

        if (claim.DamageCost > MaxDamageCost)
        {
            errors.Add($"DamageCost cannot exceed {MaxDamageCost}.");
        }

        if (claim.Created < cover.StartDate || claim.Created > cover.EndDate)
        {
            errors.Add("Created date must be within the period of the related cover.");
        }

        return errors;
    }
}
