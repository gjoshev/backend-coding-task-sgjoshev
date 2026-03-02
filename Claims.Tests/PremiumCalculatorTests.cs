using Claims.Services;
using Xunit;

namespace Claims.Tests;

/// <summary>
/// Unit tests for <see cref="PremiumCalculator"/>.
/// </summary>
public class PremiumCalculatorTests
{
    private readonly PremiumCalculator _calculator = new();

    [Fact]
    public void Compute_SingleDay_Yacht_ReturnsBaseRate()
    {
        // Yacht multiplier = 1.1, base = 1250, 1 day => 1250 * 1.1 = 1375
        var result = _calculator.Compute(
            new DateOnly(2025, 1, 1),
            new DateOnly(2025, 1, 2),
            CoverType.Yacht);

        Assert.Equal(1375m, result);
    }

    [Fact]
    public void Compute_SingleDay_PassengerShip_ReturnsBaseRate()
    {
        // PassengerShip multiplier = 1.2, base = 1250, 1 day => 1500
        var result = _calculator.Compute(
            new DateOnly(2025, 1, 1),
            new DateOnly(2025, 1, 2),
            CoverType.PassengerShip);

        Assert.Equal(1500m, result);
    }

    [Fact]
    public void Compute_SingleDay_Tanker_ReturnsBaseRate()
    {
        // Tanker multiplier = 1.5, base = 1250, 1 day => 1875
        var result = _calculator.Compute(
            new DateOnly(2025, 1, 1),
            new DateOnly(2025, 1, 2),
            CoverType.Tanker);

        Assert.Equal(1875m, result);
    }

    [Fact]
    public void Compute_SingleDay_ContainerShip_ReturnsBaseRateWithDefaultMultiplier()
    {
        // Default multiplier = 1.3, base = 1250, 1 day => 1625
        var result = _calculator.Compute(
            new DateOnly(2025, 1, 1),
            new DateOnly(2025, 1, 2),
            CoverType.ContainerShip);

        Assert.Equal(1625m, result);
    }

    [Fact]
    public void Compute_SingleDay_BulkCarrier_ReturnsBaseRateWithDefaultMultiplier()
    {
        // BulkCarrier uses default multiplier = 1.3
        var result = _calculator.Compute(
            new DateOnly(2025, 1, 1),
            new DateOnly(2025, 1, 2),
            CoverType.BulkCarrier);

        Assert.Equal(1625m, result);
    }

    [Fact]
    public void Compute_30Days_Yacht_AllAtBaseRate()
    {
        // 30 days, all in first tier (no discount)
        var result = _calculator.Compute(
            new DateOnly(2025, 1, 1),
            new DateOnly(2025, 1, 31),
            CoverType.Yacht);

        Assert.Equal(1375m * 30, result);
    }

    [Fact]
    public void Compute_31Days_Yacht_SecondTierDiscountApplies()
    {
        // 30 days at base rate + 1 day at 5% discount
        var basePremium = 1250m * 1.1m;
        var expected = basePremium * 30 + basePremium * 0.95m;

        var result = _calculator.Compute(
            new DateOnly(2025, 1, 1),
            new DateOnly(2025, 2, 1),
            CoverType.Yacht);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Compute_31Days_ContainerShip_SecondTierDiscountApplies()
    {
        // 30 days at base rate + 1 day at 2% discount
        var basePremium = 1250m * 1.3m;
        var expected = basePremium * 30 + basePremium * 0.98m;

        var result = _calculator.Compute(
            new DateOnly(2025, 1, 1),
            new DateOnly(2025, 2, 1),
            CoverType.ContainerShip);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Compute_180Days_Yacht_CorrectTierApplication()
    {
        // 30 days at base + 150 days at 5% discount
        var basePremium = 1250m * 1.1m;
        var expected = basePremium * 30 + basePremium * 0.95m * 150;

        var result = _calculator.Compute(
            new DateOnly(2025, 1, 1),
            new DateOnly(2025, 6, 30),
            CoverType.Yacht);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Compute_181Days_Yacht_ThirdTierDiscountApplies()
    {
        // 30 days at base + 150 days at 5% discount + 1 day at 8% discount
        var basePremium = 1250m * 1.1m;
        var expected = basePremium * 30
            + basePremium * 0.95m * 150
            + basePremium * 0.92m;

        var result = _calculator.Compute(
            new DateOnly(2025, 1, 1),
            new DateOnly(2025, 7, 1),
            CoverType.Yacht);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Compute_181Days_BulkCarrier_ThirdTierDiscountApplies()
    {
        // 30 days at base + 150 days at 2% discount + 1 day at 3% discount
        var basePremium = 1250m * 1.3m;
        var expected = basePremium * 30
            + basePremium * 0.98m * 150
            + basePremium * 0.97m;

        var result = _calculator.Compute(
            new DateOnly(2025, 1, 1),
            new DateOnly(2025, 7, 1),
            CoverType.BulkCarrier);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Compute_365Days_Yacht_FullPeriod()
    {
        // 30 + 150 + 185
        var basePremium = 1250m * 1.1m;
        var expected = basePremium * 30
            + basePremium * 0.95m * 150
            + basePremium * 0.92m * 185;

        var result = _calculator.Compute(
            new DateOnly(2025, 1, 1),
            new DateOnly(2026, 1, 1),
            CoverType.Yacht);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Compute_365Days_Tanker_FullPeriod()
    {
        // 30 + 150 + 185
        var basePremium = 1250m * 1.5m;
        var expected = basePremium * 30
            + basePremium * 0.98m * 150
            + basePremium * 0.97m * 185;

        var result = _calculator.Compute(
            new DateOnly(2025, 1, 1),
            new DateOnly(2026, 1, 1),
            CoverType.Tanker);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Compute_ZeroDays_ReturnsZero()
    {
        var result = _calculator.Compute(
            new DateOnly(2025, 1, 1),
            new DateOnly(2025, 1, 1),
            CoverType.Yacht);

        Assert.Equal(0m, result);
    }
}
