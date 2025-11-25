namespace Playtesters.API.Extensions;

public static class TimeExtensions
{
    /// <summary>
    /// Converts a decimal hours value to HH:mm:ss format,
    /// showing all accumulated hours, even if more than 24.
    /// </summary>
    /// <param name="hours">Number of hours (can include fractions)</param>
    /// <returns>String in HH:mm:ss format</returns>
    public static string ToHhMmSs(this double hours)
    {
        TimeSpan time = TimeSpan.FromHours(hours);

        // TotalHours returns all accumulated hours (including days)
        // Casting to int removes the fractional part.
        int totalHours = (int)time.TotalHours;

        int minutes = time.Minutes;
        int seconds = time.Seconds;
        return $"{totalHours:D2}h:{minutes:D2}m:{seconds:D2}s";
    }
}
