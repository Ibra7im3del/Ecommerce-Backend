# E-Commerce Backend API

RESTful Web API for an e-commerce store, built with ASP.NET Core, Entity Framework Core, and SQL Server.

**Live Demo (frontend):** https://ecommerce-frontend-mu-mocha.vercel.app
**Frontend Repo:** https://github.com/Ibra7im3del/Ecommerce-frontend

## Features
- JWT authentication (register / login)
- Role-based authorization (Admin / User)
- Full CRUD for products and categories
- Shopping cart, wishlist, and orders
- SQL Server database managed with EF Core (Code-First)

## Tech Stack
C#, ASP.NET Core Web API, Entity Framework Core, LINQ, SQL Server, JWT

## Main Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/auth/register | Create a new account |
| POST | /api/auth/login | Log in and get a JWT |
| GET | /api/products | List all products |
| GET | /api/wishlist | Get the user's wishlist |

## Run Locally
1. Clone the repo
2. Set your SQL Server connection string and JWT key in `appsettings.Development.json`
3. Run `dotnet ef database update`
4. Run `dotnet run`

## Author
Ibrahim Adel — [LinkedIn](https://linkedin.com/in/ibra7imadel) | [GitHub](https://github.com/Ibra7im3del)
