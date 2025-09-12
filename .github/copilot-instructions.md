# Copilot Instructions for xgplib

## Project Context

- This is a .NET Worker Service project targeting .NET 9.
- The main purpose is to synchronize data from the IGDB API (games, genres, etc.) into a local database.
- The codebase follows Clean Architecture principles, with clear separation between application, domain, and infrastructure layers.
- Use dependency injection for all services and repositories.
- Logging is handled via Serilog and Microsoft.Extensions.Logging.

## File Structure

- `XgpLib.SyncService.Application`: Application layer (use cases, DTOs, interfaces).
- `XgpLib.SyncService.Domain`: Domain entities and interfaces.
- `XgpLib.SyncService.Infrastructure`: Implementations for repositories, API clients, etc.
- `XgpLib.SyncService`: Worker entry point and DI setup.

---

**When generating code for this project, always follow these instructions to ensure consistency, maintainability, and alignment with the project's architecture and goals.**
