using Business.Application.Common.Interfaces;

namespace Business.UnitTests.TimeTracking;

public class FakeCurrentUserService : ICurrentUserService
{
    public Guid? UserId { get; set; }
    public Guid? CompanyId { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
}
