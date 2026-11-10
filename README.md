# Stičišče

**Stičišče** is a web application for discovering events and registering participants. Users can browse events, filter them by date, view event details and availability, and submit registrations.

The application consists of an ASP.NET Core Web API backend, a Blazor frontend, and a PostgreSQL database.

## Technologies

| Component | Technology |
|---|---|
| Backend | ASP.NET Core Web API (.NET 10) |
| Frontend | Blazor Web App (Interactive Server) |
| Database | PostgreSQL |
| ORM | Entity Framework Core |
| Database migrations | EF Core Migrations |
| Language | C# |
| Styling | Bootstrap and custom CSS |

### Technology Selection

Blazor was selected because it enables interactive web development using C# and integrates naturally with the ASP.NET Core ecosystem.

The frontend and backend are separated into two projects. This provides a clear separation between the user interface, API, business logic, and database access.

Blazor Interactive Server also allows the frontend to communicate with the API through server-side `HttpClient` requests.

PostgreSQL was selected as a free, open-source relational database with support for transactions, constraints, and concurrent operations.

## Features

### Event Browsing

- View a list of events
- Filter events by a date range
- View event details, including date, location, and capacity
- Display the number of remaining available places
- Distinguish between upcoming, past, and fully booked events

### Event Registration

Users can register for events by providing their full name and email address.

The backend enforces the following business rules:

1. Registration is not allowed when an event has reached its capacity.
2. The same email address cannot be registered twice for the same event.
3. Registration is not allowed for past events.
4. Invalid requests return appropriate HTTP status codes and understandable error messages.

Registration processing uses database transactions and PostgreSQL row-level locking to protect event capacity from concurrent registrations.

A unique database index on the combination of event ID and email provides additional protection against duplicate registrations.

## Project Structure

```text
HF_naloga/
├── HF.Api/
│   ├── Controllers/    # API endpoints
│   ├── Data/           # DbContext and database seeding
│   ├── DTOs/           # API request models
│   ├── Models/         # Database entities
│   ├── Migrations/     # EF Core migrations
│   └── Program.cs
│
├── HF.Web/
│   ├── Components/
│   │   ├── Layout/    # Application layout
│   │   └── Pages/     # Event listing and details
│   ├── Models/         # Frontend data models
│   ├── Services/       # API communication
│   ├── wwwroot/        # CSS and static assets
│   └── Program.cs
│
├── HF.slnx
├── .gitignore
└── README.md
```

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/events` | Retrieve events with optional date filtering |
| GET | `/events/{id}` | Retrieve event details and remaining capacity |
| POST | `/events/{id}/registrations` | Register a participant |

### Event Filtering

Optional query parameters:

- `from` – Start date (inclusive)
- `to` – End date (inclusive)

Dates use the `yyyy-MM-dd` format. The current API interprets date boundaries in UTC.

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

### HTTP Responses

| Status | Description |
|---|---|
| 200 OK | Successful data retrieval |
| 201 Created | Registration created successfully |
| 400 Bad Request | Invalid input or date filter |
| 404 Not Found | Event does not exist |
| 409 Conflict | Duplicate registration, full event, or past event |

API errors use structured error responses containing descriptive messages.

## Database

The database contains two main entities.

### Event

- `Id`
- `Name`
- `DateTime`
- `Location`
- `Capacity`

### Registration

- `Id`
- `EventId`
- `FullName`
- `Email`
- `RegisteredAt`

Each event can have multiple registrations.

Database schema changes are managed using Entity Framework Core migrations.

### Initial Data

During development, the application automatically inserts sample events when the events table is empty.

The sample data includes upcoming and past events to support manual testing.

Existing database records are not overwritten when the seeder runs again.

## Local Development

### Prerequisites

- .NET 10 SDK
- PostgreSQL
- EF Core CLI tools (`dotnet-ef`)
- Visual Studio Code or another .NET-compatible IDE

### 1. Configure PostgreSQL

Create a PostgreSQL database named `hf_naloga`.

Initialize User Secrets for the API project if they have not already been initialized:

```powershell
dotnet user-secrets init --project HF.Api
```

Configure the connection string:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=hf_naloga;Username=postgres;Password=YOUR_PASSWORD" --project HF.Api
```

Replace `YOUR_PASSWORD` with your local database password.

Never commit database credentials to version control.

### 2. Restore Dependencies

From the repository root:

```powershell
dotnet restore
```

### 3. Apply Database Migrations

```powershell
dotnet ef database update --project HF.Api --startup-project HF.Api
```

This creates the required database tables.

### 4. Configure the Frontend

Set the backend URL in `HF.Web/appsettings.Development.json`:

```json
{
  "Api": {
    "BaseUrl": "http://localhost:5083/"
  }
}
```

Keep any other existing configuration sections. Adjust the port if necessary.

### 5. Run the Backend

```powershell
dotnet run --project HF.Api
```

### 6. Run the Frontend

Open another terminal:

```powershell
dotnet run --project HF.Web
```

Open the URL displayed by the frontend application.

Both projects must be running during local development.

## User Interface

Stičišče uses a responsive Blazor interface with a beige, burgundy, and mint-green color palette.

The main page displays available events and provides a date-range search.

The event details page provides information about the selected event and a registration form with validation and feedback messages.

## Security and Data Validation

- Input validation is performed before registrations are processed.
- Business rules are enforced by the backend.
- Email addresses are normalized before being stored through the API.
- Duplicate registrations are restricted by a database unique index.
- Transactions and row-level locking prevent concurrent registrations from exceeding capacity.
- Database credentials are stored locally using .NET User Secrets.
- Generated build files and sensitive local configuration files are excluded through `.gitignore`.

.NET User Secrets are intended for local development and are not encrypted secret storage.

## Development Status

### Implemented

- ASP.NET Core Web API backend
- Blazor Interactive Server frontend
- PostgreSQL integration
- EF Core migrations
- Automatic sample event seeding
- Event listing and date filtering
- Event details and availability
- Event registration
- Backend business rules
- Input validation and API error responses
- Frontend API integration
- Responsive user interface and application branding

### Planned

- Automated unit and integration tests
- Tests for invalid, duplicate, full-event, and past-event registrations
- Concurrent registration testing
- Dockerfile and containerized deployment
- GitLab CI pipeline for automated builds and tests
- Improved local date and time presentation

## License

No license has been specified for this project.