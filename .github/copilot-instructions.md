# AsyncTaskList Project Instructions

## Technical Stack
- **Framework**: .NET 8.0 ASP.NET Core MVC.
- **Language**: C# 12+ (Prefer primary constructors, collection expressions).
- **Database**: Oracle (via `Oracle.ManagedDataAccess.Core`).
- **ORM**: Dapper for lightweight SQL operations.

## Architecture & Patterns
- **Repository Pattern**: All database interactions must go through `ISqlRepository`.
- **Dependency Injection**: 
  - `ISqlRepository` is registered as Transient.
  - `IHomeService` is registered as Scoped.
- **Transaction Management**: 
  - Transactions are started automatically in `SqlRepository` constructor.
  - Explicitly call `CommitAsync()` or `RollbackAsync()` at the end of a business unit (e.g., in Controller).
- **Async Workflow**:
  - Use `IAsyncEnumerable<T>` for real-time streaming of task results.
  - Use `Task.WhenAny` to process parallel tasks as they complete.

## Coding Standards
- **Naming**: Use PascalCase for methods, classes, and public properties. Use camelCase for private fields (with `_` prefix if not using primary constructors).
- **Async**: Always use `Async` suffix for asynchronous methods. Use `await` instead of `.Result` or `.Wait()`.
- **DI**: Prefer **Primary Constructors** for injecting dependencies into Controllers and Services.

## UI/Frontend
- Built with ASP.NET Core MVC (Razor Views).
- Uses Bootstrap and jQuery.
