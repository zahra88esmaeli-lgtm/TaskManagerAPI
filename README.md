# TaskManagerAPI

REST API for task management, built with ASP.NET Core, Entity Framework Core and JWT authentication.

## Features
- Register and login with JWT
- Change password and delete account
- Task management for each user
- Layered architecture: Controller, Service, Repository

## Tech Stack
C#, ASP.NET Core Web API, Entity Framework Core, SQL Server, JWT

## Getting Started
1. Clone the repository
2. Set `ConnectionStrings:DefaultConnection` and `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` in `appsettings.json`
3. Run `dotnet ef database update`
4. Run the project and open `/scalar/v1`

## Endpoints
| Method | URL | Auth |
|---|---|---|
| POST | /api/user/register | No |
| POST | /api/user/login | No |
| PUT | /api/user/me/ChangePassword | Yes |
| DELETE | /api/user/me | Yes |