# SchoolPortal

A school management system built as a **modular monolith** with ASP.NET Core. Students, grades and attendance run as separate modules inside one deployable application, with enforced boundaries between them.

SchoolPortal started as two separate apps (Students and Grades) talking over HTTP. They shared a domain and had to be deployed together, so they were really *accidental microservices*. This rebuild merges them into one app where modules communicate through in-process events and architecture tests guard the boundaries.

## Features (v1)

- **Students**: records, addresses and status, with search by name, email and status.
- **Grades**: academic years, terms, courses, course sections, assessments and grades. Final grades are computed from weighted assessments.
- **Attendance**: per-class-session attendance (recurring sessions, concrete occurrences, per-student status and notes).
- **Authentication**: cookie login with three roles, **Admin**, **Teacher** and **Student**.
  - Admin creates users and assigns roles. There is no public self-registration, so nobody can promote themselves.
  - Only Admin and Teacher can mark attendance. A Student sees only their own history.
- **CSRF protection**: state-changing `/api/*` requests require the `X-CSRF-TOKEN` header. The Students, Grades and Attendance frontends fetch the token from `GET /api/antiforgery/token` and send it on every `fetch` call.
- **Email**: confirmation on user creation, and email-enumeration-safe password reset.
- **Health endpoints**: `/health`, `/health/live` and `/health/ready`, with a check per module.
- **Tests**: unit, integration and architecture tests (`SchoolPortal.ArchTests`) all pass.

## After v1

Planned, not part of v1:

- **Bulk grade import** (Grades):
  - One assessment per file: pick the assessment, then upload its file.
  - Partial import: valid rows are saved, invalid rows are skipped and reported with their row number and reason.
  - Students who already have a grade for that assessment are not overwritten. They are returned as a warning.
- Notifications module (event-driven emails, such as a term-end grade report)
- Nightly GPA and ranking recalculation job
- Caching for class rankings and course rosters
- Rate limiting on login and public grade-lookup endpoints
- Arabic / English localization
- Grade audit trail (who changed a grade, from what to what, and when)

## Architecture

```
SchoolPortal.sln
├── SchoolPortal.SharedKernel          # Result type, base entities, hand-built mediator
├── SchoolPortal.Identity              # ASP.NET Core Identity, cookie auth, roles
├── SchoolPortal.Students.{Domain,Application,Infrastructure}
├── SchoolPortal.Grades.{Domain,Application,Infrastructure}
├── SchoolPortal.Attendance.{Domain,Application,Infrastructure}
├── SchoolPortal.Web                   # Minimal API endpoints and thin pages
└── SchoolPortal.ArchTests             # NetArchTest rules that enforce the boundaries
```

- **One PostgreSQL schema per module**, so modules cannot join across each other's tables by accident.
- **CQRS with a hand-built mediator**. Cross-module communication uses published events, never direct calls into another module.
- **EF Core for writes in every module, plus Dapper for read queries in Grades**.
- **Minimal APIs** grouped per module.
- **Domain entities** use private constructors and `Result`-returning factory methods, so invalid state cannot be created.
- **Admin overrides** are explicit domain methods (for example `OverrideScore`), never boolean bypass flags. Nothing is hard-deleted; every soft delete has a `Restore()`.

### Data access in the Grades module

Grades uses both EF Core and Dapper, split by what the code is doing:

| Job | Tool | Why |
|---|---|---|
| Load an entity, change it, save it (`GetByIdAsync`, `Add`, `Update`, existence checks) | **EF Core** | The change tracker, the audit interceptor (`CreatedAt`, `UpdatedAt`, user ids) and domain-event publishing all happen on `SaveChanges`. |
| Read data for display (lists, per-student grades, class rankings, GPA reports) | **Dapper** | Read-only, often joins several tables or aggregates across students. Hand-written SQL is clearer than translated LINQ, and nothing needs tracking. |

Each directory class (for example `GradeDirectory`) takes both the `GradesDbContext` and an `NpgsqlDataSource`, so the interfaces and command handlers do not change.

Things to remember when writing Dapper queries here:

- **Soft delete is not automatic.** EF's global query filter (`!IsDeleted`) does not apply to Dapper. Every raw query must include `"IsDeleted" = FALSE` for each table it touches, or deleted rows will appear.
- **`DateOnly` needs a type handler.** `DateOnlyTypeHandler` is registered once in `AddGradesModule`. Without it, Dapper cannot read Postgres `date` columns into `DateOnly`.
- **Dapper reads use their own connection**, so they only see data that has already been saved. Unsaved changes sitting in the EF change tracker are invisible to them.
- **Enums are stored as text** (`HasConversion<string>()`), so a column such as `"Status"` can be selected directly and already matches the enum's `ToString()`.

### Boundaries enforced by architecture tests

- Domain has no dependency on EF Core, Dapper, ASP.NET Core or outer layers.
- Application does not depend on Infrastructure or Web.
- Modules do not reference each other's layers directly.
- Dapper is confined to `Grades.Infrastructure`.
- Only `Program.cs` (the composition root) in Web references module Infrastructure.

## Tech stack

ASP.NET Core · EF Core 10 (writes and migrations) · Dapper (Grades reads) · PostgreSQL · ASP.NET Core Identity · Brevo (email) · xUnit · NetArchTest

## Getting started

### Prerequisites

- .NET SDK (matching the version in the solution)
- PostgreSQL

### Run

```bash
git clone <your-repo-url>
cd SchoolPortal

dotnet user-secrets set "ConnectionStrings:SchoolPortalDb" "Host=localhost;Database=schoolportal;Username=postgres;Password=<password>" --project SchoolPortal.Web

dotnet run --project SchoolPortal.Web
```

On startup the app creates each module's schema and seeds roles and sample data, so every page and dropdown has something to show. Without Brevo credentials in development, emails are written to the console.

Check that it is running:

```bash
curl http://localhost:<port>/health
```

### Migrations

`dotnet ef` cannot use your user secrets, because the design-time factory is created without dependency injection. The Grades factory reads the connection string from an environment variable instead, so no password is committed:

```bash
# PowerShell
$env:ConnectionStrings__SchoolPortalDb = "Host=localhost;Database=schoolportal;Username=postgres;Password=<password>"
```

### Test

```bash
dotnet test
```
