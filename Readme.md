# GroupsApp

![.NET](https://img.shields.io/badge/.NET-Core-blue) ![React](https://img.shields.io/badge/React-TypeScript-blue) ![License: MIT](https://img.shields.io/badge/License-MIT-green)

## Description

**GroupsApp** is a simple web application for managing user groups and splitting expenses. It allows users to create groups, assign members, record transactions, and automatically calculate who owes or is owed money.

## Table of Contents

* [Features](#features)
* [Technologies](#technologies)
* [Getting Started](#getting-started)
* [Project Structure](#project-structure)
* [API Documentation](#api-documentation)
* [Testing](#testing)
* [Contributing](#contributing)
* [License](#license)

## Features

1. **Groups**

   * View all groups with current balances (owed or owing).
   * Create new groups with a title.
2. **Group Details**

   * Display group title and member list with individual balances and settle functionality.
   * Add or remove members (removal only when settled).
   * View and create transactions.
3. **Transactions**

   * Select the payer and enter the total amount.
   * Split expenses **Equally**, by **Percentage**, or **Manually** (exact amounts for each member, including payer).

## Technologies

* **Back end**

  * ASP.NET Core Web API
  * Entity Framework Core with In-Memory Database
  * C# (Clean Architecture, Dependency Injection, AutoMapper)
* **Front end**

  * React with TypeScript
  * Material UI
  * React Router
* **Tools**

  * Git, GitHub
  * VS Code
  * Node.js, npm

## Getting Started

1. **Clone the repository**

   ```bash
   git clone https://github.com/kompiuterizmas/GroupsApp.git
   cd GroupsApp
   ```
2. **Run the back end**

   ```bash
   cd backend
   dotnet run
   ```
3. **Run the front end**

   ```bash
   cd ../frontend
   npm install
   npm start
   ```
4. **Open in browser**
   Visit `http://localhost:3000` to use the application.

## Project Structure

```
/backend
  /Controllers      – API controllers
  /Data             – DbContext and migrations
  /Models           – Domain entities
  /DTOs             – Data transfer objects
  /Services         – Business logic
  /Mappings         – AutoMapper profiles
/frontend
  /src
    /components     – Shared UI components
    /pages          – Page views (GroupsList, GroupDetail, NewTransaction)
    /services       – API client (axios or fetch)
    /models         – TypeScript interfaces
    /hooks          – Custom React hooks
    /utils          – Utility functions
```

## API Documentation

Swagger UI is available at:

```
http://localhost:5000/swagger
```

### Main Endpoints

* `GET /api/groups` – Retrieve all groups
* `POST /api/groups` – Create a new group
* `GET /api/groups/{id}` – Get group details
* `POST /api/groups/{id}/members` – Add a member
* `DELETE /api/groups/{id}/members/{memberId}` – Remove a member
* `POST /api/groups/{id}/transactions` – Create a transaction

## Testing

1. Navigate to the backend test project:

   ```bash
   cd backend/GroupsApp.Tests
   dotnet test
   ```
2. Write unit tests for business logic using xUnit.

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/YourFeature`)
3. Commit your changes (`git commit -m "Add some feature"`)
4. Push to the branch (`git push origin feature/YourFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License.
