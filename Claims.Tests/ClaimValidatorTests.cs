using Claims.Services;
using Xunit;

namespace Claims.Tests;

/// <summary>
/// Unit tests for <see cref="ClaimValidator"/>.
/// </summary>
public class ClaimValidatorTests
{
    private static Cover CreateValidCover()
    {
        return new Cover
        {
            Id = "cover-1",
            StartDate = new DateOnly(2025, 1, 1),
            EndDate = new DateOnly(2025, 12, 31),
            Type = CoverType.Yacht,
            Premium = 100000m
        };
    }

    [Fact]
    public void Validate_ValidClaim_ReturnsNoErrors()
    {
        var cover = CreateValidCover();
        var claim = new Claim
        {
            CoverId = cover.Id,
            Created = new DateOnly(2025, 6, 15),
            Name = "Test Claim",
            Type = ClaimType.Collision,
            DamageCost = 50000m
        };

        var errors = ClaimValidator.Validate(claim, cover);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_DamageCostExceeds100000_ReturnsError()
    {
        var cover = CreateValidCover();
        var claim = new Claim
        {
            CoverId = cover.Id,
            Created = new DateOnly(2025, 6, 15),
            Name = "Test Claim",
            Type = ClaimType.Collision,
            DamageCost = 100001m
        };

        var errors = ClaimValidator.Validate(claim, cover);

        Assert.Single(errors);
        Assert.Contains("DamageCost", errors[0]);
    }

    [Fact]
    public void Validate_DamageCostExactly100000_ReturnsNoErrors()
    {
        var cover = CreateValidCover();
        var claim = new Claim
        {
            CoverId = cover.Id,
            Created = new DateOnly(2025, 6, 15),
            Name = "Test Claim",
            Type = ClaimType.Collision,
            DamageCost = 100000m
        };

        var errors = ClaimValidator.Validate(claim, cover);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_CreatedBeforeCoverStart_ReturnsError()
    {
        var cover = CreateValidCover();
        var claim = new Claim
        {
            CoverId = cover.Id,
            Created = new DateOnly(2024, 12, 31),
            Name = "Test Claim",
            Type = ClaimType.Collision,
            DamageCost = 5000m
        };

        var errors = ClaimValidator.Validate(claim, cover);

        Assert.Single(errors);
        Assert.Contains("within the period", errors[0]);
    }

    [Fact]
    public void Validate_CreatedAfterCoverEnd_ReturnsError()
    {
        var cover = CreateValidCover();
        var claim = new Claim
        {
            CoverId = cover.Id,
            Created = new DateOnly(2026, 1, 1),
            Name = "Test Claim",
            Type = ClaimType.Collision,
            DamageCost = 5000m
        };

        var errors = ClaimValidator.Validate(claim, cover);

        Assert.Single(errors);
        Assert.Contains("within the period", errors[0]);
    }

    [Fact]
    public void Validate_CreatedOnCoverStartDate_ReturnsNoErrors()
    {
        var cover = CreateValidCover();
        var claim = new Claim
        {
            CoverId = cover.Id,
            Created = cover.StartDate,
            Name = "Test Claim",
            Type = ClaimType.Collision,
            DamageCost = 5000m
        };

        var errors = ClaimValidator.Validate(claim, cover);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_CreatedOnCoverEndDate_ReturnsNoErrors()
    {
        var cover = CreateValidCover();
        var claim = new Claim
        {
            CoverId = cover.Id,
            Created = cover.EndDate,
            Name = "Test Claim",
            Type = ClaimType.Collision,
            DamageCost = 5000m
        };

        var errors = ClaimValidator.Validate(claim, cover);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_MultipleViolations_ReturnsMultipleErrors()
    {
        var cover = CreateValidCover();
        var claim = new Claim
        {
            CoverId = cover.Id,
            Created = new DateOnly(2024, 1, 1),
            Name = "Test Claim",
            Type = ClaimType.Fire,
            DamageCost = 200000m
        };

        var errors = ClaimValidator.Validate(claim, cover);

        Assert.Equal(2, errors.Count);
    }
}
