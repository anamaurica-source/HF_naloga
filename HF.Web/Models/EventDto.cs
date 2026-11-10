
namespace HF.Web.Models;

public class EventDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime DateTime { get; set; }
    public string Location { get; set; } = "";
    public int Capacity { get; set; }
    public int RegisteredCount { get; set; }
    public int AvailablePlaces { get; set; }
}
