using Business.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Business.UnitTests.TimeTracking;

public static class TestDbContextFactory
{
    public static BusinessDbContext Create()
    {
        var options = new DbContextOptionsBuilder<BusinessDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new BusinessDbContext(options);
    }
}
