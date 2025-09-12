---
applyTo: "**/*.cs"
---
# Project coding standards for CSharp files

## Code Style

- Use C# 13.0 features where appropriate.
- Prefer expression-bodied members for simple properties and methods.
- Use `var` when the type is obvious from the right side of the assignment.
- Use collection expressions (e.g., `[]`) for array initializations.
- Use `ILogger<T>` for logging.
- Use `System.Text.Json` for serialization.

## Example Patterns

- **Use Case Pattern:** Encapsulate business logic in use case classes (e.g., `SyncGamesUseCase`).
- **Repository Pattern:** Abstract data access behind repository interfaces.
- **BackgroundService:** For long-running background tasks, inherit from `BackgroundService`.

## What to Avoid

- Avoid business logic in controllers or worker entry points.
- Avoid static classes for services or repositories.
- Avoid tightly coupling infrastructure code with domain logic.
