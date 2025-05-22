# GroupsApp

![.NET](https://img.shields.io/badge/.NET-Core-blue) ![React](https://img.shields.io/badge/React-TypeScript-blue) ![License: MIT](https://img.shields.io/badge/License-MIT-green)

## Description

**GroupsApp** is a web application for managing user groups and splitting expenses. You can create groups, add members, record transactions with different split types (Equal, Percentage, Manual), and view per-user and per-group balances.

## Table of Contents

- [Features](#features)  
- [Technologies](#technologies)  
- [Getting Started](#getting-started)  
- [Project Structure](#project-structure)  
- [Environment Variables](#environment-variables)  
- [API Documentation](#api-documentation)  
- [Testing](#testing)  
- [Contributing](#contributing)  
- [License](#license)  

## Features

1. **Groups**  
   - View all groups with current balance  
   - Create a new group by title  

2. **Group Details**  
   - List members with individual balances and “settle” actions  
   - Add or remove members (removal only if settled)  
   - View and create transactions  

3. **Transactions**  
   - Enter payer, amount, and choose split mode  
   - Split **Equally**, by **Percentage**, or **Manual** amounts  
   - Transaction cards show description, type, amount, group, payer, date, and (on user page) your per-transaction balance  

4. **Demo Data Seeding**  
   - Click **Seed Demo Data** in the Navbar to populate 50 users, 10 groups, random memberships and transactions  

## Technologies

- **Back end**  
  - ASP.NET Core Web API (C#)  
  - EF Core In-Memory database  
  - AutoMapper, Dependency Injection  

- **Front end**  
  - React with TypeScript  
  - Material UI  
  - React Router  

- **Tools**  
  - Git, GitHub  
  - VS Code  
  - Node.js, npm  

## Getting Started

1. **Clone the repository**  
   ```bash
   git clone https://github.com/kompiuterizmas/GroupsApp.git
   cd GroupsApp

2. **Back end**

   cd backend/GroupsApp.Api
   dotnet restore
   dotnet run

3. **Front end**

   cd ../../frontend
   npm install
   npm start

   Create a .env file in /frontend (see Environment Variables).
   The React app runs at http://localhost:3000.

4. **Use the App**

   Open your browser at http://localhost:3000
   Click Seed Demo Data in the Navbar to populate sample users/groups/transactions.

## Project Structure

/backend/GroupsApp.Api
  /Controllers      – API controllers (Groups, Users, Seed)
/  Data             – AppDbContext configuration
/  Models           – EF entities (User, Group, Transaction, GroupMember)
/  DTOs             – API data shapes
/  Services         – business logic (GroupsService, SeedService)
/  Mappings         – AutoMapper profiles
/frontend
  /src
    /components     – shared React components (NavBar, ConnectionChecker, TransactionCard)
    /pages          – route views (GroupsListPage, GroupDetailPage, NewTransactionPage, MembersPage, UserPage)
    /services       – API client functions (api.ts)
    /models         – TypeScript interfaces (if any)
    /App.tsx, index.tsx
.env.example        – environment variable template (copy to `.env`)

## Environment Variables

Copy .env.example to .env in the /frontend folder and set:
REACT_APP_API_BASE_URL=http://localhost:5272/api
Then restart the React server.

## API Documentation

Swagger UI is available once the back end is running:
http://localhost:5272/swagger

## Main Endpoints

Groups

GET /api/groups
POST /api/groups
GET /api/groups/{id}
POST /api/groups/{id}/members
DELETE /api/groups/{id}/members/{userId}
POST /api/groups/{id}/transactions

Users

GET /api/users
GET /api/users/{userId}
GET /api/users/{userId}/groups
GET /api/users/{userId}/transactions

Seeding

POST /api/seed (not shown in Swagger)

## Testing

Back end unit tests
cd backend/GroupsApp.Tests
dotnet test

We use xUnit to verify GroupsService logic.

## Contributing

1. Fork the repo
2. Create a feature branch (git checkout -b feature/YourFeature)
3. Commit changes (git commit -m "Add feature")
4. Push to your branch (git push origin feature/YourFeature)
5. Open a Pull Request

## License

This project is licensed under the MIT License.