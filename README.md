# SwimAnalytics

A web app for analysing swimming performance: results, personal bests, progression over time, World Aquatics points, race pacing and an optimal medley relay picker.


## What it does

- Browse swimmers with search and pagination
- Swimmer page with personal bests (per event and pool length) and a progression chart
- Leaderboard per event and pool length (25 m / 50 m)
- Result page with World Aquatics points, a split table and a pacing chart
- Medley relay optimizer: picks the 4 swimmers (back, breast, fly, free) with the lowest combined time, optionally from a single club
- Form for adding results with splits, validated on both client and server

## Tech stack

- **Backend:** ASP.NET Core Web API (C#), EF Core (code-first), LINQ, SQL Server
- **Frontend:** Angular 19 (standalone components, signals, reactive forms), RxJS, Chart.js

## Architecture

The backend is split into four projects:

- `Domain`: entities and enums
- `Data`: `DbContext`, EF configurations, migrations, seeding
- `Application`: DTOs, services and the calculators (points, pacing, relay)
- `Api`: thin controllers that only deal with HTTP

## Running it locally

You need the .NET SDK, SQL Server, Node.js and the Angular CLI.

**Backend**

```
cd server
dotnet ef database update --project SwimAnalytics.Data --startup-project SwimAnalytics.Api
dotnet run --project SwimAnalytics.Api
```

Set your connection string in `SwimAnalytics.Api/appsettings.json`. In Development the database is seeded with sample data on first start.

**Frontend**

```
cd frontend/swim-client
npm install
ng serve
```

Then open http://localhost:4200. The API address is set in `src/app/core/api.config.ts` (default `http://localhost:5130/api`).
