namespace Business.Application.Common;

public static class ProbationCalculator
{
    /// Probezeitende = Eintrittsdatum + Probezeitdauer (Monate). Ohne Probezeitdauer kein Probezeitende.
    public static DateOnly? CalculateEnd(DateOnly entryDate, int? probationMonths) =>
        probationMonths is null ? null : entryDate.AddMonths(probationMonths.Value);
}
