
using HF.Api.Data;
using HF.Api.DTOs;
using HF.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using HF.Api.Services;

namespace HF.Api.Controllers;

[ApiController]
[Route("events")]
public class EventsController : ControllerBase
{
    private readonly AppDbContext _context;

    public EventsController(AppDbContext context)
    {
        _context = context;
    }

    // GET /events?from=2026-11-01&to=2026-12-31
    [HttpGet]
    public async Task<IActionResult> GetEvents(
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to)
    {
        if (from.HasValue && to.HasValue && from > to)
        {
            return Problem(
                statusCode: 400,
                title: "Neveljaven datumski filter.",
                detail: "Datum 'from' ne sme biti za datumom 'to'.");
        }

        var query = _context.Events.AsNoTracking();

        // Datumski filter interpretiramo v UTC.
        if (from.HasValue)
        {
            var start = from.Value.ToDateTime(
                TimeOnly.MinValue, DateTimeKind.Utc);

            query = query.Where(e => e.DateTime >= start);
        }

        if (to.HasValue)
        {
            var end = to.Value.AddDays(1).ToDateTime(
                TimeOnly.MinValue, DateTimeKind.Utc);

            query = query.Where(e => e.DateTime < end);
        }

        var events = await query
            .OrderBy(e => e.DateTime)
            .Select(e => new
            {
                e.Id,
                e.Name,
                e.DateTime,
                e.Location,
                e.Capacity,
                RegisteredCount = e.Registrations.Count(),
                AvailablePlaces =
                    e.Capacity - e.Registrations.Count()
            })
            .ToListAsync();

        return Ok(events);
    }

    // GET /events/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetEvent(int id)
    {
        var ev = await _context.Events
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new
            {
                e.Id,
                e.Name,
                e.DateTime,
                e.Location,
                e.Capacity,
                RegisteredCount = e.Registrations.Count(),
                AvailablePlaces =
                    e.Capacity - e.Registrations.Count()
            })
            .FirstOrDefaultAsync();

        if (ev == null)
        {
            return Problem(
                statusCode: 404,
                title: "Dogodek ne obstaja.",
                detail: $"Dogodek z ID {id} ni bil najden.");
        }

        return Ok(ev);
    }

    // POST /events/1/registrations
    [HttpPost("{id:int}/registrations")]
    public async Task<IActionResult> Register(
        int id,
        [FromBody] CreateRegistrationRequest request)
    {
        var fullName = request.FullName?.Trim() ?? "";
        var email = RegistrationRules.NormalizeEmail(request.Email ?? "");

        if (string.IsNullOrWhiteSpace(fullName) ||
            string.IsNullOrWhiteSpace(email))
        {
            return Problem(
                statusCode: 400,
                title: "Neveljavni podatki.",
                detail: "Ime, priimek in e-pošta so obvezni.");
        }

        // Transakcija zagotovi, da kapaciteto
        // preverimo in posodobimo atomarno.
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        // Zaklenemo vrstico dogodka do konca transakcije.
        // S tem preprečimo hkratno prekoračitev kapacitete.
        Event? ev = null;

        var lockedEvents = _context.Events
            .FromSqlInterpolated(
                $"SELECT * FROM \"Events\" WHERE \"Id\" = {id} FOR UPDATE")
            .AsAsyncEnumerable();

        await foreach (var item in lockedEvents)
        {
            ev = item;
            break;
        }

        if (ev == null)
        {
            return Problem(
                statusCode: 404,
                title: "Dogodek ne obstaja.");
        }

        
        var alreadyRegistered = await _context.Registrations
            .AnyAsync(r => r.EventId == id && r.Email == email);

        var registeredCount = await _context.Registrations
            .CountAsync(r => r.EventId == id);

        var validation = RegistrationRules.Validate(
            ev,
            registeredCount,
            alreadyRegistered,
            DateTime.UtcNow);

        switch (validation)
        {
            case RegistrationError.PastEvent:
                return Problem(
                    statusCode: 409,
                    title: "Prijava ni mogoča.",
                    detail: "Na pretekle dogodke se ni mogoče prijaviti.");

            case RegistrationError.DuplicateEmail:
                return Problem(
                    statusCode: 409,
                    title: "Podvojena prijava.",
                    detail: "Ta e-poštni naslov je že prijavljen na dogodek.");

            case RegistrationError.EventFull:
                return Problem(
                    statusCode: 409,
                    title: "Dogodek je poln.",
                    detail: "Na dogodku ni več prostih mest.");
        }


        var registration = new Registration
        {
            EventId = id,
            FullName = fullName,
            Email = email,
            RegisteredAt = DateTime.UtcNow
        };

        _context.Registrations.Add(registration);

        try
        {
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException pg &&
                  pg.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            return Problem(
                statusCode: 409,
                title: "Podvojena prijava.",
                detail: "Ta e-poštni naslov je že prijavljen na dogodek.");
        }

        return StatusCode(201, new
        {
            registration.Id,
            registration.EventId,
            registration.FullName,
            registration.Email,
            registration.RegisteredAt,
            Message = "Prijava je bila uspešna."
        });
    }
}
