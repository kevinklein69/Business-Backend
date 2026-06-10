using Business.Application.Common;

namespace Business.UnitTests;

public class ProbationCalculatorTests
{
    [Fact]
    public void CalculateEnd_WithoutProbationMonths_ReturnsNull()
    {
        var entryDate = new DateOnly(2026, 1, 15);

        var result = ProbationCalculator.CalculateEnd(entryDate, null);

        Assert.Null(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(6)]
    public void CalculateEnd_WithProbationMonths_AddsMonthsToEntryDate(int months)
    {
        var entryDate = new DateOnly(2026, 1, 15);

        var result = ProbationCalculator.CalculateEnd(entryDate, months);

        Assert.Equal(entryDate.AddMonths(months), result);
    }

    [Fact]
    public void CalculateEnd_AtMonthEnd_ClampsToShorterMonth()
    {
        var entryDate = new DateOnly(2026, 1, 31);

        var result = ProbationCalculator.CalculateEnd(entryDate, 1);

        Assert.Equal(new DateOnly(2026, 2, 28), result);
    }
}
