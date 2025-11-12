# ThreadPool Starvation Debug

This is an ASP.NET Core demonstration application designed to illustrate and debug ThreadPool starvation issues using different synchronous and asynchronous programming approaches.

## 🎯 Purpose

The project demonstrates the impact of different code execution methods on the .NET ThreadPool:
- Synchronous vs asynchronous execution
- Using `.Wait()` and `.GetAwaiter().GetResult()`
- Using `Task.Run()` and `Task.Factory.StartNew()`
- Impact on performance and available threads

## 📋 Prerequisites

### Required Software
- **.NET 9.0 SDK** or higher
- **SQL Server** (SQL Server Express or full version)
  - Recommended version: SQL Server 2019 or higher
  - Or SQL Server Express (free)
- **Visual Studio 2022** (or Visual Studio Code with C# extension)

### Database
- An empty SQL database named `demo`
- No tables required (the project only uses the `WAITFOR DELAY` command)

## 🚀 Installation and Configuration

### 1. Clone or Download the Project

```bash
git clone <repository-url>
cd ThreadPoolStarvationDebug
```

### 2. Create the Database

Execute this SQL command in SQL Server Management Studio or Azure Data Studio:

```sql
CREATE DATABASE demo;
```

### 3. Configure the Connection String

Edit the `appsettings.json` file in the `ThreadPoolStarvationDebug` project:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Initial Catalog=demo;Integrated Security=True;TrustServerCertificate=True;"
  }
}
```

### 4. Restore NuGet Packages

```bash
dotnet restore
```

### 5. Run the Application

```bash
dotnet run --project ThreadPoolStarvationDebug
```

Or via Visual Studio: press `F5` or click the "Start" button.

## 🔧 Available API Endpoints

The API exposes several endpoints to test different scenarios:

| Endpoint | Description |
|----------|-------------|
| `GET /api/demo/sync` | Synchronous blocking execution |
| `GET /api/demo/async` | Asynchronous execution (best practice) |
| `GET /api/demo/wait` | Using `.Wait()` (sync over async anti-pattern) |
| `GET /api/demo/configureawait` | Using `ConfigureAwait(false).GetAwaiter().GetResult()` (sync over async anti-pattern) |
| `GET /api/demo/taskrun` | Wrapping a synchronous method in `Task.Run()` |
| `GET /api/demo/taskfactory` | Using `Task.Factory.StartNew()` with `LongRunning` |
| `GET /api/demo/threadinfo` | Information about the current ThreadPool state |

### Usage Examples

```bash
# Via curl
curl https://localhost:5001/api/demo/async

# Via PowerShell
Invoke-WebRequest -Uri https://localhost:5001/api/demo/async

# Via browser
https://localhost:5001/api/demo/threadinfo
```

## 📊 Monitoring Features

### Application Insights
The project is configured with Application Insights to collect:
- ThreadPool thread count
- ThreadPool queue length
- Completed work items count

### Logs
ThreadPool statistics are logged with each request:
- Active thread count
- Pending work items count
- Completed work items count

## 📦 Main Dependencies

- **Microsoft.ApplicationInsights.AspNetCore** (2.23.0) - Telemetry and monitoring
- **Microsoft.ApplicationInsights.EventCounterCollector** (2.23.0) - Event counter collection
- **Microsoft.ApplicationInsights.PerfCounterCollector** (2.23.0) - Performance counter collection
- **Microsoft.Data.SqlClient** (6.0.1) - SQL Server client

## 🧪 Load Testing

To observe ThreadPool starvation, use a load testing tool such as:

### Bombardier (https://github.com/codesenberg/bombardier)
```bash
bombardier https://localhost:7005/api/demo/sync -c 200 -t 2s -d 10s
bombardier https://localhost:7005/api/demo/wait -c 200 -t 2s -d 10s
bombardier https://localhost:7005/api/demo/async -c 200 -t 2s -d 10s
```

### PowerShell
```powershell
1..100 | ForEach-Object -Parallel {
    Invoke-WebRequest -Uri https://localhost:5001/api/demo/sync
} -ThrottleLimit 50
```

## 📚 Demonstrated Concepts

### 1. Synchronous Execution (`/sync`)
Blocks the ThreadPool thread for the entire duration of the SQL operation.

### 2. Asynchronous Execution (`/async`)
Releases the thread while waiting for the SQL operation (best practice).

### 3. Anti-patterns
- `.Wait()`: Blocks the thread and can cause deadlocks
- `ConfigureAwait(false).GetAwaiter().GetResult()`: Similar to `.Wait()`

### 4. Workarounds (not recommended)
- `Task.Run()`: Uses an additional ThreadPool thread
- `Task.Factory.StartNew()` with `LongRunning`: Creates a dedicated thread

## ⚠️ Warnings

- The anti-patterns demonstrated should **not** be used in production
- The best practice is to use `async/await` end-to-end

## 📖 Resources

- [Debug ThreadPool starvation](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-threadpool-starvation)
- [David Fowler : Async Guidance](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md)
- [Async/await best practices](https://docs.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming)
- [ASP.NET Core Best Pratices](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-9.0#avoid-blocking-calls)
- [ThreadPool Class](https://docs.microsoft.com/en-us/dotnet/api/system.threading.threadpool)
- [Task-based Asynchronous Pattern](https://docs.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/task-based-asynchronous-pattern-tap)

