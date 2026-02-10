# QueueLess

QueueLess is a web application for managing virtual queues, allowing businesses to organize their service flow while clients track their position remotely without standing in physical lines.

This project is developed as part of the ASP.NET Fundamentals course at SoftUni and represents the foundation of a system that will be further extended in the ASP.NET Advanced module.

---

## Project Idea

Project Idea

QueueLess addresses a common real-world problem – long waiting lines for services such as auto repair shops, beauty salons, clinics, and public offices.

Clients can:
- join a virtual queue via a public page
- track their position and estimated waiting time in real time
- avoid waiting on-site

Businesses can:
- create and manage service queues
- view waiting clients
- monitor served and expired entries
- control service flow more efficiently

---

## Current State (ASP.NET Fundamentals)

At this stage, the project implements a complete working system with real-time queue tracking and background processing, while maintaining clean architecture and clear separation of responsibilities.

### Implemented Features
- ASP.NET Core MVC architecture
- Entity Framework Core with SQL Server
- Code-first approach with migrations
- Seeded database with demo data
- ASP.NET Identity (Individual Accounts)
- Shared layout with navigation and footer
- Business and client-facing pages
- ViewModels with validation
- Server-side and client-side validation
- Full CRUD operations for queues
- Real-time waiting status updates
- Background service for queue entry expiration
- Queue entry lifecycle (Waiting / Served / Skipped / Expired)
- History management with cleanup options

### Pages Implemented
- **Home** – project overview and concept explanation
- **My Queues** – business view for managing owned queues
- **Create Queue** – functional queue creation form
- **Edit / Delete Queue** – queue management pages
- **Queue Details** – business dashboard with tabs (Waiting clients and History)
- **Active Queues** – lists available queues
- **Public Queue** – client-facing queue details
- **Join Queue** – client joins a queue
- **Waiting Page** – real-time queue tracking with position

> Note:  
> The *Active Queues* page currently serves as a demo entry point.
> In a real-world scenario, clients would usually access queues via direct links or QR codes.

---

## Planned Features
- Full ownership enforcement using authenticated accounts
- Queue association with business accounts
- Client association with identity accounts
- QR code / access code support

---

## How to Run
How to Run

1. Clone the repository
2. Open the solution in Visual Studio
3. Apply migrations and update the database
4. Run the project using the default configuration

No additional configuration is required at this stage.
