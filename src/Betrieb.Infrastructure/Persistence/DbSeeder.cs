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
        context.VacationRequests.AddRange(CreateVacationRequests(byName["Max Müller"]));

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
                OrderStatus.Backlog, new DateTime(2026, 6, 1), []),
            ("Roof inspection", "Annual inspection", "Property Management Ltd.",
                OrderStatus.InProgress, new DateTime(2026, 6, 2), ["Max Müller", "Anna Schmidt"]),
            ("Electrical installation - ground floor", null, null,
                OrderStatus.InProgress, new DateTime(2026, 5, 28), ["Tom Wagner"]),
            ("Plumbing - upper floor", null, "Mr. Meier",
                OrderStatus.ReadyForAcceptance, new DateTime(2026, 5, 20), ["Max Müller"]),
            ("Window replacement - 2nd floor", "All 4 windows", "Ms. Koch",
                OrderStatus.Invoicing, new DateTime(2026, 5, 15), ["Tom Wagner", "Jonas Fischer"]),
            ("Painting work - ground floor", null, null,
                OrderStatus.Done, new DateTime(2026, 5, 10), ["Lisa Bauer"]),
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
            (new DateTime(2026, 6, 2), new TimeSpan(7, 45, 0), new TimeSpan(16, 30, 0)),
            (new DateTime(2026, 6, 1), new TimeSpan(8, 0, 0), new TimeSpan(17, 0, 0)),
            (new DateTime(2026, 5, 31), new TimeSpan(7, 30, 0), new TimeSpan(15, 0, 0)),
            (new DateTime(2026, 5, 30), new TimeSpan(8, 0, 0), new TimeSpan(16, 0, 0)),
            (new DateTime(2026, 5, 29), new TimeSpan(7, 50, 0), new TimeSpan(16, 15, 0)),
        ];

        return specs.Select(s => new TimeEntry
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            ClockIn = s.Date.Add(s.Start),
            ClockOut = s.Date.Add(s.End),
        }).ToList();
    }

    private static List<VacationRequest> CreateVacationRequests(User user)
    {
        (DateOnly Start, DateOnly End, VacationStatus Status, string? Comment)[] specs =
        [
            (new DateOnly(2026, 7, 15), new DateOnly(2026, 7, 19), VacationStatus.Approved, "Summer vacation"),
            (new DateOnly(2026, 9, 1), new DateOnly(2026, 9, 5), VacationStatus.Open, null),
            (new DateOnly(2026, 12, 27), new DateOnly(2026, 12, 31), VacationStatus.Rejected, "Company holidays"),
        ];

        return specs.Select(s => new VacationRequest
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            StartDate = s.Start,
            EndDate = s.End,
            Status = s.Status,
            Comment = s.Comment,
        }).ToList();
    }
}
