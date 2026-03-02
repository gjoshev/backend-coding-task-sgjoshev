using Claims.Data;
using Claims.Services;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
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

    [Fact]
    public void ComputePremium_DelegatesToPremiumCalculator()
    {
        _premiumCalculatorMock
            .Setup(c => c.Compute(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), CoverType.Yacht))
            .Returns(42000m);

        var service = CreateService(CreateInMemoryContext());

        var result = service.ComputePremium(
            new DateOnly(2025, 1, 1),
            new DateOnly(2025, 2, 1),
            CoverType.Yacht);

        Assert.Equal(42000m, result);
        _premiumCalculatorMock.Verify(
            c => c.Compute(new DateOnly(2025, 1, 1), new DateOnly(2025, 2, 1), CoverType.Yacht),
            Times.Once);
    }

    private CoversService CreateService(ClaimsDbContext context)
    {
        return new CoversService(context, _auditServiceMock.Object, _premiumCalculatorMock.Object);
    }

    private static ClaimsDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ClaimsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ClaimsDbContext(options);
    }
}
