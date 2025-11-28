# Commit Message Guidelines

## Format

Follow the **Conventional Commits** specification with this structure:

```
<type>: <description>

[optional body]

[optional footer(s)]
```

## Types

Use one of the following commit types:

- **feat**: New feature or functionality
- **fix**: Bug fix
- **refactor**: Code restructuring without changing behavior
- **docs**: Documentation changes only
- **style**: Code formatting, whitespace, or style changes (no logic changes)
- **perf**: Performance improvements
- **test**: Adding or updating tests
- **build**: Changes to build system or dependencies
- **ci**: Changes to CI/CD configuration
- **chore**: Maintenance tasks, tooling, or other non-production code

## Description Rules

1. **Use imperative mood**: "Add feature" not "Added feature" or "Adds feature"
2. **Start with a capital letter**: "Add XML documentation"
3. **No period at the end**: "Update dependencies" not "Update dependencies."
4. **Keep it concise**: Aim for 50-72 characters
5. **Be specific and clear**: Describe **what** changed, not **why** (use body for why)

## Examples

### Good Commits
```
feat: Add GetGenreById query and API endpoint
fix: Correct genre lookup in sync workflow
refactor: Introduce UnitOfWork pattern for repositories
docs: Update README with Docker setup instructions
perf: Optimize database queries in genre sync
build: Upgrade to .NET 9 and update dependencies
chore: Remove unused using directives
```

### Multi-line Commits (when needed)
```
feat: Add RabbitMQ integration and message broker API

- Implement RabbitMqService for message publishing
- Add MessageBrokerController with publish endpoint
- Configure RabbitMQ connection in appsettings

Closes #42
```

## Additional Guidelines

### Scope (Optional)
You may add a scope in parentheses after the type:
```
feat(api): Add genres controller
refactor(workers): Combine sync workers into single worker
fix(db): Correct migration for genre relationships
```

### Body
- Use the body to explain **why** the change was made
- Separate from description with a blank line
- Wrap at 72 characters per line
- Can include multiple paragraphs

### Footer
- Reference issues: `Closes #123`, `Fixes #456`, `Relates to #789`
- Breaking changes: `BREAKING CHANGE: Description of what breaks`

### Breaking Changes
For breaking changes, use one of these formats:
```
feat!: Remove obsolete GetAllAsync methods

BREAKING CHANGE: GetAllAsync is no longer available in repositories.
Use GetPagedAsync instead.
```

## Anti-Patterns to Avoid

❌ **Vague descriptions**
```
fix: Fix bug
update: Update code
refactor: Refactor stuff
```

❌ **Past tense**
```
Added new feature
Fixed the bug
Updated dependencies
```

❌ **Multiple changes in one commit**
```
feat: Add API endpoint, fix bug, update docs, and refactor service
```

❌ **Periods at the end**
```
feat: Add new feature.
```

❌ **Too long or too short**
```
fix: Fix
feat: Add comprehensive integration with third-party authentication system including OAuth2 flow
```

## Best Practices

1. **One logical change per commit**: Each commit should represent a single, focused change
2. **Test before committing**: Ensure code builds and tests pass
3. **Commit frequently**: Small, atomic commits are better than large ones
4. **Write for others**: Assume someone will read the commit history to understand changes
5. **Use present tense**: Describe what the commit does, not what you did

## Project-Specific Conventions

- When working with **CQRS patterns**, specify command/query in scope: `feat(cqrs): Add SyncGenresCommand`
- For **migrations**, always mention entity: `build(db): Add migration for Game-Genre relationship`
- For **dependency updates**, group related packages: `build: Update EF Core dependencies to version 9.x`
- For **worker changes**, specify worker name: `refactor(workers): Increase polling interval for IGDB workers`
- For **Docker/infrastructure**, use appropriate scope: `build(docker): Add .dockerignore and update compose configuration`

---

**Remember**: A well-written commit message helps you and your team understand the project's history and evolution.
