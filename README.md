# BookingApp

A small appointment booking API, built while learning .NET coming from a Laravel background. Work in progress — expect rough edges and churn.

## Stack

- .NET 10 minimal APIs (no controllers)
- EF Core + PostgreSQL (Npgsql)
- ASP.NET Core Identity (cookie auth, role + resource-based authorization)
- Bogus for seed data

## Structure

```
src/BookingApp.Api/
  Auth/         custom claims factory, roles, authorization requirements/handlers
  Data/         DbContext, entity configurations, seeders, factories
  Endpoints/    route groups (auth, services, appointments)
  Models/       domain entities (Appointment, Client, Staff, Service, Invoice, ...)
  Repositories/ data access abstractions
  Services/     business logic (e.g. availability calculation)
  Migrations/   EF Core migrations
```

## Running locally

1. Start Postgres:
   ```
   docker compose up -d
   ```
2. Run the API (migrations and seed data run automatically in Development):
   ```
   dotnet run --project src/BookingApp.Api
   ```

## Status

Actively exploring .NET/EF Core idioms and how they compare to the Laravel/Eloquent world — Identity setup, authorization policies, and the appointment/availability domain are the current focus.
