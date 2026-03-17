# MoneyMap API

MoneyMap API is a minimalist personal expense tracker backend built with ASP.NET Core 8 for a clean GitHub showcase. It keeps the architecture simple, uses JWT auth, stores data in SQL Server through EF Core, and ensures every authenticated user only sees their own categories, transactions, and reports.

## Stack

- .NET 8 Web API
- EF Core + SQL Server
- JWT authentication
- Swagger / OpenAPI
- xUnit integration tests
- Docker + Docker Compose

## Features

- Register and login with JWT token issuance
- Create and list personal categories
- Create, list, update, and delete personal transactions
- Monthly summary report with income, expenses, net total, and category breakdown
- Global exception middleware for consistent JSON errors
- User ownership enforced on every non-auth endpoint
- EF Core migrations included in the repo

## Project Structure

```text
MoneyMap
|-- MoneyMap.Api
|   |-- Configuration
|   |-- Controllers
|   |-- Data
|   |-- DTOs
|   |-- Exceptions
|   |-- Middleware
|   |-- Migrations
|   |-- Models
|   |-- Services
|   `-- Program.cs
|-- MoneyMap.Tests
|   |-- Auth
|   |-- Infrastructure
|   `-- Transactions
|-- docker-compose.yml
|-- global.json
`-- README.md
```

## API Endpoints

### Auth

- `POST /api/auth/register`
- `POST /api/auth/login`

### Categories

- `GET /api/categories`
- `POST /api/categories`

### Transactions

- `GET /api/transactions`
- `POST /api/transactions`
- `PUT /api/transactions/{id}`
- `DELETE /api/transactions/{id}`

### Reports

- `GET /api/reports/monthly-summary`

## Authentication

All endpoints except `/api/auth/register` and `/api/auth/login` require a bearer token.

Example header:

```http
Authorization: Bearer <jwt-token>
```

## Request Notes

- Transaction `amount` should always be positive.
- Whether a transaction counts as income or expense is determined by the selected category type.
- `GET /api/transactions` accepts optional `year` and `month` query parameters together.
- `GET /api/reports/monthly-summary` accepts optional `year` and `month` query parameters together. If omitted, the API returns the current UTC month.

## Local Development

### 1. Restore tools and packages

```bash
dotnet tool restore
dotnet restore
```

### 2. Start SQL Server

```bash
docker compose up -d sqlserver
```

SQL Server will be available on `localhost,14333`.

### 3. Run the API

```bash
dotnet run --project MoneyMap.Api
```

Swagger UI is available at the URL printed by `dotnet run`, typically ending in `/swagger`.

The API applies EF Core migrations automatically on startup when `Database:ApplyMigrationsOnStartup` is `true`.

## Docker

Run the full stack:

```bash
docker compose up --build
```

Services:

- API: `http://localhost:8080/swagger`
- SQL Server: `localhost,14333`

## Migrations

The initial migration is already included.

Create a new migration:

```bash
dotnet tool restore
dotnet dotnet-ef migrations add <MigrationName> --project MoneyMap.Api --startup-project MoneyMap.Api
```

Apply migrations manually:

```bash
dotnet tool restore
dotnet dotnet-ef database update --project MoneyMap.Api --startup-project MoneyMap.Api
```

## Testing

Run the test suite:

```bash
dotnet test
```

The tests cover:

- register/login flow
- JWT protection on secured endpoints
- end-to-end category and transaction workflow
- monthly summary calculations
- user ownership boundaries for transactions

## Example Payloads

### Register

```json
{
  "fullName": "Ada Lovelace",
  "email": "ada@example.com",
  "password": "Password123!"
}
```

### Create Category

```json
{
  "name": "Groceries",
  "type": "Expense"
}
```

### Create Transaction

```json
{
  "categoryId": 1,
  "amount": 125.50,
  "description": "Weekend grocery run",
  "occurredOn": "2026-03-15T00:00:00Z"
}
```

### Monthly Summary

```text
GET /api/reports/monthly-summary?year=2026&month=3
```

## Design Choices

- Service-based architecture to keep controllers thin
- DTOs for API contracts instead of exposing EF entities directly
- Custom exception middleware for predictable error responses
- Per-user filtering in services so data ownership is enforced centrally
- Integration-style tests for realistic request/response coverage without overengineering
