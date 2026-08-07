# FortuneTeller — Expense Tracker API

A personal & shared-expense tracking backend built with **ASP.NET Core 10** and **Clean Architecture**. Users track income/expenses across accounts and categories, set monthly budgets, and split shared costs (roommates, trips) inside groups, with automatic debt simplification.

## Architecture

The solution is split into four projects, with dependencies pointing inward only:

```
FortuneDomain          <- Entities, enums, repository interfaces, domain exceptions. No dependencies.
FortuneApplication     <- DTOs, use-case services, FluentValidation validators, mappers. Depends on Domain.
FortuneInfrastructure  <- EF Core DbContext + configurations, repositories, JWT/password services. Depends on Domain + Application.
FortuneTeller          <- ASP.NET Core Web API: controllers, JWT auth, Swagger, exception middleware. Depends on Application + Infrastructure.
```

Business rules (entities, use cases) never reference EF Core, ASP.NET Core, or SQL Server directly — those live only in `FortuneInfrastructure` and `FortuneTeller`, wired up through interfaces.

## Features

- **Auth** — register/login with hashed passwords (ASP.NET Core Identity's PBKDF2 hasher), JWT access tokens + rotating refresh tokens.
- **Accounts** — cash/bank/card/wallet accounts per user.
- **Categories** — user-defined spending categories.
- **Transactions** — income/expense records per account & category, filterable and paged.
- **Budgets** — monthly spending limits per category, with live spent/remaining/progress calculated from transactions.
- **Groups** — shared groups (trips, households) with member invites by email.
- **Group expenses** — split a shared cost Equally, by Percentage, by Exact amount, or by Shares; splits are validated to always reconcile to the expense total.
- **Balances & settlements** — net balance per group member, plus a greedy debt-simplification algorithm that reduces everyone's debts to the minimum number of payments, and a `SettleUp` endpoint to record a payment.

## Tech stack

| Concern | Choice |
|---|---|
| Runtime | .NET 10 / ASP.NET Core Web API |
| Database | SQL Server (via EF Core 10, code-first migrations) |
| Auth | JWT bearer tokens (access + refresh), PBKDF2 password hashing |
| Validation | FluentValidation |
| API docs | Swagger / OpenAPI (Swashbuckle) |

## Getting started

**Prerequisites:** .NET 10 SDK, SQL Server (LocalDB is enough for local dev).

```bash
# Restore & build
dotnet build

# Apply the database schema (the API also does this automatically on startup)
dotnet ef database update --project FortuneInfrastructure --startup-project FortuneTeller

# Run the API
dotnet run --project FortuneTeller
```

Swagger UI is available at `/swagger` in the Development environment.

### Configuration

Set these in `FortuneTeller/appsettings.json` / `appsettings.Development.json` (or environment variables / user-secrets in production):

```json
{
  "ConnectionStrings": { "Database": "<your SQL Server connection string>" },
  "Jwt": {
    "Issuer": "FortuneTeller",
    "Audience": "FortuneTeller.Client",
    "Secret": "<at least 32 characters>",
    "AccessTokenLifetimeMinutes": 15,
    "RefreshTokenLifetimeDays": 7
  }
}
```

The `Secret` in `appsettings.Development.json` is a placeholder for local development only — replace it before deploying anywhere real.

## API overview

| Area | Endpoints |
|---|---|
| Auth | `POST /api/auth/register`, `login`, `refresh`, `logout` |
| Accounts | `GET/POST /api/accounts`, `GET/PUT/DELETE /api/accounts/{id}` |
| Categories | `GET/POST /api/categories`, `GET/PUT/DELETE /api/categories/{id}` |
| Transactions | `GET/POST /api/transactions` (filterable & paged), `GET/PUT/DELETE /api/transactions/{id}` |
| Budgets | `GET/PUT /api/budgets`, `DELETE /api/budgets/{id}` |
| Groups | `GET/POST /api/groups`, `GET /api/groups/{id}`, `POST /api/groups/{id}/members`, `DELETE /api/groups/{id}/members/{userId}` |
| Group expenses | `GET/POST /api/groups/{groupId}/expenses` |
| Settlements | `GET /api/groups/{groupId}/balances`, `GET /api/groups/{groupId}/simplify-debts`, `POST /api/groups/{groupId}/settlements` |

All endpoints except `auth/*` require a `Bearer` JWT access token.
