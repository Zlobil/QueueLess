# QueueLess

QueueLess is a web application for managing virtual queues, allowing businesses to organize their service flow while clients track their position remotely without standing in physical lines.

This project is developed as part of the ASP.NET Fundamentals course at SoftUni and represents the foundation of a system that will be further extended in the ASP.NET Advanced module.

---

## Current State (ASP.NET Fundamentals)

At this stage, the project implements a complete working system with real-time queue tracking, background processing, and authenticated business management, while maintaining clean architecture and clear separation of responsibilities.

### Implemented Features
- ASP.NET Core MVC architecture
- Entity Framework Core with SQL Server
- Code-first approach with migrations
- Seeded database with demo data
- ASP.NET Identity (Individual Accounts)
- Role-based and ownership-based access control
- Queue ownership bound to authenticated users
- Shared layout with dynamic navigation
- Business and client-facing pages
- ViewModels with validation
- Server-side and client-side validation
- Full CRUD operations for queues (owner-only)
- Real-time waiting status updates (polling)
- Background service for queue entry expiration
- Queue entry lifecycle (Waiting / Serving / Served / Skipped / Expired)
- History management with cleanup options
- Protection of management pages using authorization
- Public access to client pages

> **Note:**  
> The *Active Queues* page currently serves as a demo entry point.  
> In a real-world scenario, clients would usually access queues via direct links or QR codes.

### Demo Login Credentials

You can use the following seeded account to access business features:

**Email:**  
`admin@queueless.com`

**Password:**  
`Admin123!`

---

## Planned Features

The following features are planned for the next development phase (ASP.NET Advanced):

- Web API for external integrations
- Improved security and hardening
- URL access policies and claims-based authorization
- Anti-forgery tokens on all forms
- External login providers (Google, Facebook, etc.)
- SignalR for real-time queue updates
- Client identity accounts and history
- QR code generation for queue access
- Access codes / invitation system
- Mobile-friendly progressive UI

---

## How to Run

1. Clone the repository  
2. Open the solution in Visual Studio  
3. Apply migrations and update the database  
4. Run the project using the default configuration  

No additional configuration is required at this stage.
