using Business.Application.Common.Interfaces;
using Business.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Business.UnitTests.TimeTracking;

public static class TestDbContextFactory
{
    public static BusinessDbContext Create(ICurrentUserService? currentUser = null)
    {
        var options = new DbContextOptionsBuilder<BusinessDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        // Default tenant is Guid.Empty so existing tests that seed entities without an explicit
        // CompanyId still satisfy the company query filter.
        currentUser ??= new FakeCurrentUserService { CompanyId = Guid.Empty };

        return new BusinessDbContext(options, currentUser);
    }
}
