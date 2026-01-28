# QueueLess

QueueLess is a web application for managing virtual queues, allowing businesses to organize their service flow while clients track their position remotely without standing in physical lines.

This project is developed as part of the **ASP.NET Fundamentals** course at SoftUni and represents the foundation of a larger system that will be expanded in the ASP.NET Advanced module.

---

## Project Idea

QueueLess addresses a common real-world problem – long waiting lines for services such as auto repair shops, beauty salons, clinics, and public offices.

Clients can:
- join a virtual queue via a link or QR code
- track their position and estimated waiting time
- avoid waiting on-site

Businesses can:
- create and manage service queues
- view waiting clients
- control service flow more efficiently

---

## Current State (ASP.NET Fundamentals)

At this stage, the project includes a working database layer and real data flow, while continuing to focus on clean architecture and correct user experience.

### Implemented Features
- ASP.NET Core MVC architecture
- Entity Framework Core with SQL Server
- Code-first approach with migrations
- Seeded database with demo data
- Shared layout with navigation and footer
- Business and client-facing pages
- ViewModels with validation
- Basic CRUD operations for queues

### Pages Implemented
- **Home** – project overview and concept explanation
- **My Queues** – business view for managing owned queues
- **Create Queue** – functional queue creation form
- **Edit / Delete Queue** – queue management pages
- **Queue Details** – business dashboard view
- **Active Queues** – lists available queues from the database
- **Public Queue** – client-facing queue details page
- **Join Queue** – client joins a queue
- **Waiting Page** – client tracks queue position

> Note:  
> The *Active Queues* page currently serves as a demo entry point.
> In a real-world scenario, clients would usually access queues via direct links or QR codes.

---

## Planned Features
- Authentication and authorization
- Ownership enforcement for queues
- Improved client entry (QR codes / access codes)
- Advanced waiting time calculations

---

## How to Run
How to Run

1. Clone the repository
2. Open the solution in Visual Studio
3. Apply migrations and update the database
4. Run the project using the default configuration

No additional configuration is required at this stage.
