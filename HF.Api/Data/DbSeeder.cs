
using HF.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HF.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Če dogodki že obstajajo, ne dodajaj novih
        if (await context.Events.AnyAsync())
        {
            return;
        }

        var today = DateTime.UtcNow.Date;

        var events = new List<Event>
        {
            new Event
            {
                Name = "Delavnica ASP.NET Core",
                DateTime = today.AddDays(7).AddHours(10),
                Location = "Ljubljana",
                Capacity = 20
            },
            new Event
            {
                Name = "Konferenca o umetni inteligenci",
                DateTime = today.AddDays(14).AddHours(9),
                Location = "Maribor",
                Capacity = 100
            },
            new Event
            {
                Name = "Uvod v PostgreSQL",
                DateTime = today.AddDays(21).AddHours(14),
                Location = "Koper",
                Capacity = 15
            },
            new Event
            {
                Name = "Salsa večer",
                DateTime = today.AddDays(-7).AddHours(10),
                Location = "Ljubljana",
                Capacity = 30
            }
        };

        context.Events.AddRange(events);
        await context.SaveChangesAsync();
    }
}
