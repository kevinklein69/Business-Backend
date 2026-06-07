using Betrieb.Application.Common.Interfaces;
using Betrieb.Domain.Entities;
using Betrieb.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Betrieb.Infrastructure.Persistence;

/// Idempotent demo-data seeder. Runs only against an empty database (Development),
/// so a fresh checkout has working logins and recognizable Kanban/time-tracking/vacation data
/// without needing `dotnet ef migrations`/manual SQL.
public static class DbSeeder
{
    public const string DemoPassword = "Demo123!";

    private static DateTime Utc(int year, int month, int day) =>
        new(year, month, day, 0, 0, 0, DateTimeKind.Utc);

    public static async Task SeedAsync(BetriebDbContext context, IPasswordHasher passwordHasher, ILogger logger)
    {
        if (await context.Users.AnyAsync())
        {
            return;
        }

        logger.LogInformation("Seeding demo data (all demo accounts use password '{Password}')...", DemoPassword);

        var users = CreateUsers(passwordHasher);
        context.Users.AddRange(users);

        var byName = users.ToDictionary(u => $"{u.FirstName} {u.LastName}");

        context.Orders.AddRange(CreateOrders(byName));
        context.TimeEntries.AddRange(CreateTimeEntries(byName["Max Müller"]));
        context.AbsenceRequests.AddRange(CreateAbsenceRequests(byName["Max Müller"]));
        context.AbsenceRequests.AddRange(CreateSickAbsenceRequests(byName["Tom Wagner"], byName["Lisa Bauer"]));

        await context.SaveChangesAsync();
    }

    private static List<User> CreateUsers(IPasswordHasher passwordHasher)
    {
        var specs = new (string FirstName, string LastName, string Email, Role Role, string Department)[]
        {
            ("Max", "Müller", "max.mueller@firma.de", Role.Admin, "Management"),
            ("Anna", "Schmidt", "a.schmidt@firma.de", Role.Manager, "Technical"),
            ("Tom", "Wagner", "t.wagner@firma.de", Role.Employee, "Technical"),
            ("Lisa", "Bauer", "l.bauer@firma.de", Role.Employee, "Administration"),
            ("Jonas", "Fischer", "j.fischer@firma.de", Role.Employee, "Technical"),
            ("Maria", "Hoffmann", "m.hoffmann@firma.de", Role.Manager, "Sales"),
            ("Felix", "Koch", "f.koch@firma.de", Role.Employee, "Sales"),
            ("Sara", "Becker", "s.becker@firma.de", Role.Employee, "Administration"),
        };

        return specs.Select(s =>
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                Role = s.Role,
                Department = s.Department,
            };
            user.PasswordHash = passwordHasher.Hash(user, DemoPassword);
            return user;
        }).ToList();
    }

    private static List<Order> CreateOrders(IReadOnlyDictionary<string, User> byName)
    {
        (string Title, string? Description, string? Customer, OrderStatus Status, DateTime CreatedAt, string[] Assignees)[] specs =
        [
            ("Inspect heating system", null, "The Berger Family",
                OrderStatus.Backlog, Utc(2026, 6, 1), []),
            ("Roof inspection", "Annual inspection", "Property Management Ltd.",
                OrderStatus.InProgress, Utc(2026, 6, 2), ["Max Müller", "Anna Schmidt"]),
            ("Electrical installation - ground floor", null, null,
                OrderStatus.InProgress, Utc(2026, 5, 28), ["Tom Wagner"]),
            ("Plumbing - upper floor", null, "Mr. Meier",
                OrderStatus.ReadyForAcceptance, Utc(2026, 5, 20), ["Max Müller"]),
            ("Window replacement - 2nd floor", "All 4 windows", "Ms. Koch",
                OrderStatus.Invoicing, Utc(2026, 5, 15), ["Tom Wagner", "Jonas Fischer"]),
            ("Painting work - ground floor", null, null,
                OrderStatus.Done, Utc(2026, 5, 10), ["Lisa Bauer"]),
        ];

        return specs.Select(s => new Order
        {
            Id = Guid.NewGuid(),
            Title = s.Title,
            Description = s.Description,
            Customer = s.Customer,
            Status = s.Status,
            CreatedAt = s.CreatedAt,
            Assignees = s.Assignees.Select(name => byName[name]).ToList(),
        }).ToList();
    }

    private static List<TimeEntry> CreateTimeEntries(User user)
    {
        (DateTime Date, TimeSpan Start, TimeSpan End)[] specs =
        [
            (Utc(2026, 6, 2), new TimeSpan(7, 45, 0), new TimeSpan(16, 30, 0)),
            (Utc(2026, 6, 1), new TimeSpan(8, 0, 0), new TimeSpan(17, 0, 0)),
            (Utc(2026, 5, 31), new TimeSpan(7, 30, 0), new TimeSpan(15, 0, 0)),
            (Utc(2026, 5, 30), new TimeSpan(8, 0, 0), new TimeSpan(16, 0, 0)),
            (Utc(2026, 5, 29), new TimeSpan(7, 50, 0), new TimeSpan(16, 15, 0)),
        ];

        return specs.Select(s => new TimeEntry
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            ClockIn = s.Date.Add(s.Start),
            ClockOut = s.Date.Add(s.End),
        }).ToList();
    }

    private static List<AbsenceRequest> CreateAbsenceRequests(User user)
    {
        (AbsenceType Type, DateOnly Start, DateOnly End, AbsenceStatus Status, string? Comment)[] specs =
        [
            (AbsenceType.Vacation, new DateOnly(2026, 7, 15), new DateOnly(2026, 7, 19), AbsenceStatus.Approved, "Summer vacation"),
            (AbsenceType.Vacation, new DateOnly(2026, 9, 1), new DateOnly(2026, 9, 5), AbsenceStatus.Open, null),
            (AbsenceType.Vacation, new DateOnly(2026, 12, 27), new DateOnly(2026, 12, 31), AbsenceStatus.Rejected, "Company holidays"),
        ];

        return specs.Select(s => new AbsenceRequest
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Type = s.Type,
            StartDate = s.Start,
            EndDate = s.End,
            Status = s.Status,
            Comment = s.Comment,
        }).ToList();
    }

    /// Sample sick-leave / child-sick records, as a manager would record them on behalf of employees.
    private static List<AbsenceRequest> CreateSickAbsenceRequests(User tom, User lisa)
    {
        (User User, AbsenceType Type, DateOnly Start, DateOnly End, string? Comment)[] specs =
        [
            (tom, AbsenceType.Sick, new DateOnly(2026, 5, 25), new DateOnly(2026, 5, 27), "Grippaler Infekt"),
            (lisa, AbsenceType.ChildSick, new DateOnly(2026, 6, 3), new DateOnly(2026, 6, 3), "Kind krank – Kinderkrankentage"),
        ];

        return specs.Select(s => new AbsenceRequest
        {
            Id = Guid.NewGuid(),
            UserId = s.User.Id,
            Type = s.Type,
            StartDate = s.Start,
            EndDate = s.End,
            Status = AbsenceStatus.Approved,
            Comment = s.Comment,
        }).ToList();
    }
}
