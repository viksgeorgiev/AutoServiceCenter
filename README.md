
🚗 AutoServiceCenter Web Application
AutoServiceCenter is a web-based auto repair shop management system developed as part of a coding examination. 
The application is built using ASP.NET Core 8.0 with Entity Framework Core, following the MVC pattern, 
and includes user authentication and role management with ASP.NET Identity.


🧩 Features

The system provides full functionality for customers, mechanics, and administrators:

👤 User Authentication – Register and log in with role-based access: Customer, Mechanic, or Administrator.
📅 Appointment Booking – Customers can book appointments for vehicle services.
🔧 Mechanic & Service Management – Admins can manage mechanics, their specializations, and available services.
🚘 Vehicle Management – Users can register and manage vehicles.
🔍 Search & Pagination – Search across appointments, vehicles, customers, and more with paginated results.
🛡️ Security – Includes CSRF protection, input validation, and proper error handling.


⚙️ Technologies Used

Framework: ASP.NET Core 8.0
Language: C#
Database: SQL Server / EF Core
UI: Razor Views, Bootstrap 5
Authentication: ASP.NET Identity
Testing: NUnit, Moq
IDE: Visual Studio 2022 / JetBrains Rider

🧪 Test Accounts

Role	    Email	            Password
Admin	    admin@auto.com	    Password123!
Mechanic	mechanic@auto.com	Password123!
Customer	john.doe@auto.com	Password123!


🏁 How to Run the Project

🧰 Requirements

.NET 8.0 SDK
Visual Studio 2022 or JetBrains Rider
SQL Server (e.g., SQL Server Express)
Git

🚀 Steps

Clone the repository:

git clone [your-repo-link]
Open the project in Visual Studio or Rider.

Configure the connection string in appsettings.json:

"ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AutoServiceCenter;Trusted_Connection=True;"
}

Apply migrations to create the database:

dotnet ef migrations add InitialCreate
dotnet ef database update


Run the application:

dotnet run
or press F5 in your IDE.

Access the app at:
https://localhost:5001

📁 Project Structure
File / Folder	                                Description
AutoServiceCenter.Data/ApplicationDbContext.cs	Configures the EF Core database context and entity sets.
AutoServiceCenter.Data.Models/	                Contains domain models such as Appointment.cs, Customer.cs, Service.cs, etc.
AutoServiceCenter.Services.Core/	            Business logic layer with services like AppointmentService.cs.
AutoServiceCenter.Web/Areas/Admin/Controllers/	Controllers for Admin functionality (e.g., managing services and customers).
AutoServiceCenter.Web/Views/	                Razor views for UI rendering (e.g., login, appointment booking).
AutoServiceCenter.Services.Core.Tests/	        Unit tests using NUnit and Moq.


📊 Features Implemented for Coding Exam

✅ ASP.NET Core MVC app with 5+ models: Appointment, Customer, Mechanic, Service, Vehicle.
✅ 5+ controllers including Admin and Mechanic areas.
✅ Implementation of Areas (e.g., Admin).
✅ Pagination and search functionality across multiple lists.
✅ ASP.NET Identity integration with role-based access control.
✅ Unit tests covering key services and logic.
✅ Form and model validation with anti-forgery protection.
✅ Seeded test data included for demonstration.


🔐 Security Measures

🧼 HTML encoding to prevent XSS.
🛡️ CSRF protection using [ValidateAntiForgeryToken] and @Html.AntiForgeryToken().
🔒 Role-based access and policy authorization.
❗ Proper error handling with status code pages and error views.


🌱 Seeding Data

Seed data is located in the ServiceTests.cs file (method: SeedData). Includes:
👥 12 Customers
🚗 12 Vehicles
👨‍🔧 12 Mechanics with various specialties
🛠️ 12 Services (e.g., Oil Change, Brake Inspection)
📅 12 Appointments


🧠 What Was Learned

Configuring and customizing ASP.NET Identity for user roles.
Writing unit tests using NUnit and Moq.
Implementing search, pagination, and sorting logic.
Structuring a clean architecture using dependency injection and service layers.
Ensuring secure and user-friendly form handling with proper validation.

🐞 Known Limitations

💄 UI is functional but minimal; could be improved for better UX.
🗓️ Some edge cases (e.g., invalid dates) may need improved validation messages.
🔁 Currently no AJAX/Web API — fully server-rendered MVC.
🧪 Seed data is bundled in tests; could be separated for cleaner deployment.
