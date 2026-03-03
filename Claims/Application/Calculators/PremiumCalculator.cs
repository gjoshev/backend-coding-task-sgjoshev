using Claims.Application.Interfaces;
using Claims.Domain.Enums;

namespace Claims.Application.Calculators;

public class PremiumCalculator : IPremiumCalculator
{
    /// <inheritdoc />
    public decimal Compute(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        var premiumPerDay = PremiumCalculatorConstants.BaseDayRate * GetTypeMultiplier(coverType);
        var insuranceLength = endDate.DayNumber - startDate.DayNumber;
        var totalPremium = 0m;

        for (var i = 0; i < insuranceLength; i++)
        {
            totalPremium += GetDailyRate(i, premiumPerDay, coverType);
        }

        return totalPremium;
    }

    private static decimal GetTypeMultiplier(CoverType coverType)
    {
        return coverType switch
        {
            CoverType.Yacht => PremiumCalculatorConstants.YachtMultiplier,
            CoverType.PassengerShip => PremiumCalculatorConstants.PassengerShipMultiplier,
            CoverType.Tanker => PremiumCalculatorConstants.TankerMultiplier,
            _ => PremiumCalculatorConstants.DefaultMultiplier
        };
    }

    private static decimal GetDailyRate(int dayIndex, decimal basePremiumPerDay, CoverType coverType)
    {
        if (dayIndex < PremiumCalculatorConstants.FirstTierDays)
        {
            return basePremiumPerDay;
        }

        if (dayIndex < PremiumCalculatorConstants.SecondTierDays)
        {
            var discount = coverType == CoverType.Yacht
                ? PremiumCalculatorConstants.YachtFirstTierDiscount
                : PremiumCalculatorConstants.DefaultFirstTierDiscount;
            return basePremiumPerDay * (1m - discount);
        }

        var lastDiscount = coverType == CoverType.Yacht
            ? PremiumCalculatorConstants.YachtSecondTierDiscount
            : PremiumCalculatorConstants.DefaultSecondTierDiscount;
        return basePremiumPerDay * (1m - lastDiscount);
    }
}
