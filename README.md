# sloEvent – Event Registration Application

**sloEvent** is a web application for browsing events and registering participants. It provides an intuitive interface for discovering events, viewing their details, and submitting registrations.

The application is built with ASP.NET Core Web API, Blazor, and PostgreSQL.

## Technologies

- **Backend:** ASP.NET Core Web API (.NET 10)
- **Frontend:** Blazor Web App (Interactive Server)
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core
- **Database migrations:** EF Core Migrations

Blazor was selected for the frontend because it enables the development of interactive web interfaces using C# and integrates naturally with the .NET backend.

## Features

sloEvent is designed to provide the following functionality:

- Browse available events
- Filter events by date range
- View event details, including remaining capacity
- Register for events using a full name and email address
- Prevent registrations when an event is fully booked
- Prevent duplicate registrations using the same email address
- Prevent registrations for past events
- Display clear confirmation and error messages

## Project Structure

The application consists of two separate .NET projects:

- `HF.Api/` – ASP.NET Core Web API, business logic, database models and migrations
- `HF.Web/` – Blazor frontend and user interface

Both projects are managed within a single .NET solution (`HF.slnx`).

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/events` | Retrieve events with optional date filtering |
| GET | `/events/{id}` | Retrieve event details and remaining capacity |
| POST | `/events/{id}/registrations` | Register a participant for an event |

## Local Development

### Prerequisites

- .NET 10 SDK
- PostgreSQL
- Visual Studio Code (recommended)

### Database Configuration

Create a PostgreSQL database named `hf_naloga`.

Configure the database connection using .NET User Secrets:

```powershell
dotnet user-secrets init --project HF.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=hf_naloga;Username=postgres;Password=YOUR_PASSWORD" --project HF.Api
```

Replace `YOUR_PASSWORD` with your local PostgreSQL password.

Do not commit database credentials or other sensitive configuration values to the repository.

### Apply Database Migrations

From the repository root:

```powershell
dotnet ef database update --project HF.Api --startup-project HF.Api
```

### Run the Backend

```powershell
dotnet run --project HF.Api
```

### Run the Frontend

In a separate terminal:

```powershell
dotnet run --project HF.Web
```

The backend and frontend run as separate applications during development.

## Database

The application uses two main entities:

**Event**
- ID
- Name
- Date and time
- Location
- Capacity

**Registration**
- ID
- Event reference
- Full name
- Email address
- Registration timestamp

The database schema is managed through Entity Framework Core migrations.

During development, initial test events are automatically inserted when the events table is empty.

## Security and Validation

The application is designed to enforce business rules on the backend, including event capacity restrictions, duplicate registration prevention, and validation of event dates.

Sensitive configuration values are managed through .NET User Secrets during local development. Generated build files, temporary files, and local credentials are excluded from version control.

## Development Status

**sloEvent is currently under development.**

Completed:
- Initial ASP.NET Core Web API project
- Initial Blazor Web App project
- PostgreSQL database integration
- Entity Framework Core models and migrations
- Initial event data seeding

Planned:
- Event listing and filtering API
- Event details and registration API
- Business rule validation
- Blazor event listing and registration interface
- API and application testing

## License

No license has been specified for this project.