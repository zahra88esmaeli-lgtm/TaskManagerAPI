# TaskManagerAPI

REST API for task management, built with ASP.NET Core, Entity Framework Core and JWT authentication.
Each user can only see and change their own tasks.

## Features
- Register and login with JWT
- Change password and delete account
- Full CRUD for tasks (create, read, update, delete)
- Per-user data isolation: every task query is filtered by the logged-in user's id
- Layered architecture: Controller, Service, Repository
- DTOs for input and output, so database models are never exposed directly

## Tech Stack
C#, ASP.NET Core Web API, Entity Framework Core, SQL Server, JWT

## Getting Started
1. Clone the repository
2. Set `ConnectionStrings:DefaultConnection` and `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` in `appsettings.json` (do not commit real secrets)
3. Run `dotnet ef database update`
4. Run the project and open `/scalar/v1`

## Endpoints

### Users
| Method | URL | Auth |
|---|---|---|
| POST | /api/user/register | No |
| POST | /api/user/login | No |
| PUT | /api/user/me | Yes |
| DELETE | /api/user/me | Yes |

### Tasks
All task endpoints require the header `Authorization: Bearer <token>`.

| Method | URL | Description | Auth |
|---|---|---|---|
| GET | /api/TaskItem | Get all tasks of the logged-in user | Yes |
| GET | /api/TaskItem/{id} | Get one task by id | Yes |
| POST | /api/TaskItem | Create a task | Yes |
| PUT | /api/TaskItem/{id} | Update a task | Yes |
| DELETE | /api/TaskItem/{id} | Delete a task | Yes |

### Example: create a task

```http
POST /api/TaskItem
Authorization: Bearer <token>
Content-Type: application/json

{
  "title": "Write README",
  "description": "Document the project",
  "isDone": false
}
```

Response: `201 Created`

```json
{
  "id": 1,
  "title": "Write README",
  "description": "Document the project",
  "isDone": false
}
```

## How user isolation works
The user id is stored in the JWT token. The controller reads it from the token claims and passes it to the repository.
Queries always filter by `UserId`, so a user who requests another user's task gets `404 Not Found`.


## How to run

**Requirements:** .NET 10 SDK, SQL Server LocalDB (installed with Visual Studio)

```
git clone https://github.com/zahra88esmaeli-lgtm/TaskManagerAPI.git
cd TaskManagerAPI/TaskManagerAPI
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "any-random-text-at-least-32-characters"
dotnet tool install --global dotnet-ef
dotnet ef database update
dotnet run
```

- `Jwt:Key` must be at least **32 characters**, otherwise Login returns a 500 error.
- The API usually runs at `http://localhost:5280` (check the `dotnet run` output).
- Web front end for this API: [TaskManagerWeb](https://github.com/zahra88esmaeli-lgtm/TaskManagerWeb)

## Running the tests

## Running the tests

Run this command from the repository root (the folder that contains the `.slnx` file):

```
dotnet test
```
