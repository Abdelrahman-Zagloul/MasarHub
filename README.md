# MasarHub API

<a href="https://learn.microsoft.com/en-us/aspnet/core/" target="_blank"><img src="https://img.shields.io/badge/.NET-10.0-blueviolet?logo=dotnet" alt=".NET 10" /></a>
<a href="https://masarhub.runasp.net/swagger/index.html" target="_blank"><img src="https://img.shields.io/badge/Swagger-API%20Docs-green?logo=swagger" alt="Swagger" /></a>
<a href="https://masarhub.runasp.net/scalar" target="_blank"><img src="https://img.shields.io/badge/Scalar-API%20Reference-black" alt="Scalar" /></a>
<a href="https://opensource.org/licenses/MIT" target="_blank"><img src="https://img.shields.io/badge/License-MIT-yellow.svg" alt="License: MIT" /></a>
<a href="https://www.linkedin.com/in/abdelrahman-zagloul/" target="_blank"><img src="https://img.shields.io/badge/LinkedIn-Abdelrahman%20Zagloul-blue?logo=linkedin" alt="LinkedIn" /></a>

A **modular ASP.NET Core Web API** for an online learning platform, built with **Clean Architecture**, **DDD principles**, **CQRS with MediatR**, **Rich Domain Model**, and **Domain Events**.

---

## Table of Contents

- [Overview](#overview)
- [Project Status](#project-status)
- [Key Features](#key-features)
  - [Authentication System](#authentication-system)
  - [Localization](#localization)
  - [Notifications & Realtime](#notifications-and-realtime)
  - [Caching & Background Jobs](#caching--background-jobs)
  - [Storage](#storage)
  - [API Infrastructure](#api-infrastructure)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Testing](#testing)
- [API Features](#api-features)
- [API Documentation](#api-documentation)
- [Setup & Run](#setup--run)

---

## Overview

- **Architecture:** Clean Architecture + DDD + CQRS (MediatR) with Rich Domain Model & Domain Events
- **Authentication:** JWT + Refresh Tokens (HttpOnly cookies)
- **Identity:** ASP.NET Core Identity with role-based users (Admin, Instructor, Student)
- **2FA:** Email, SMS, authenticator app, and recovery codes
- **External Login:** Google, GitHub, LinkedIn, Facebook
- **Persistence:** EF Core + SQL Server + Dapper
- **Caching:** Redis with hybrid cache fallback
- **Background Jobs:** Hangfire (SQL Server storage)
- **Realtime:** SignalR notification hub
- **Localization:** JSON-based (English / Arabic)
- **API Docs:** Swagger + Scalar
- **Logging:** Serilog (Console, File, Seq)

---

## Project Status

The project is in **active development**. Instructor features are complete, student features are pending.

### ✅ Instructor — Complete

| Area | Status |
|------|--------|
| Authentication & account management | ✅ Complete |
| Two-factor authentication (email, SMS, authenticator, recovery codes) | ✅ Complete |
| External OAuth login (Google, GitHub, LinkedIn, Facebook) | ✅ Complete |
| Category management (CRUD, reorder, parent/child hierarchy) | ✅ Complete |
| Course management (CRUD, approval workflow, thumbnail, filters, instructor query) | ✅ Complete |
| Module management (CRUD, reorder, background announcements) | ✅ Complete |
| Lesson management (article/video, archive/preview, reorder, attachments, thumbnail) | ✅ Complete |
| Exam management (CRUD, publish/unpublish) | ✅ Complete |
| Question management (CRUD, get by ID, get by exam with QuestionType filter) | ✅ Complete |
| File storage (Cloudinary with upload signature + auto cleanup) | ✅ Complete |
| Real-time notifications (SignalR hub with user groups) | ✅ Complete |
| Background jobs (Hangfire for email, notifications, cleanup) | ✅ Complete |
| Unit test coverage (handler + validator for every feature) | ✅ Complete |

### ⬜ Student & Admin — Pending

| Area | Status |
|------|--------|
| Enrollments & course enrollment | ⬜ Pending |
| Course browsing & search | ⬜ Pending |
| Lesson viewing & progress tracking | ⬜ Pending |
| Exam attempts & answers | ⬜ Pending |
| Certificates module | ⬜ Pending |
| Orders & payments | ⬜ Pending |
| Coupons & discounts | ⬜ Pending |
| Course reviews & ratings | ⬜ Pending |
| Instructor profiles & social links | ⬜ Pending |
| Wishlist | ⬜ Pending |
| Admin dashboard & analytics | ⬜ Pending |
| User management | ⬜ Pending |
| Content moderation | ⬜ Pending |
| Platform settings | ⬜ Pending |

---

## Key Features

### Authentication System

Complete authentication suite including register, login, 2FA, external auth, password management, email confirmation, and token management.

#### Authentication & Authorization
- Student & instructor registration
- Login / logout with JWT + refresh tokens
- Email confirmation & resend confirmation
- Password management (change, forget, reset, verify)
- Role-based authorization (Admin, Instructor, Student)

#### Two-Factor Authentication
- Enable / disable 2FA
- Email & SMS verification codes
- Authenticator app setup & verification
- Recovery codes generation & verification

#### External Authentication
- Google, GitHub, LinkedIn, Facebook OAuth login
- Isolated external auth configuration

---

### Localization

- JSON-based provider (English / Arabic)
- Culture middleware with `Accept-Language` header
- Cached localization resources

### Notifications and Realtime

- Notification domain model with priorities and types
- Background notification creation via Hangfire
- SignalR hub for real-time delivery
- User group mapping (admins, instructors, students)

### Caching & Background Jobs

- Redis cache service with hybrid memory/Redis fallback for better performance and fault tolerance
- Hangfire dashboard & job processing
- Background email and notification workflows

### Storage

- Cloudinary integration for file uploads
- Upload signature generation
- Automatic cleanup of stale resources (thumbnails, attachments, videos)

### API Infrastructure

- Hybrid URL + header API versioning
- Rate limiting (global, sensitive, OTP, strict)
- CORS for development and production
- Custom JWT challenge & forbidden responses
- Problem Details for consistent error responses

---

## Architecture

Clean Architecture with 4 layers + tests:

```text
MasarHub.slnx
├── src/
│   ├── MasarHub.API/                  # Presentation layer
│   │   ├── Controllers/V1/            # API endpoints
│   │   ├── Middlewares/                # Global exception handler, etc.
│   │   ├── Filters/                   # Authorization filters
│   │   ├── SignalR/                   # Real-time hubs
│   │   ├── Localization/              # ar.json, en.json
│   │   └── wwwroot/EmailTemplates/    # HTML email templates
│   │
│   ├── Core/
│   │   ├── MasarHub.Application/      # CQRS commands/queries, handlers, validators
│   │   │   ├── Features/              # Per-feature modules
│   │   │   ├── Common/                # Behaviors, extensions, pagination, results
│   │   │   └── Abstractions/          # Interfaces (queries, repositories, services)
│   │   │
│   │   └── MasarHub.Domain/           # Entities, value objects, domain events
│   │       ├── Common/                # Base classes, guards, results, errors
│   │       └── Modules/               # Categories, Courses, Exams, etc.
│   │
│   └── Infrastructure/
│       ├── MasarHub.Infrastructure/   # External services, identity, jobs
│       └── MasarHub.Infrastructure.Persistence/  # EF Core, Dapper, migrations
│
└── tests/
    ├── Architecture/
    │   └── MasarHub.ArchitectureTests/          # Layer dependency tests
    ├── Integration/
    │   ├── MasarHub.API.IntegrationTests/       # API endpoint integration
    │   └── MasarHub.Infrastructure.IntegrationTests/  # Persistence integration
    └── Unit/
        ├── MasarHub.Application.UnitTests/      # Handler + validator tests
        ├── MasarHub.Domain.UnitTests/           # Entity behavior tests
        └── MasarHub.Infrastructure.UnitTests/   # Infrastructure service tests
```

### Architecture Highlights

- **CQRS** with MediatR for clean command/query separation
- **FluentValidation** pipeline behavior for automatic validation
- **Result pattern** for predictable error handling
- **Dapper** for read queries (performance), **EF Core** for writes (consistency)
- **Repository + Unit of Work** for data access abstraction
- **Domain events** for side-effect separation
- **Dependency injection** organized per layer

---

## Tech Stack

### Core & Architecture
ASP.NET Core (.NET 10), Clean Architecture, CQRS, MediatR, FluentValidation, Result Pattern

### Data & Persistence
Entity Framework Core, SQL Server, ASP.NET Core Identity, Dapper, Generic Repository, Unit of Work

### Authentication & Security
JWT Authentication, Refresh Tokens, HttpOnly Cookies, Role-Based Authorization, 2FA, External OAuth

### Caching & Background
Redis, StackExchange.Redis, Hangfire + SQL Server storage

### API & Documentation
Swagger / OpenAPI, Scalar, API Versioning, Rate Limiting, CORS, Problem Details

### Realtime & Notifications
SignalR, Notification Hub, Background notification jobs

### Email & SMS
SMTP, HTML templates, Twilio SMS

### File Storage
Cloudinary (images, videos, attachments)

### Logging
Serilog (Console, File, Seq)

---

## Testing

🔹 Testing total **817** test cases (unit + Architecture + integration)  

| Project | Type | Tests | Scope |
|---------|------|:-----:|-------|
| `MasarHub.ArchitectureTests` | Architecture | 9 | Layer dependencies, naming conventions |
| `MasarHub.Application.UnitTests` | Unit | 535 | Handler + validator tests for all features |
| `MasarHub.Domain.UnitTests` | Unit | 273 | Entity behavior (categories, courses, lessons, exams) |
| `MasarHub.Infrastructure.UnitTests` | Unit | — | Infrastructure services |
| `MasarHub.API.IntegrationTests` | Integration | — | API endpoint integration |
| `MasarHub.Infrastructure.IntegrationTests` | Integration | — | Persistence integration |

---

## API Features

- Hybrid URL + header API versioning (`/api/v1/...` or `api-version` header)
- Swagger UI & Scalar with JWT bearer authentication
- AutoResponseType filter (infers success types from MediatR, auto-adds response schemas)
- Global Problem Details error handling
- Sliding window rate limiting (Global, Sensitive, OTP, Strict)
- CORS for development & production
- Forwarded headers for reverse proxies
- Serilog request logging
- JSON enum serialization as strings

---

## API Documentation

Interactive API docs are available at:

- **Swagger** — [`https://masarhub.runasp.net/swagger/index.html`](https://masarhub.runasp.net/swagger/index.html)
- **Scalar** — [`https://masarhub.runasp.net/scalar`](https://masarhub.runasp.net/scalar)

For a complete endpoint reference, explore Swagger or Scalar.

---

## Setup & Run

### Run Locally

#### 1. Clone the Repository

```bash
git clone https://github.com/Abdelrahman-Zagloul/MasarHub.git
cd MasarHub
```

#### 2. Configure `appsettings.json`

Update `src/MasarHub.API/appsettings.json` with your local values.

#### 3. Restore Packages

```bash
dotnet restore
```

#### 4. Run the Application

```bash
dotnet run --project src/MasarHub.API
```

The API will start on:
- `https://localhost:7232`
- `http://localhost:5266`

### Database Migrations

```bash
dotnet ef database update --project src/Infrastructure/MasarHub.Infrastructure.Persistence --startup-project src/MasarHub.API
```

### Run Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Unit/MasarHub.Application.UnitTests
```

---

## Author

**Abdelrahman Zagloul**  
Software Engineer | Back End .NET Developer

<p align="center">
  <a href="mailto:abdelrahman.zagloul.dev@gmail.com" target="_blank"><img height="75" src="https://go-skill-icons.vercel.app/api/icons?i=gmail" alt="Gmail"/></a>&nbsp;&nbsp;
  <a href="https://github.com/Abdelrahman-Zagloul" target="_blank"><img height="75" src="https://go-skill-icons.vercel.app/api/icons?i=github" alt="GitHub"/></a>&nbsp;&nbsp;
  <a href="https://www.linkedin.com/in/abdelrahman-zagloul/" target="_blank"><img height="75" src="https://go-skill-icons.vercel.app/api/icons?i=linkedin" alt="LinkedIn"/></a>&nbsp;&nbsp;
  <a href="https://wa.me/0201285168885" target="_blank"><img height="75" src="https://upload.wikimedia.org/wikipedia/commons/6/6b/WhatsApp.svg" alt="WhatsApp"/></a>&nbsp;&nbsp;
  <a href="https://www.facebook.com/bdalrhmnzghlwl.291648" target="_blank"><img height="75" src="https://go-skill-icons.vercel.app/api/icons?i=facebook" alt="Facebook"/></a>
</p>
