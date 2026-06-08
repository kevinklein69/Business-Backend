using Business.Domain.Enums;

namespace Business.Domain.Common;

/// Computes German public holidays per Bundesland for a given year.
/// Movable feasts are derived from the Easter Sunday date (Gaussian Easter algorithm),
/// so this works for any year without a maintained date table.
public static class GermanHolidays
{
    /// Heiligabend and Silvester are not statutory holidays anywhere in Germany,
    /// but are commonly treated as half working days by employers.
    public static readonly IReadOnlySet<MonthDay> HalfDays = new HashSet<MonthDay>
    {
        new(12, 24),
        new(12, 31),
    };

    public readonly record struct MonthDay(int Month, int Day);

    public static bool IsFullHoliday(DateOnly date, GermanState state) =>
        GetHolidays(date.Year, state).Contains(date);

    public static bool IsHalfDay(DateOnly date) =>
        HalfDays.Contains(new MonthDay(date.Month, date.Day));

    public static IReadOnlySet<DateOnly> GetHolidays(int year, GermanState state)
    {
        var easterSunday = GetEasterSunday(year);

        var holidays = new HashSet<DateOnly>
        {
            new(year, 1, 1),                  // Neujahr
            easterSunday.AddDays(-2),         // Karfreitag
            easterSunday.AddDays(1),          // Ostermontag
            new(year, 5, 1),                  // Tag der Arbeit
            easterSunday.AddDays(39),         // Christi Himmelfahrt
            easterSunday.AddDays(50),         // Pfingstmontag
            new(year, 10, 3),                 // Tag der Deutschen Einheit
            new(year, 12, 25),                // 1. Weihnachtsfeiertag
            new(year, 12, 26),                // 2. Weihnachtsfeiertag
        };

        var fronleichnam = easterSunday.AddDays(60);

        switch (state)
        {
            case GermanState.BadenWuerttemberg:
                holidays.Add(new DateOnly(year, 1, 6));   // Heilige Drei Könige
                holidays.Add(fronleichnam);
                holidays.Add(new DateOnly(year, 11, 1));  // Allerheiligen
                break;

            case GermanState.Bayern:
                holidays.Add(new DateOnly(year, 1, 6));
                holidays.Add(fronleichnam);
                holidays.Add(new DateOnly(year, 8, 15));  // Mariä Himmelfahrt (in der Mehrheit der Gemeinden)
                holidays.Add(new DateOnly(year, 11, 1));
                break;

            case GermanState.Berlin:
                holidays.Add(new DateOnly(year, 3, 8));   // Internationaler Frauentag
                break;

            case GermanState.Brandenburg:
                holidays.Add(new DateOnly(year, 10, 31)); // Reformationstag
                break;

            case GermanState.Bremen:
                holidays.Add(new DateOnly(year, 10, 31));
                break;

            case GermanState.Hamburg:
                holidays.Add(new DateOnly(year, 10, 31));
                break;

            case GermanState.Hessen:
                holidays.Add(fronleichnam);
                break;

            case GermanState.MecklenburgVorpommern:
                holidays.Add(new DateOnly(year, 3, 8));
                holidays.Add(new DateOnly(year, 10, 31));
                break;

            case GermanState.Niedersachsen:
                holidays.Add(new DateOnly(year, 10, 31));
                break;

            case GermanState.NordrheinWestfalen:
                holidays.Add(fronleichnam);
                holidays.Add(new DateOnly(year, 11, 1));
                break;

            case GermanState.RheinlandPfalz:
                holidays.Add(fronleichnam);
                holidays.Add(new DateOnly(year, 11, 1));
                break;

            case GermanState.Saarland:
                holidays.Add(fronleichnam);
                holidays.Add(new DateOnly(year, 8, 15));  // Mariä Himmelfahrt
                holidays.Add(new DateOnly(year, 11, 1));
                break;

            case GermanState.Sachsen:
                holidays.Add(new DateOnly(year, 10, 31));
                holidays.Add(GetBussUndBettag(year));
                break;

            case GermanState.SachsenAnhalt:
                holidays.Add(new DateOnly(year, 1, 6));
                holidays.Add(new DateOnly(year, 10, 31));
                break;

            case GermanState.SchleswigHolstein:
                holidays.Add(new DateOnly(year, 10, 31));
                break;

            case GermanState.Thueringen:
                holidays.Add(new DateOnly(year, 9, 20));  // Weltkindertag
                holidays.Add(new DateOnly(year, 10, 31));
                break;
        }

        return holidays;
    }

    /// Buß- und Bettag: the Wednesday immediately preceding November 23rd
    /// (so it always falls between November 16th and 22nd).
    private static DateOnly GetBussUndBettag(int year)
    {
        var nov23 = new DateOnly(year, 11, 23);
        var daysBack = ((int)nov23.DayOfWeek - (int)DayOfWeek.Wednesday + 7) % 7;
        if (daysBack == 0) daysBack = 7;
        return nov23.AddDays(-daysBack);
    }

    /// Gaussian Easter algorithm — returns the date of Easter Sunday for the given year.
    private static DateOnly GetEasterSunday(int year)
    {
        var a = year % 19;
        var b = year / 100;
        var c = year % 100;
        var d = b / 4;
        var e = b % 4;
        var f = (b + 8) / 25;
        var g = (b - f + 1) / 3;
        var h = (19 * a + b - d - g + 15) % 30;
        var i = c / 4;
        var k = c % 4;
        var l = (32 + 2 * e + 2 * i - h - k) % 7;
        var m = (a + 11 * h + 22 * l) / 451;
        var month = (h + l - 7 * m + 114) / 31;
        var day = (h + l - 7 * m + 114) % 31 + 1;

        return new DateOnly(year, month, day);
    }
}
