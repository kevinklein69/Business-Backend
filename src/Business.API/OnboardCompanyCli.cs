using Business.Application.Common.Exceptions;
using Business.Application.Features.Auth.RegisterCompany;
using FluentValidation;
using MediatR;

namespace Business.API;

/// Internal onboarding tool. Creates a new tenant (company + first Admin) from the command line,
/// so customers are provisioned manually after payment — there is no public self-service endpoint.
///
/// Usage:
/// cd /Users/kevinklein/Documents/DEV/Business-Backend
/// dotnet run --project src/Business.API --no-build -- onboard-company \
/// --company 'Schmitt Heizung GmbH' \
/// --first 'Peter' --last 'Schmitt' \
/// --email 'peter@schmitt-heizung.de' \
/// --password 'Start2026!Sicher'
public static class OnboardCompanyCli
{
    public const string CommandName = "onboard-company";

    public static async Task<int> RunAsync(IServiceProvider services, string[] args)
    {
        var opts = ParseArgs(args);

        string? Missing(string key) => opts.TryGetValue(key, out var v) && !string.IsNullOrWhiteSpace(v) ? null : key;
        var missing = new[] { "company", "first", "last", "email", "password" }
            .Select(Missing)
            .Where(m => m is not null)
            .ToArray();

        if (missing.Length > 0)
        {
            Console.Error.WriteLine($"Fehlende Argumente: {string.Join(", ", missing.Select(m => "--" + m))}");
            Console.Error.WriteLine(
                "Aufruf: dotnet run --project src/Business.API -- onboard-company " +
                "--company \"Firma GmbH\" --first Vorname --last Nachname --email admin@firma.de --password \"Passwort\"");
            return 1;
        }

        using var scope = services.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        try
        {
            await sender.Send(new RegisterCompanyCommand(
                opts["company"],
                opts["first"],
                opts["last"],
                opts["email"],
                opts["password"]));
        }
        catch (ConflictException ex)
        {
            Console.Error.WriteLine($"Fehler: {ex.Message}");
            return 1;
        }
        catch (ValidationException ex)
        {
            Console.Error.WriteLine("Validierung fehlgeschlagen:");
            foreach (var failure in ex.Errors)
            {
                Console.Error.WriteLine($"  - {failure.PropertyName}: {failure.ErrorMessage}");
            }
            return 1;
        }

        Console.WriteLine($"Firma '{opts["company"]}' angelegt. Admin-Login: {opts["email"]}");
        return 0;
    }

    private static Dictionary<string, string> ParseArgs(string[] args)
    {
        // args[0] is the command name; flags follow as "--key value".
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        for (var i = 1; i < args.Length - 1; i++)
        {
            if (args[i].StartsWith("--", StringComparison.Ordinal))
            {
                result[args[i][2..]] = args[i + 1];
                i++;
            }
        }
        return result;
    }
}
