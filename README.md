# Stičišče – Event Registration Application

**Stičišče** is a web application for discovering events and registering participants. It provides a simple interface for browsing events, filtering them by date, viewing event details, and submitting registrations.

The application is built using ASP.NET Core Web API, Blazor, and PostgreSQL.

## Technologies

- **Backend:** ASP.NET Core Web API (.NET 10)
- **Frontend:** Blazor Web App (Interactive Server)
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core
- **Database migrations:** EF Core Migrations
- **Language:** C#

Blazor was selected for the frontend because it enables interactive web development using C# and integrates naturally with the ASP.NET Core backend. The frontend and backend are implemented as separate projects to maintain a clear separation of responsibilities.

## Features

The application currently supports:

- Browsing available events
- Filtering events by date range
- Viewing event details and remaining capacity
- Registering for events using a full name and email address
- Preventing registrations when an event is fully booked
- Preventing duplicate registrations using the same email address
- Preventing registrations for past events
- Displaying registration confirmations and error messages
- Validating registration data on both frontend and backend

The backend uses database transactions and row-level locking to prevent event capacity from being exceeded by concurrent registrations.

## Project Structure

The solution consists of two separate .NET projects:

```text
HF_naloga-main/
├── HF.Api/
│   ├── Controllers/
│   ├── Data/
│   ├── DTOs/
│   ├── Models/
│   ├── Migrations/
│   └── Program.cs
│
├── HF.Web/
│   ├── Components/
│   │   ├── Layout/
│   │   └── Pages/
│   ├── Models/
│   ├── Services/
│   ├── wwwroot/
│   └── Program.cs
│
├── HF.slnx
├── .gitignore
└── README.md
```

- `HF.Api` handles API requests, business logic, validation, and database operations.
- `HF.Web` provides the Blazor user interface and communicates with the backend through HTTP requests.

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/events` | Retrieve events with optional date filtering |
| GET | `/events/{id}` | Retrieve event details and remaining capacity |
| POST | `/events/{id}/registrations` | Register a participant for an event |

### Event Filtering

Events can be filtered using optional `from` and `to` query parameters.

Example:

```http
GET /events?from=2026-11-01&to=2026-12-31
```

### Registration

Example request:

```http
POST /events/1/registrations
Content-Type: application/json
```

```json
{
  "fullName": "Ana Novak",
  "email": "ana.novak@example.com"
}
```

The API returns appropriate HTTP status codes:

- `200 OK` – Successful data retrieval
- `201 Created` – Successful registration
- `400 Bad Request` – Invalid input
- `404 Not Found` – Event not found
- `409 Conflict` – Duplicate registration, full event, or past event

## Local Development

### Prerequisites

- .NET 10 SDK
- PostgreSQL
- Visual Studio Code (recommended)

### Database Configuration

Create a PostgreSQL database named `hf_naloga`.

Initialize .NET User Secrets for the backend:

```powershell
dotnet user-secrets init --project HF.Api
```

Configure the database connection:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=hf_naloga;Username=postgres;Password=YOUR_PASSWORD" --project HF.Api
```

Replace `YOUR_PASSWORD` with your local PostgreSQL password.

Database credentials must not be committed to the repository.

### Apply Database Migrations

From the repository root, execute:

```powershell
dotnet ef database update --project HF.Api --startup-project HF.Api
```

The database schema is created and updated through EF Core migrations.

### Run the Backend

```powershell
dotnet run --project HF.Api
```

The API runs on the address configured in `HF.Api/Properties/launchSettings.json`.

### Configure the Frontend

Ensure that `HF.Web/appsettings.Development.json` contains the correct backend URL:

```json
{
  "Api": {
    "BaseUrl": "http://localhost:5083/"
  }
}
```

The port must match the backend's actual listening port.

### Run the Frontend

Open a second terminal and execute:

```powershell
dotnet run --project HF.Web
```

Open the frontend URL displayed in the terminal.

Both applications must be running for the frontend to communicate with the API.

## Database

The application uses two main entities.

### Event

- ID
- Name
- Date and time
- Location
- Capacity

### Registration

- ID
- Event reference
- Full name
- Email address
- Registration timestamp

A one-to-many relationship connects events with their registrations.

A unique database index on event ID and email prevents duplicate registrations.

Event dates and registration timestamps are stored in UTC.

### Initial Data

During development, the application automatically seeds an empty database with sample events.

These include upcoming events and a past event for testing registration restrictions.

## Security and Validation

- Registration data is validated before processing.
- Business rules are enforced on the backend.
- Database transactions protect against concurrent overbooking.
- Duplicate registrations are restricted by a unique database index.
- API errors return appropriate HTTP status codes and descriptive messages.
- Sensitive configuration values are stored locally using .NET User Secrets.
- Build artifacts, local environment files, and credentials are excluded from version control.

## Development Status

**The initial working version of Stičišče has been implemented.**

### Completed

- ASP.NET Core Web API project
- Blazor frontend project
- PostgreSQL database integration
- Entity Framework Core models and migrations
- Automatic database seeding
- Event listing and date filtering
- Event details and remaining capacity
- Event registration endpoint
- Backend business rules and validation
- Blazor event listing page
- Event details and registration form
- Frontend-to-backend API communication

### Planned Improvements

- UI/UX improvements and application branding
- Improved error handling and user feedback
- Localized date and time display
- Unit and integration tests
- Dockerfile and containerized deployment
- GitLab CI pipeline for automated builds and tests
- Additional application testing and documentation

## License

No license has been specified for this project.