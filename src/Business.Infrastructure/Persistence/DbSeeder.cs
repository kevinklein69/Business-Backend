using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using Business.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Business.Infrastructure.Persistence;

/// Idempotent demo-data seeder. Runs only against an empty database (Development),
/// so a fresh checkout has working logins and recognizable Kanban/time-tracking/vacation data
/// without needing `dotnet ef migrations`/manual SQL.
public static class DbSeeder
{
    public const string DemoPassword = "Demo123!";

    private static DateTime Utc(int year, int month, int day) =>
        new(year, month, day, 0, 0, 0, DateTimeKind.Utc);

    public static async Task SeedAsync(BusinessDbContext context, IPasswordHasher passwordHasher, ILogger logger)
    {
        // The seeder runs outside an HTTP request, so there is no current tenant and the
        // company query filter would hide existing rows. Bypass it for the empty-check and
        // stamp every seeded row with the demo company explicitly.
        if (await context.Users.IgnoreQueryFilters().AnyAsync())
        {
            return;
        }

        logger.LogInformation("Seeding demo data (all demo accounts use password '{Password}')...", DemoPassword);

        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Demo GmbH",
            CreatedAt = DateTime.UtcNow,
        };
        context.Companies.Add(company);

        var users = CreateUsers(passwordHasher);
        foreach (var u in users) u.CompanyId = company.Id;
        context.Users.AddRange(users);

        var byName = users.ToDictionary(u => $"{u.FirstName} {u.LastName}");

        var orders = CreateOrders(byName);
        foreach (var o in orders) o.CompanyId = company.Id;
        context.Orders.AddRange(orders);

        var timeEntries = CreateTimeEntries(byName["Max Müller"]);
        foreach (var t in timeEntries) t.CompanyId = company.Id;
        context.TimeEntries.AddRange(timeEntries);

        var absences = CreateAbsenceRequests(byName["Max Müller"])
            .Concat(CreateSickAbsenceRequests(byName["Tom Wagner"], byName["Lisa Bauer"]))
            .ToList();
        foreach (var a in absences) a.CompanyId = company.Id;
        context.AbsenceRequests.AddRange(absences);

        context.CompanySettings.Add(new CompanySettings
        {
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            State = GermanState.Bayern,
        });

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
                EntryDate = new DateOnly(2026, 5, 1),
            };
            user.PasswordHash = passwordHasher.Hash(user, DemoPassword);
            return user;
        }).ToList();
    }

    private static List<Order> CreateOrders(IReadOnlyDictionary<string, User> byName)
    {
        (string Title, string? Description, string? Customer, string Street, string HouseNumber, string Zip, string City, OrderStatus Status, DateTime CreatedAt, string[] Assignees)[] specs =
        [
            ("Inspect heating system", null, "The Berger Family",
                "Hauptstraße", "12", "70173", "Stuttgart",
                OrderStatus.ToDo, Utc(2026, 6, 1), []),
            ("Roof inspection", "Annual inspection", "Property Management Ltd.",
                "Bahnhofstraße", "5", "70435", "Stuttgart",
                OrderStatus.InProgress, Utc(2026, 6, 2), ["Max Müller", "Anna Schmidt"]),
            ("Electrical installation - ground floor", null, null,
                "Gartenweg", "3", "70499", "Stuttgart",
                OrderStatus.InProgress, Utc(2026, 5, 28), ["Tom Wagner"]),
            ("Plumbing - upper floor", null, "Mr. Meier",
                "Schillerstraße", "21", "70178", "Stuttgart",
                OrderStatus.ReadyForAcceptance, Utc(2026, 5, 20), ["Max Müller"]),
            ("Window replacement - 2nd floor", "All 4 windows", "Ms. Koch",
                "Goethestraße", "8", "70184", "Stuttgart",
                OrderStatus.Invoicing, Utc(2026, 5, 15), ["Tom Wagner", "Jonas Fischer"]),
            ("Painting work - ground floor", null, null,
                "Lindenallee", "17", "70195", "Stuttgart",
                OrderStatus.Done, Utc(2026, 5, 10), ["Lisa Bauer"]),
        ];

        return specs.Select(s => new Order
        {
            Id = Guid.NewGuid(),
            Title = s.Title,
            Description = s.Description,
            Customer = s.Customer,
            Street = s.Street,
            HouseNumber = s.HouseNumber,
            Zip = s.Zip,
            City = s.City,
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
            (Utc(2026, 5, 26), new TimeSpan(8, 0, 0), new TimeSpan(17, 0, 0)),
            (Utc(2026, 5, 29), new TimeSpan(7, 30, 0), new TimeSpan(15, 0, 0)),
            (Utc(2026, 5, 28), new TimeSpan(8, 0, 0), new TimeSpan(16, 0, 0)),
            (Utc(2026, 5, 27), new TimeSpan(7, 50, 0), new TimeSpan(16, 15, 0)),
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
