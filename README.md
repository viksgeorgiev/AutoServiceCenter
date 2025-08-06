AutoServiceCenter Web Application
AutoServiceCenter is a web-based auto repair shop management system developed as part of a coding examination. The application is built using ASP.NET Core 8.0 with Entity Framework Core, following the MVC architectural pattern. It includes user authentication and role-based access control using ASP.NET Identity.

Features
The system provides full functionality for customers, mechanics, and administrators:

User Authentication – Users can register and log in with role-based access: Customer, Mechanic, or Administrator.

Appointment Booking – Customers can book appointments for vehicle services such as oil changes or brake inspections.

Mechanic and Service Management – Administrators can manage mechanics, their areas of specialization, and the services offered.

Vehicle Management – Customers can register and manage their vehicles.

Search and Pagination – Allows users to search and filter across appointments, customers, vehicles, and services with pagination support.

Security – Includes CSRF protection, input validation, and structured error handling.

Technologies Used
Framework: ASP.NET Core 8.0

Language: C#

Database: SQL Server / Entity Framework Core

UI: Razor Views with Bootstrap 5

Authentication: ASP.NET Identity

Testing: NUnit and Moq

IDE: Visual Studio 2022 / JetBrains Rider

Test Accounts
Role	Email	Password
Admin	admin@auto.com	Password123!
Mechanic	mechanic@auto.com	Password123!
Customer	john.doe@auto.com	Password123!

How to Run the Project
Requirements
.NET 8.0 SDK

Visual Studio 2022 or JetBrains Rider

SQL Server (e.g., SQL Server Express)

Git

Steps
Clone the repository:

git clone [your-repo-link]
Open the project in Visual Studio or JetBrains Rider.

Update the connection string in appsettings.json:

"ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AutoServiceCenter;Trusted_Connection=True;"
}
Apply migrations and create the database:

dotnet ef migrations add InitialCreate
dotnet ef database update
Build and run the application:

dotnet run
Or press F5 in your IDE.

Open your browser and navigate to https://localhost:5001.

Project Structure
Path	Description
AutoServiceCenter.Data/ApplicationDbContext.cs	Configures the EF Core database context and entity sets.
AutoServiceCenter.Data.Models/	Contains domain models (e.g., Appointment, Customer, Service).
AutoServiceCenter.Services.Core/	Business logic services such as AppointmentService.cs.
AutoServiceCenter.Web/Areas/Admin/Controllers/	Controllers for administrative features.
AutoServiceCenter.Web/Views/	Razor views for the UI, including login and management pages.
AutoServiceCenter.Services.Core.Tests/	Unit tests using NUnit and Moq.

Features Implemented for Coding Exam
ASP.NET Core MVC web application with more than 5 models: Appointment, Customer, Mechanic, Service, Vehicle.

Implementation of 5+ controllers, including administrative and mechanic-specific logic.

Use of MVC Areas (e.g., an Admin area).

Full support for pagination and search.

ASP.NET Identity for user login and role management.

Unit testing of core services using NUnit and Moq.

Data validation at both UI and model levels, including anti-forgery tokens.

Test data seeded for demonstration purposes.

Security Measures
HTML encoding to prevent cross-site scripting (XSS).

CSRF protection using [ValidateAntiForgeryToken] and @Html.AntiForgeryToken().

Role-based authorization using ASP.NET Identity policies.

Error handling implemented using status code pages and error views.

Seeding Data
Seed data is located in the ServiceTests.cs file, within the SeedData method. It populates the database with:

12 Customers

12 Vehicles

12 Mechanics with various specializations

12 Services (e.g., Oil Change, Brake Inspection)

12 Appointments linking the above data

What Was Learned
Configuring and customizing ASP.NET Identity for user roles.

Writing effective unit tests using NUnit and Moq.

Implementing search, pagination, and sorting logic in a scalable way.

Structuring a clean and maintainable architecture using dependency injection and service layers.

Ensuring security and form validation across the application.

Known Limitations
The user interface is functional but minimal; it could be enhanced for better user experience.

Certain edge cases (such as invalid appointment dates) may require improved validation messages.

The app currently lacks AJAX functionality and a public Web API; it is entirely server-rendered using MVC.

Seed data is bundled within test files and could be moved to a standalone seeding script for production readiness.