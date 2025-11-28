# Copilot Instructions for xgplib

## Project Context

- This is a .NET 9 solution with multiple projects including a Worker Service and a Web API.
- The main purpose is to synchronize data from the [IGDB API](https://api-docs.igdb.com/#getting-started) (games, genres, etc.) into a local PostgreSQL database.
- The project includes message broker capabilities using RabbitMQ for asynchronous communication.
- The codebase follows Clean Architecture principles, with clear separation between application, domain, and infrastructure layers.
- Use dependency injection for all services and repositories.
- Logging is handled via Serilog and Microsoft.Extensions.Logging.

## Project Structure

- `XgpLib.SyncService.Api`: ASP.NET Core Web API with controllers for genres, message broker operations, and OpenAPI/Swagger documentation.
- `XgpLib.SyncService.WorkerServices`: Background workers (IgdbGamesSyncWorker, IgdbGenresSyncWorker) for scheduled synchronization tasks.
- `XgpLib.SyncService.Application`: Application layer containing:
  - Use cases (SyncGames, SyncGenres, message broker operations)
  - DTOs (IgdbGame, IgdbGenre, PublishMessage, ReceiveMessages, etc.)
  - Abstractions (Data, Messaging, Services interfaces)
  - Integration Events (SyncGamesIntegrationEvent, SyncGenresIntegrationEvent)
- `XgpLib.SyncService.Domain`: Domain entities and interfaces.
- `XgpLib.SyncService.Infrastructure`: Implementations for:
  - Repositories (PostgreSQL with Entity Framework Core)
  - API clients (IGDB with Twitch OAuth)
  - Message broker services (RabbitMQ)
  - HTTP handlers and configurations
- `XgpLib.SyncService.CrossCutting`: Dependency injection setup and service registrations.

---

**When generating code for this project, always follow these instructions to ensure consistency, maintainability, and alignment with the project's architecture and goals.**
