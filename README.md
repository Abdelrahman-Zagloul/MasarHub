# MasarHub API

[![.NET 10](https://img.shields.io/badge/.NET-10.0-blueviolet?logo=dotnet)](https://learn.microsoft.com/en-us/aspnet/core/)
[![Swagger](https://img.shields.io/badge/Swagger-API%20Docs-green?logo=swagger)](https://masarhub.runasp.net/swagger/index.html)
[![Scalar](https://img.shields.io/badge/Scalar-API%20Reference-black)](https://masarhub.runasp.net/scalar)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-Abdelrahman%20Zagloul-blue?logo=linkedin)](https://www.linkedin.com/in/abdelrahman-zagloul/)

A work-in-progress **ASP.NET Core Web API** for an online learning platform built using
**Clean Architecture**, **CQRS with MediatR**, and a modular Ritch domain model.

MasarHub is designed to support students, instructors, courses, exams, certificates,
orders, payments, notifications, localization, and secure account management. The current
implementation focuses mainly on the authentication foundation, infrastructure setup,
domain modeling, persistence, background processing, and real-time notification support.

---

## Quick Overview

- **Architecture:** Clean Architecture + CQRS (MediatR)
- **Authentication:** JWT + Refresh Tokens (HttpOnly cookies)
- **Identity:** ASP.NET Core Identity with role-based users
- **Two-Factor Authentication:** Email, SMS, authenticator app, and recovery codes
- **External Login:** Google, GitHub, LinkedIn, and Facebook provider support
- **Persistence:** EF Core, SQL Server, Repository, Unit of Work, and Dapper
- **Caching:** Redis cache service
- **Background Jobs:** Hangfire with SQL Server storage
- **Realtime:** SignalR notification hub
- **Localization:** JSON-based localization for English and Arabic
- **API Documentation:** Swagger and Scalar in development
- **Logging:** Serilog with Console, File, and Seq sinks

---

## Table of Contents

- [Features](#features)
  - [Authentication & Authorization](#authentication--authorization)
  - [Two-Factor Authentication](#two-factor-authentication)
  - [External Authentication](#external-authentication)
  - [Learning Platform Modules](#learning-platform-modules)
  - [Notifications & Realtime](#notifications--realtime)
  - [Localization](#localization)
  - [Caching & Background Jobs](#caching--background-jobs)
- [Architecture](#architecture)
- [API & Platform Features](#api--platform-features)
- [Tech Stack](#tech-stack)
- [API Endpoints](#api-endpoints)
- [Setup & Run](#setup--run)
  - [Run Locally](#run-locally)
  - [Database Migrations](#database-migrations)
  - [Run Test Cases](#run-test-cases)
- [Project Status](#project-status)
- [Author](#author)

---

## Features

### Authentication & Authorization

- Student registration
- Instructor registration
- User login and logout
- JWT access token generation
- Refresh token rotation using secure HttpOnly cookies
- Refresh token revocation
- Email confirmation and resend confirmation support
- Password management:
  - Change password
  - Forget password
  - Reset password
  - Verify password
- Role-based users:
  - Admin
  - Instructor
  - Student

### Two-Factor Authentication

- Enable and disable two-factor authentication
- Send two-factor verification code
- Verify two-factor code
- Authenticator app setup
- Authenticator code verification
- Generate recovery codes
- Verify recovery codes
- Email templates for two-factor events

### External Authentication

- Google login provider support
- GitHub login provider support
- LinkedIn login provider support
- Facebook login provider support
- External auth settings are isolated in configuration

### Learning Platform Modules

The domain layer already contains the main learning platform modules:

- Courses
- Course modules
- Lessons:
  - Video lessons
  - Article lessons
  - Resource lessons
- Lesson attachments
- Course progress
- Lesson progress
- Course reviews
- Course announcements
- Enrollments
- Categories
- Exams
- Questions and options
- Exam attempts and answers
- Certificates
- Certificate downloads
- Orders
- Payments
- Coupons and coupon usage
- Instructor profiles and social links

### Notifications & Realtime

- Notification domain model
- Notification priorities and types
- Background notification creation job
- SignalR notification hub
- Realtime notification service
- User group mapping for admins, instructors, and students

### Localization

- JSON-based localization provider
- Supported cultures:
  - English (`en`)
  - Arabic (`ar`)
- Culture middleware
- Localized API messages
- Cached localization resources

### Caching & Background Jobs

- Redis cache service
- Fail-isolated infrastructure service abstractions
- Hangfire background job processing
- Hangfire dashboard registration
- Background email and notification workflows

---

## Architecture

This project follows **Clean Architecture** principles with a modular structure.

```text
MasarHub.slnx
|-- src
|   |-- MasarHub.API
|   |   |-- Controllers
|   |   |-- Extensions
|   |   |-- Filters
|   |   |-- Middlewares
|   |   |-- Resources
|   |   |-- SignalR
|   |   `-- wwwroot/EmailTemplates
|   |
|   |-- Core
|   |   |-- MasarHub.Application
|   |   |   |-- Abstractions
|   |   |   |-- Common
|   |   |   |-- Features
|   |   |   |-- Settings
|   |   |   `-- Extensions
|   |   |
|   |   `-- MasarHub.Domain
|   |       |-- Common
|   |       `-- Modules
|   |
|   `-- Infrastructure
|       |-- MasarHub.Infrastructure
|       |   |-- ExternalServices
|       |   |-- Identity
|       |   |-- Jobs
|       |   |-- Localization
|       |   `-- Services
|       |
|       `-- MasarHub.Infrastructure.Persistence
|           |-- Configurations
|           |-- Contexts
|           |-- Dapper
|           |-- Identity
|           |-- Migrations
|           |-- Repositories
|           `-- SeedData
|
`-- tests
```

### Architecture Highlights

- Clean separation between API, Application, Domain, Infrastructure, and Persistence
- CQRS pattern using MediatR
- FluentValidation pipeline behavior
- Domain entities isolated from web/API concerns
- Infrastructure concerns hidden behind application abstractions
- EF Core configurations separated by module
- Repository and Unit of Work abstractions
- Dapper connection factory for query-focused data access
- Result pattern for predictable success/error handling
- Dependency injection split by layer

---

## API & Platform Features

- API versioning using the `api-version` header
- Swagger UI with JWT bearer support
- Scalar API reference in development
- Global problem details setup
- Sliding window rate limiting policies:
  - Global
  - Sensitive
  - OTP
  - Strict
- CORS configuration for development and production
- Forwarded headers support for reverse proxies
- Static files support for email templates
- Serilog request logging
- JSON enum serialization as strings

---

## Tech Stack

### Core & Architecture

- ASP.NET Core (.NET 10)
- Clean Architecture
- CQRS Pattern
- MediatR
- FluentValidation
- Result Pattern

### Data & Persistence

- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Dapper
- Generic Repository
- Unit of Work
- EF Core Migrations

### Authentication & Security

- JWT Authentication
- Refresh Tokens
- HttpOnly Secure Cookies
- Role-Based Authorization
- Two-Factor Authentication
- External OAuth login providers

### Caching & Background Processing

- Redis
- StackExchange.Redis
- Hangfire
- Hangfire SQL Server storage

### API & Documentation

- Swagger / OpenAPI
- Scalar API Reference
- API Versioning
- Rate Limiting
- CORS
- Problem Details

### Realtime & Notifications

- SignalR
- Notification Hub
- Background notification jobs

### Email & SMS

- SMTP email service
- HTML email templates
- Twilio SMS service

### Logging & Diagnostics

- Serilog
- Console sink
- File sink
- Seq sink

---

## API Endpoints

**Local Base URL:**

```text
https://localhost:7232
```

**Swagger UI:**

```text
https://localhost:7232/swagger
```

**Scalar API Reference:**

```text
https://localhost:7232/scalar
```

### Authentication

| Method | Endpoint                        | Description                  |
| ------ | ------------------------------- | ---------------------------- |
| POST   | `/api/auth/student/register`    | Register student account     |
| POST   | `/api/auth/instructor/register` | Register instructor account  |
| POST   | `/api/auth/login`               | User login                   |
| POST   | `/api/auth/external/login`      | Login using external provider |
| POST   | `/api/auth/logout`              | User logout                  |

### Email

| Method | Endpoint                              | Description               |
| ------ | ------------------------------------- | ------------------------- |
| POST   | `/api/auth/email/confirm`             | Confirm email address     |
| POST   | `/api/auth/email/resend-confirmation` | Resend confirmation email |

### Tokens

| Method | Endpoint                  | Description          |
| ------ | ------------------------- | -------------------- |
| POST   | `/api/auth/token/refresh` | Refresh access token |
| POST   | `/api/auth/token/revoke`  | Revoke refresh token |

### Password

| Method | Endpoint                    | Description            |
| ------ | --------------------------- | ---------------------- |
| POST   | `/api/auth/password/change` | Change password        |
| POST   | `/api/auth/password/forget` | Request password reset |
| POST   | `/api/auth/password/reset`  | Reset password         |
| POST   | `/api/auth/password/verify` | Verify password        |

### Two-Factor Authentication

| Method | Endpoint                                  | Description                    |
| ------ | ----------------------------------------- | ------------------------------ |
| POST   | `/api/auth/2fa/enable`                    | Enable two-factor auth         |
| POST   | `/api/auth/2fa/disable`                   | Disable two-factor auth        |
| POST   | `/api/auth/2fa/send-code`                 | Send two-factor code           |
| POST   | `/api/auth/2fa/verify`                    | Verify two-factor code         |
| POST   | `/api/auth/2fa/authenticator/setup`       | Setup authenticator app        |
| POST   | `/api/auth/2fa/authenticator/verify`      | Verify authenticator code      |
| POST   | `/api/auth/2fa/recovery-codes/generate`   | Generate recovery codes        |
| POST   | `/api/auth/2fa/recovery-codes/verify`     | Verify recovery code           |

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

```text
https://localhost:7232
http://localhost:5266
```

---

## Author

**Abdelrahman Zagloul**  
Software Engineer | Back End .NET Developer

<p align="center">
  <a href="mailto:abdelrahman.zagloul.dev@gmail.com">
    <img height="75" src="https://go-skill-icons.vercel.app/api/icons?i=gmail" alt="Gmail"/>
  </a>
  &nbsp;&nbsp;
  <a href="https://github.com/Abdelrahman-Zagloul">
    <img height="75" src="https://go-skill-icons.vercel.app/api/icons?i=github" alt="GitHub"/>
  </a>
  &nbsp;&nbsp;
  <a href="https://www.linkedin.com/in/abdelrahman-zagloul/">
    <img height="75" src="https://go-skill-icons.vercel.app/api/icons?i=linkedin" alt="LinkedIn"/>
  </a>
  &nbsp;&nbsp;
  <a href="https://wa.me/0201285168885">
    <img height="75" src="https://upload.wikimedia.org/wikipedia/commons/6/6b/WhatsApp.svg" alt="WhatsApp"/>
  </a>
  &nbsp;&nbsp;
  <a href="https://www.facebook.com/bdalrhmnzghlwl.291648">
    <img height="75" src="https://go-skill-icons.vercel.app/api/icons?i=facebook" alt="Facebook"/>
  </a>
</p>
