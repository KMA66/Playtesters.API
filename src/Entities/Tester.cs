namespace Playtesters.API.Entities;

public class Tester
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AccessKey { get; set; } = Guid.NewGuid().ToString();
    public double TotalHoursPlayed { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
