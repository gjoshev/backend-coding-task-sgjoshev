using Claims.Domain.Entities;
using Claims.Domain.Enums;

namespace Claims.Application.Validators;

/// <summary>
/// Validates <see cref="Cover"/> entities against business rules.
/// </summary>
public static class CoverValidator
{
    private const int MaxInsurancePeriodDays = 365;

    /// <summary>
    /// Validates a cover and returns a list of error messages. An empty list indicates the cover is valid.
    /// </summary>
    /// <param name="cover">The cover to validate.</param>
    /// <returns>A list of validation error messages.</returns>
    public static List<string> Validate(Cover cover)
    {
        var errors = new List<string>();

        if (cover.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            errors.Add("StartDate cannot be in the past.");
        }

        var insurancePeriodDays = cover.EndDate.DayNumber - cover.StartDate.DayNumber;
        if (insurancePeriodDays > MaxInsurancePeriodDays)
        {
            errors.Add($"Total insurance period cannot exceed {MaxInsurancePeriodDays} days.");
        }

        return errors;
    }
}
