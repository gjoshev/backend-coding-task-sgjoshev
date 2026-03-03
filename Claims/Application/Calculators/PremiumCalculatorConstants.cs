namespace Claims.Application.Calculators;

internal static class PremiumCalculatorConstants
{
    internal const decimal BaseDayRate = 1250m;

    internal const int FirstTierDays = 30;
    internal const int SecondTierDays = 180;

    internal const decimal YachtMultiplier = 1.1m;
    internal const decimal PassengerShipMultiplier = 1.2m;
    internal const decimal TankerMultiplier = 1.5m;
    internal const decimal DefaultMultiplier = 1.3m;

    internal const decimal YachtFirstTierDiscount = 0.05m;
    internal const decimal DefaultFirstTierDiscount = 0.02m;
    internal const decimal YachtSecondTierDiscount = 0.08m;
    internal const decimal DefaultSecondTierDiscount = 0.03m;
}
