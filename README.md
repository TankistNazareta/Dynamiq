# Dynamiq 🛒

Full-stack e-commerce application built with **.NET 8 (C#)** and **React + TypeScript**.  
The project demonstrates clean architecture, secure payments, and modern UI for an online furniture store.

## 📸 Screenshots

![Main Page](docs/screenshots/main.png)  
![Product Page](docs/screenshots/product.png)  
👉 [Live Demo](https://your-demo-link.com)

## ✨ Key Features

- Product catalog with categories and filters
- Shopping cart with discounts and coupons
- Stripe payments: one-time and subscriptions (webhooks included)
- Secure authentication (JWT & Google OIDC)
- Email confirmation and account management
- Search suggestions and product recommendations

## 🛠 Tech Stack

- **Backend:** .NET 8, ASP.NET Core, EF Core, MediatR, FluentValidation
- **Frontend:** React, TypeScript, SCSS, Bootstrap
- **Database:** SQL Server
- **Other:** Stripe API, GitHub Actions (CI/CD)

## 🏛 Architecture

Built using **Domain-Driven Design + Clean Architecture + CQRS**.  
Core business logic is isolated in domain layer, with clear separation of concerns.  
Integration tests run in isolated SQL containers with Testcontainers.
