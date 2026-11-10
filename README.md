# Stičišče

**Stičišče** is a web application for browsing events and registering participants. Users can filter events by date, view event details and available places, and register using their name and email address.

The application prevents registrations for full or past events and duplicate registrations using the same email address.

## Technologies and Design Decisions

- **Backend:** ASP.NET Core Web API (.NET 10)
- **Frontend:** Blazor Web App (Interactive Server)
- **Database:** PostgreSQL with Entity Framework Core
- **Testing:** xUnit
- **Containerization:** Docker and Docker Compose
- **CI:** GitLab CI

Blazor was chosen because it allows both frontend and backend development in C#, simplifying development and maintenance. The projects are separated to keep the API independent of the user interface.

PostgreSQL was selected as a free, open-source relational database supporting transactions and data integrity constraints. Entity Framework Core simplifies database access and schema management through migrations.

Business rules are enforced on the backend. Database transactions and row-level locking prevent concurrent registrations from exceeding event capacity.

## Running the Application

### Local Development

Requirements: .NET 10 SDK, PostgreSQL and EF Core CLI tools.

Create a PostgreSQL database named `hf_naloga` and configure the connection using .NET User Secrets:

```powershell
dotnet user-secrets init --project HF.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=hf_naloga;Username=postgres;Password=YOUR_PASSWORD" --project HF.Api
```

Apply database migrations:

```powershell
dotnet ef database update --project HF.Api --startup-project HF.Api
```

Start the backend:

```powershell
dotnet run --project HF.Api
```

In a separate terminal, start the frontend:

```powershell
dotnet run --project HF.Web
```

The frontend API URL is configured in `HF.Web/appsettings.Development.json`. By default, the API runs at `http://localhost:5083/`. Adjust the configuration if necessary.

Sample events are automatically inserted into an empty database during development.

### Docker

Docker Compose starts the API and PostgreSQL database. The frontend is not included in the current Docker configuration.

Create a local `.env` file from `.env.example` and replace the placeholder with a database password.

```powershell
Copy-Item .env.example .env
docker compose up --build -d
```

The API is available at:

`http://localhost:8085/events`

To stop the containers:

```powershell
docker compose down
```

Database data is preserved in a Docker volume.

## Tests and CI

Business rules are covered by xUnit unit tests, including full events, duplicate registrations and past events.

Run tests locally:

```powershell
dotnet test HF.Tests/HF.Tests.csproj
```

The `.gitlab-ci.yml` pipeline builds the .NET solution and executes automated tests. Failed builds or tests cause the pipeline to fail.

### Proposed CI/CD Extension

The pipeline could be extended with the following stages:

1. **Build and test:** Compile the solution and run automated tests.
2. **Publish:** Build a versioned Docker image and push it to the GitLab Container Registry.
3. **Test deployment:** Automatically deploy changes from `main` to a separate test environment, apply migrations and run integration tests.
4. **Production deployment:** After successful testing, allow manual deployment of the tested image using protected credentials.
5. **Verification:** Perform health checks and monitoring, with application rollback support if deployment fails.

Test and production environments would use separate databases and secrets. Production deployment would additionally require HTTPS, database backups and controlled migrations.

## Possible Improvements

Given more development time, the following improvements would be prioritized.

### User Experience

- **Improved UI/UX:** Further refine the visual design, responsiveness and accessibility.
- **Detailed locations:** Extend the event model with a more precise address or venue information.
- **Location filtering:** Allow users to search for events by city or location.
- **Past events:** Move past events to the bottom of the list and display them with reduced opacity.
- **User accounts:** Introduce registration, login and user profiles where participants could view and manage their event registrations.
- **Notifications:** Allow users to subscribe to event categories or specific events and receive email notifications about new events, changes or upcoming dates.

### Technical Improvements

- **Security:** Strengthen API security with rate limiting, input validation and protection against SQL injection. Continue using parameterized queries through Entity Framework Core and avoid dynamically constructed SQL from user input.
- **Authentication:** If user accounts are introduced, implement secure authentication and store passwords using established salted password-hashing algorithms, such as those provided by ASP.NET Core Identity, rather than plaintext or reversible encryption.
- **Authorization:** Introduce administrator roles for creating, editing and deleting events.
- **Testing:** Add API integration tests against PostgreSQL, including concurrent registration scenarios.
- **Monitoring:** Introduce structured logging, health checks and centralized error monitoring.
- **Date handling:** Display event dates in the user's local time zone while storing timestamps in UTC.

## License

No license has been specified for this project.