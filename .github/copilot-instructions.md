# AsyncTaskList Project Instructions

## Project Purpose
Demo app showcasing **real-time async task processing** using `IAsyncEnumerable<T>` with `Task.WhenAny` to stream parallel task results as they complete. Demonstrates transaction rollback when any task fails.

## Technical Stack
- **Framework**: .NET 8.0 ASP.NET Core MVC
- **Language**: C# 12+ (prefer primary constructors, collection expressions)
- **Database**: Oracle Free 23c (via `Oracle.ManagedDataAccess.Core`)
- **ORM**: Dapper for raw SQL execution
- **Frontend**: Razor Views, Bootstrap 5, jQuery with Fetch API streaming

## Critical Architecture Patterns

### Multi-Project Solution
- **AsyncTaskList** (Main): MVC app (`AsyncEnumerable_TEST_MVC` namespace)
- **DbService**: Separate class library for data access abstraction

### Transaction-Per-Request Pattern
```csharp
// SqlRepository auto-starts transaction in constructor
public SqlRepository(IDbConnection dbConnection) {
    _connection.Open();
    _transaction = _connection.BeginTransaction(); // Always starts here
}
```
**Key Workflow**: Services receive `ISqlRepository` as method parameters (NOT constructor injection), allowing Controllers to decide commit/rollback based on aggregate task results.

### Async Streaming Pattern (Core Feature)
See `HomeController.ExecuteTasks()`:
```csharp
public async IAsyncEnumerable<BaseModel> ExecuteTasks() {
    var tasks = new List<Task<BaseModel>> { /* parallel tasks */ };
    while (tasks.Count > 0) {
        await Task.WhenAny(tasks); // Wait for any task
        var finishedTasks = tasks.Where(t => t.IsCompleted).ToList();
        foreach (var task in finishedTasks) {
            tasks.Remove(task);
            yield return await task; // Stream result immediately
        }
    }
    // Rollback if any task failed
    if (results.Any(r => r.IsSuccess == false))
        await sqlRepository.RollbackAsync();
    else
        await sqlRepository.CommitAsync();
}
```
Frontend uses Fetch API to consume streamed JSON chunks in real-time.

## Dependency Injection Rules
```csharp
// Program.cs registrations
builder.Services.AddScoped<IJobExecutionService, JobExecutionService>();
builder.Services.AddTransient<IDbConnection>(sp => new OracleConnection(...));
builder.Services.AddTransient<ISqlRepository, SqlRepository>(); // New instance per request
```
- **ISqlRepository**: Transient (one per HTTP request)
- **IJobExecutionService**: Scoped
- Services receive `ISqlRepository` via method params, NOT constructor

## Database Management

### Local Development
Run Oracle via Docker Compose:
```bash
docker-compose up -d
```
Connection string in `appsettings.json`:
```
User Id=system;Password=SecretPassword123;Data Source=localhost:1521/freepdb1
```

### Schema Initialization
Done in `Program.cs` on app startup using PL/SQL anonymous block with `ORA-00955` error suppression (table already exists). Production should use proper migration tools.

## Coding Standards
- **Primary Constructors**: Always use for DI in Controllers/Services (`public class HomeController(ILogger logger)`)
- **Async Naming**: All async methods end with `Async`
- **Never block**: Use `await`, never `.Result` or `.Wait()` (except in sync contexts like `Program.cs` startup)
- **Namespaces**: Main project uses `AsyncEnumerable_TEST_MVC` (historical name), DbService uses `DbService`

## Common Pitfalls
1. **Don't inject ISqlRepository in constructors** - pass it as method parameter to control transaction scope
2. **Always call CommitAsync/RollbackAsync** - SqlRepository doesn't auto-commit
3. **Dispose management**: SqlRepository.CommitAsync/RollbackAsync call Dispose internally
4. **Frontend streaming**: Client must parse NDJSON (newline-delimited JSON) when consuming `IAsyncEnumerable` endpoints

## Key Files
- [Program.cs](AsyncTaskList/Program.cs) - DI setup, DB init
- [HomeController.cs](AsyncTaskList/Controllers/HomeController.cs) - Async streaming demo
- [SqlRepository.cs](DbService/Services/SqlRepository.cs) - Transaction management
- [MultiTask.cshtml](AsyncTaskList/Views/Home/MultiTask.cshtml) - Real-time progress UI
