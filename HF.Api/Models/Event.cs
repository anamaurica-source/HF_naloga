namespace HF.Api.Models;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime DateTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public int Capacity { get; set; }

    public List<Registration> Registrations { get; set; } = new();
}