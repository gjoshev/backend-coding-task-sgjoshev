using Claims.Application.Interfaces;
using Claims.Application.Services;
using Claims.Domain.Auditing;
using Claims.Domain.Entities;
using Claims.Domain.Enums;
using Claims.Domain.Exceptions;
using Moq;
using Xunit;

namespace Claims.Tests;

/// <summary>
/// Unit tests for <see cref="CoversService"/>.
/// </summary>
public class CoversServiceTests
{
    private readonly Mock<IAuditService> _auditServiceMock = new();
    private readonly Mock<IPremiumCalculator> _premiumCalculatorMock = new();
    private readonly Mock<ICoversRepository> _coversRepositoryMock = new();

    // -------------------------------------------------------------------------
    // GetAllAsync
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetAllAsync_ReturnsAllCoversFromRepository()
    {
        var covers = new List<Cover>
        {
            new() { Id = "1", StartDate = FutureDate(1), EndDate = FutureDate(30), Type = CoverType.Yacht },
            new() { Id = "2", StartDate = FutureDate(1), EndDate = FutureDate(60), Type = CoverType.PassengerShip }
        };
        _coversRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(covers);

        var result = await CreateService().GetAllAsync();

        Assert.Equal(covers, result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyEnumerable_WhenRepositoryIsEmpty()
    {
        _coversRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

        var result = await CreateService().GetAllAsync();

        Assert.Empty(result);
    }

    // -------------------------------------------------------------------------
    // GetByIdAsync
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetByIdAsync_ReturnsMatchingCover()
    {
        var cover = new Cover { Id = "abc", StartDate = FutureDate(1), EndDate = FutureDate(30), Type = CoverType.Tanker };
        _coversRepositoryMock.Setup(r => r.GetByIdAsync("abc")).ReturnsAsync(cover);

        var result = await CreateService().GetByIdAsync("abc");

        Assert.Equal(cover, result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenCoverDoesNotExist()
    {
        _coversRepositoryMock.Setup(r => r.GetByIdAsync("missing")).ReturnsAsync((Cover?)null);

        var result = await CreateService().GetByIdAsync("missing");

        Assert.Null(result);
    }

    // -------------------------------------------------------------------------
    // CreateAsync
    // -------------------------------------------------------------------------

    [Fact]
    public async Task CreateAsync_AssignsNewId_AndComputedPremium()
    {
        var cover = ValidCover();
        _premiumCalculatorMock
            .Setup(c => c.Compute(cover.StartDate, cover.EndDate, cover.Type))
            .Returns(5000m);

        var result = await CreateService().CreateAsync(cover);

        Assert.False(string.IsNullOrEmpty(result.Id));
        Assert.Equal(5000m, result.Premium);
    }

    [Fact]
    public async Task CreateAsync_CallsRepositoryAddAsync()
    {
        var cover = ValidCover();
        _premiumCalculatorMock
            .Setup(c => c.Compute(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CoverType>()))
            .Returns(1000m);

        await CreateService().CreateAsync(cover);

        _coversRepositoryMock.Verify(r => r.AddAsync(cover), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_EnqueuesCoverAuditWithPostHttpRequestType()
    {
        var cover = ValidCover();
        _premiumCalculatorMock
            .Setup(c => c.Compute(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CoverType>()))
            .Returns(1000m);

        await CreateService().CreateAsync(cover);

        _auditServiceMock.Verify(
            a => a.EnqueueAudit(It.Is<CoverAudit>(audit =>
                audit.CoverId == cover.Id &&
                audit.HttpRequestType == "POST")),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ThrowsValidationException_WhenStartDateIsInPast()
    {
        var cover = new Cover
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(30),
            Type = CoverType.Yacht
        };

        await Assert.ThrowsAsync<ValidationException>(() => CreateService().CreateAsync(cover));
    }

    [Fact]
    public async Task CreateAsync_ThrowsValidationException_WhenPeriodExceeds365Days()
    {
        var cover = new Cover
        {
            StartDate = FutureDate(1),
            EndDate = FutureDate(367),
            Type = CoverType.Yacht
        };

        await Assert.ThrowsAsync<ValidationException>(() => CreateService().CreateAsync(cover));
    }

    [Fact]
    public async Task CreateAsync_DoesNotCallRepository_WhenValidationFails()
    {
        var cover = new Cover
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(30),
            Type = CoverType.Yacht
        };

        await Assert.ThrowsAsync<ValidationException>(() => CreateService().CreateAsync(cover));

        _coversRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Cover>()), Times.Never);
    }

    // -------------------------------------------------------------------------
    // DeleteAsync
    // -------------------------------------------------------------------------

    [Fact]
    public async Task DeleteAsync_CallsRepositoryRemoveAsync_WhenCoverExists()
    {
        var cover = new Cover { Id = "del-1", StartDate = FutureDate(1), EndDate = FutureDate(30), Type = CoverType.Yacht };
        _coversRepositoryMock.Setup(r => r.GetByIdAsync("del-1")).ReturnsAsync(cover);

        await CreateService().DeleteAsync("del-1");

        _coversRepositoryMock.Verify(r => r.RemoveAsync(cover), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DoesNotCallRepositoryRemoveAsync_WhenCoverDoesNotExist()
    {
        _coversRepositoryMock.Setup(r => r.GetByIdAsync("ghost")).ReturnsAsync((Cover?)null);

        await CreateService().DeleteAsync("ghost");

        _coversRepositoryMock.Verify(r => r.RemoveAsync(It.IsAny<Cover>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_EnqueuesCoverAuditWithDeleteHttpRequestType()
    {
        _coversRepositoryMock.Setup(r => r.GetByIdAsync("del-2")).ReturnsAsync((Cover?)null);

        await CreateService().DeleteAsync("del-2");

        _auditServiceMock.Verify(
            a => a.EnqueueAudit(It.Is<CoverAudit>(audit =>
                audit.CoverId == "del-2" &&
                audit.HttpRequestType == "DELETE")),
            Times.Once);
    }

    // -------------------------------------------------------------------------
    // ComputePremium
    // -------------------------------------------------------------------------

    [Fact]
    public void ComputePremium_DelegatesToPremiumCalculator()
    {
        _premiumCalculatorMock
            .Setup(c => c.Compute(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), CoverType.Yacht))
            .Returns(42000m);

        var result = CreateService().ComputePremium(
            new DateOnly(2025, 1, 1),
            new DateOnly(2025, 2, 1),
            CoverType.Yacht);

        Assert.Equal(42000m, result);
        _premiumCalculatorMock.Verify(
            c => c.Compute(new DateOnly(2025, 1, 1), new DateOnly(2025, 2, 1), CoverType.Yacht),
            Times.Once);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static DateOnly FutureDate(int daysFromNow) =>
        DateOnly.FromDateTime(DateTime.UtcNow).AddDays(daysFromNow);

    private static Cover ValidCover() => new()
    {
        StartDate = FutureDate(1),
        EndDate = FutureDate(30),
        Type = CoverType.Yacht
    };

    private CoversService CreateService() =>
        new(_coversRepositoryMock.Object, _auditServiceMock.Object, _premiumCalculatorMock.Object);
}
