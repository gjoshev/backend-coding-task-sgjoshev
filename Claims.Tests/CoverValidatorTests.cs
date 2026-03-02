using Claims.Application.Validators;
using Claims.Domain.Entities;
using Claims.Domain.Enums;
using Xunit;

namespace Claims.Tests;

/// <summary>
/// Unit tests for <see cref="CoverValidator"/>.
/// </summary>
public class CoverValidatorTests
{
    [Fact]
    public void Validate_ValidCover_ReturnsNoErrors()
    {
        var cover = new Cover
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(30),
            Type = CoverType.Yacht
        };

        var errors = CoverValidator.Validate(cover);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_StartDateInPast_ReturnsError()
    {
        var cover = new Cover
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(30),
            Type = CoverType.Yacht
        };

        var errors = CoverValidator.Validate(cover);

        Assert.Single(errors);
        Assert.Contains("past", errors[0]);
    }

    [Fact]
    public void Validate_PeriodExceedsOneYear_ReturnsError()
    {
        var cover = new Cover
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(366),
            Type = CoverType.Tanker
        };

        var errors = CoverValidator.Validate(cover);

        Assert.Single(errors);
        Assert.Contains("365", errors[0]);
    }

    [Fact]
    public void Validate_PeriodExactly365Days_ReturnsNoErrors()
    {
        var cover = new Cover
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(365),
            Type = CoverType.ContainerShip
        };

        var errors = CoverValidator.Validate(cover);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_StartDateToday_ReturnsNoErrors()
    {
        var cover = new Cover
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10),
            Type = CoverType.BulkCarrier
        };

        var errors = CoverValidator.Validate(cover);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_MultipleViolations_ReturnsMultipleErrors()
    {
        var cover = new Cover
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-10),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(400),
            Type = CoverType.Yacht
        };

        var errors = CoverValidator.Validate(cover);

        Assert.Equal(2, errors.Count);
    }
}
