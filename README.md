# QueueLess

QueueLess is a web application for managing virtual queues, allowing businesses to organize their service flow while clients track their position remotely without standing in physical lines.

This project is developed as part of the **ASP.NET Fundamentals** course at SoftUni and represents the foundation of a larger system that will be expanded in the ASP.NET Advanced module.

---

## Project Idea

QueueLess focuses on solving a common real-world problem – long waiting lines for services such as auto repair, salons, clinics, and public offices.

Instead of waiting on-site, clients can:
- join a virtual queue via a link or QR code
- track their position and estimated waiting time
- arrive only when their turn is approaching

Businesses can:
- create and manage queues
- advance or skip clients
- control service flow more efficiently

---

## Current State (ASP.NET Fundamentals)

At this stage, the project focuses on **architecture, navigation, and user flow**, without database persistence yet.

### Implemented Features
- ASP.NET Core MVC architecture
- Shared layout with navigation and footer
- Public and business-oriented pages
- QueueController with all required actions
- Fully navigable UI skeleton

### Pages Implemented
- **Home** – project overview and concept explanation
- **My Queues** – business view for managing owned queues
- **Create Queue** – queue creation page (UI only)
- **Queue Details** – business dashboard view
- **Active Queues** – demo page listing available queues
- **Public Queue** – client-facing queue page
- **Join Queue** – client joins a queue
- **Waiting Page** – client tracks queue position

> Note:  
> The *Active Queues* page is currently used as a demo and fallback entry point.  
> In a real-world scenario, clients would usually access queues via direct links or QR codes.

---

## Planned Features
- Entity Framework Core
- SQL Server database
- Authentication and authorization
- Real-time queue updates (ASP.NET Advanced)
- Improved client entry (queue codes / QR scanning)

---

## How to Run
1. Clone the repository
2. Open the solution in Visual Studio
3. Run the project using the default configuration

No additional configuration is required at this stage.
