namespace Claims.Services;

/// <summary>
/// Computes insurance cover premiums based on cover type and insurance period length.
/// </summary>
/// <remarks>
/// Premium logic:
/// <list type="bullet">
///   <item>Base day rate: 1250</item>
///   <item>Yacht: +10%, Passenger ship: +20%, Tanker: +50%, other types: +30%</item>
///   <item>First 30 days: base rate</item>
///   <item>Days 31–180: 5% discount for Yacht, 2% for others</item>
///   <item>Days 181+: additional 3% discount for Yacht (total 8%), additional 1% for others (total 3%)</item>
/// </list>
/// </remarks>
public class PremiumCalculator : IPremiumCalculator
{
    private const decimal BaseDayRate = 1250m;
    private const int FirstTierDays = 30;
    private const int SecondTierDays = 180;

    /// <inheritdoc />
    public decimal Compute(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        var premiumPerDay = BaseDayRate * GetTypeMultiplier(coverType);
        var insuranceLength = endDate.DayNumber - startDate.DayNumber;
        var totalPremium = 0m;

        for (var i = 0; i < insuranceLength; i++)
        {
            totalPremium += GetDailyRate(i, premiumPerDay, coverType);
        }

        return totalPremium;
    }

    /// <summary>
    /// Returns the type multiplier for a given cover type.
    /// </summary>
    private static decimal GetTypeMultiplier(CoverType coverType)
    {
        return coverType switch
        {
            CoverType.Yacht => 1.1m,
            CoverType.PassengerShip => 1.2m,
            CoverType.Tanker => 1.5m,
            _ => 1.3m
        };
    }

    /// <summary>
    /// Returns the premium rate for a specific day index, applying progressive discounts.
    /// </summary>
    private static decimal GetDailyRate(int dayIndex, decimal basePremiumPerDay, CoverType coverType)
    {
        if (dayIndex < FirstTierDays)
        {
            // First 30 days: no discount
            return basePremiumPerDay;
        }

        if (dayIndex < SecondTierDays)
        {
            // Days 31–180: 5% discount for Yacht, 2% for others
            var discount = coverType == CoverType.Yacht ? 0.05m : 0.02m;
            return basePremiumPerDay * (1m - discount);
        }

        // Days 181+: 8% discount for Yacht (5%+3%), 3% discount for others (2%+1%)
        {
            var discount = coverType == CoverType.Yacht ? 0.08m : 0.03m;
            return basePremiumPerDay * (1m - discount);
        }
    }
}
