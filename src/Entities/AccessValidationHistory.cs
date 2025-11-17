namespace Playtesters.API.Entities;

public class AccessValidationHistory
{
    public int Id { get; set; }
    public int TesterId { get; set; }
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    public string IpAddress { get; set; }
    public Tester Tester { get; set; }
}
