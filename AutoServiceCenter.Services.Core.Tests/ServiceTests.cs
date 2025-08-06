using AutoServiceCenter.Data;
using AutoServiceCenter.Data.Common.Enums;
using AutoServiceCenter.Data.Models;
using AutoServiceCenter.Services.Core.Contracts;
using AutoServiceCenter.Web.ViewModels.Appointment;
using AutoServiceCenter.Web.ViewModels.Customer;
using AutoServiceCenter.Web.ViewModels.Mechanics;
using AutoServiceCenter.Web.ViewModels.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace AutoServiceCenter.Services.Core.Tests
{
    [TestFixture]
    public class ServiceTests
    {
        private ApplicationDbContext _context;
        private Mock<ILogger<AppointmentService>> _appointmentLoggerMock;
        private Mock<ILogger<CustomerService>> _customerLoggerMock;
        private Mock<ILogger<MechanicService>> _mechanicLoggerMock;
        private Mock<ILogger<ServiceService>> _serviceLoggerMock;
        private Mock<UserManager<IdentityUser>> _userManagerMock;
        private IAppointmentService _appointmentService;
        private ICustomerService _customerService;
        private IMechanicService _mechanicService;
        private IServiceService _serviceService;

        private List<IdentityUser> _users;
        private List<Customer> _customers;
        private List<Vehicle> _vehicles;
        private List<Mechanic> _mechanics;
        private List<Service> _services;
        private List<Appointment> _appointments;

        [SetUp]
        public async Task SetUp()
        {
            // Initialize in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            // Setup sample data
            _users = new List<IdentityUser>
            {
                new IdentityUser { Id = "user1", UserName = "john.doe@auto.com", Email = "john.doe@auto.com" },
                new IdentityUser { Id = "user2", UserName = "jane.smith@auto.com", Email = "jane.smith@auto.com" },
                new IdentityUser { Id = "mechanic1", UserName = "mechanic@auto.com", Email = "mechanic@auto.com" }
            };

            _customers = new List<Customer>
            {
                new Customer { Id = Guid.NewGuid(), UserId = "user1", Name = "John Doe", Address = "123 Main St", IsDeleted = false, User = _users[0] },
                new Customer { Id = Guid.NewGuid(), UserId = "user2", Name = "Jane Smith", Address = "456 Elm St", IsDeleted = false, User = _users[1] },
                new Customer { Id = Guid.NewGuid(), UserId = "user1", Name = "Bob Johnson", Address = "789 Oak St", IsDeleted = true, User = _users[0] }
            };

            _vehicles = new List<Vehicle>
            {
                new Vehicle { Id = Guid.NewGuid(), CustomerId = _customers[0].Id, Customer = _customers[0], Make = "Toyota", Model = "Camry", Year = 2020, LicensePlate = "ABC123", IsDeleted = false },
                new Vehicle { Id = Guid.NewGuid(), CustomerId = _customers[1].Id, Customer = _customers[1], Make = "Honda", Model = "Civic", Year = 2019, LicensePlate = "XYZ789", IsDeleted = false },
                new Vehicle { Id = Guid.NewGuid(), CustomerId = _customers[0].Id, Customer = _customers[0], Make = "Ford", Model = "Focus", Year = 2018, LicensePlate = "DEF456", IsDeleted = true }
            };

            _mechanics = new List<Mechanic>
            {
                new Mechanic { Id = Guid.NewGuid(), UserId = "mechanic1", Name = "Mike Mechanic", Specialization = "Engine Repair", ExperienceYears = 5, IsDeleted = false, User = _users[2] },
                new Mechanic { Id = Guid.NewGuid(), UserId = "user2", Name = "Sarah Tech", Specialization = "Brake Systems", ExperienceYears = 3, IsDeleted = false, User = _users[1] },
                new Mechanic { Id = Guid.NewGuid(), UserId = "user1", Name = "Deleted Mechanic", Specialization = "Tire Service", ExperienceYears = 2, IsDeleted = true, User = _users[0] }
            };

            _services = new List<Service>
            {
                new Service { Id = Guid.NewGuid(), Name = "Oil Change", Description = "Standard oil change", Price = 50.00m, IsDeleted = false },
                new Service { Id = Guid.NewGuid(), Name = "Brake Repair", Description = "Brake pad replacement", Price = 200.00m, IsDeleted = false },
                new Service { Id = Guid.NewGuid(), Name = "Tire Rotation", Description = "Tire rotation service", Price = 30.00m, IsDeleted = true }
            };

            _appointments = new List<Appointment>
            {
                new Appointment { Id = Guid.NewGuid(), CustomerId = _customers[0].Id, Customer = _customers[0], VehicleId = _vehicles[0].Id, Vehicle = _vehicles[0], MechanicId = _mechanics[0].Id, Mechanic = _mechanics[0], ServiceId = _services[0].Id, Service = _services[0], Date = DateTime.UtcNow.AddDays(1), Status = AppointmentStatus.Pending, Notes = "Check engine", IsDeleted = false },
                new Appointment { Id = Guid.NewGuid(), CustomerId = _customers[1].Id, Customer = _customers[1], VehicleId = _vehicles[1].Id, Vehicle = _vehicles[1], MechanicId = _mechanics[1].Id, Mechanic = _mechanics[1], ServiceId = _services[1].Id, Service = _services[1], Date = DateTime.UtcNow.AddDays(2), Status = AppointmentStatus.Confirmed, Notes = "Urgent", IsDeleted = false },
                new Appointment { Id = Guid.NewGuid(), CustomerId = _customers[0].Id, Customer = _customers[0], VehicleId = _vehicles[0].Id, Vehicle = _vehicles[0], MechanicId = _mechanics[0].Id, Mechanic = _mechanics[0], ServiceId = _services[0].Id, Service = _services[0], Date = DateTime.UtcNow.AddDays(3), Status = AppointmentStatus.Completed, Notes = "Done", IsDeleted = false },
                new Appointment { Id = Guid.NewGuid(), CustomerId = _customers[1].Id, Customer = _customers[1], VehicleId = _vehicles[1].Id, Vehicle = _vehicles[1], MechanicId = _mechanics[1].Id, Mechanic = _mechanics[1], ServiceId = _services[1].Id, Service = _services[1], Date = DateTime.UtcNow.AddDays(4), Status = AppointmentStatus.Pending, Notes = "Follow-up", IsDeleted = true },
                new Appointment { Id = Guid.NewGuid(), CustomerId = _customers[0].Id, Customer = _customers[0], VehicleId = _vehicles[0].Id, Vehicle = _vehicles[0], MechanicId = _mechanics[0].Id, Mechanic = _mechanics[0], ServiceId = _services[0].Id, Service = _services[0], Date = DateTime.UtcNow.AddDays(5), Status = AppointmentStatus.Canceled, Notes = "Cancelled", IsDeleted = false }
            };

            await _context.Users.AddRangeAsync(_users);
            await _context.Customers.AddRangeAsync(_customers);
            await _context.Vehicles.AddRangeAsync(_vehicles);
            await _context.Mechanics.AddRangeAsync(_mechanics);
            await _context.Services.AddRangeAsync(_services);
            await _context.Appointments.AddRangeAsync(_appointments);
            await _context.SaveChangesAsync();

            // Initialize mocks
            var store = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            _userManagerMock.Setup(m => m.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((string id) => _users.FirstOrDefault(u => u.Id == id));
            _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((string email) => _users.FirstOrDefault(u => u.Email == email));
            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<IdentityUser>())).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<IdentityUser>())).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            // Initialize services
            _appointmentLoggerMock = new Mock<ILogger<AppointmentService>>();
            _customerLoggerMock = new Mock<ILogger<CustomerService>>();
            _mechanicLoggerMock = new Mock<ILogger<MechanicService>>();
            _serviceLoggerMock = new Mock<ILogger<ServiceService>>();

            _appointmentService = new AppointmentService(_context, _appointmentLoggerMock.Object);
            _customerService = new CustomerService(_context, _userManagerMock.Object, _customerLoggerMock.Object);
            _mechanicService = new MechanicService(_context, _userManagerMock.Object, _mechanicLoggerMock.Object);
            _serviceService = new ServiceService(_context, _serviceLoggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        #region AppointmentService Tests
        
        [Test]
        public async Task GetAppointmentsAsync_FiltersByUserId_ForNonAdmin()
        {
            // Arrange
            int page = 1, pageSize = 10;
            string userId = "user1";
            bool isAdminOrMechanic = false;

            // Act
            var result = await _appointmentService.GetAppointmentsAsync(page, pageSize, userId, isAdminOrMechanic, null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Appointments.Count); // 3 appointments for user1
            Assert.IsTrue(result.Appointments.All(a => a.CustomerEmail == "john.doe@auto.com"));
        }

        [Test]
        public async Task GetAppointmentsAsync_FiltersBySearchTerm()
        {
            // Arrange
            int page = 1, pageSize = 10;
            string userId = "user1";
            bool isAdminOrMechanic = true;
            string searchTerm = "urgent";

            // Act
            var result = await _appointmentService.GetAppointmentsAsync(page, pageSize, userId, isAdminOrMechanic, searchTerm);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Appointments.Count);
            Assert.AreEqual("Urgent", result.Appointments[0].Notes);
        }

        [Test]
        public async Task GetAppointmentsAsync_ReturnsEmpty_WhenNoAppointments()
        {
            // Arrange
            await _context.Appointments.ForEachAsync(a => { a.IsDeleted = true; a.DeletedOn = DateTime.UtcNow; });
            await _context.SaveChangesAsync();

            // Act
            var result = await _appointmentService.GetAppointmentsAsync(1, 10, "user1", true, null);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result.Appointments);
            Assert.AreEqual(0, result.TotalPages);
        }

        [Test]
        public async Task GetAppointmentByIdAsync_ReturnsAppointment_WhenAuthorized()
        {
            // Arrange
            var appointment = _appointments[0];
            string userId = "user1";
            bool isAdminOrMechanic = true;

            // Act
            var result = await _appointmentService.GetAppointmentByIdAsync(appointment.Id, userId, isAdminOrMechanic);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(appointment.Id, result.Id);
            Assert.AreEqual("John Doe", result.Name);
            Assert.AreEqual("ABC123", result.VehicleLicensePlate);
        }

        [Test]
        public async Task GetAppointmentByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Act
            var result = await _appointmentService.GetAppointmentByIdAsync(Guid.NewGuid(), "user1", true);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAppointmentByIdAsync_ReturnsNull_WhenUnauthorized()
        {
            // Arrange
            var appointment = _appointments[1]; // Belongs to user2
            string userId = "user1";
            bool isAdminOrMechanic = false;

            // Act
            var result = await _appointmentService.GetAppointmentByIdAsync(appointment.Id, userId, isAdminOrMechanic);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task CreateAppointmentAsync_CreatesAppointment_WhenAuthorized()
        {
            // Arrange
            var model = new AppointmentCreateViewModel
            {
                CustomerId = _customers[0].Id,
                VehicleId = _vehicles[0].Id,
                ServiceId = _services[0].Id,
                MechanicId = _mechanics[0].Id,
                AppointmentDate = DateTime.UtcNow.AddDays(10),
                Status = AppointmentStatus.Pending,
                Notes = "New appointment"
            };
            string userId = "user1";
            bool isAdminOrMechanic = true;

            // Act
            await _appointmentService.CreateAppointmentAsync(model, userId, isAdminOrMechanic);
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Notes == "New appointment");

            // Assert
            Assert.IsNotNull(appointment);
            Assert.AreEqual(model.CustomerId, appointment.CustomerId);
            Assert.IsFalse(appointment.IsDeleted);
        }

        [Test]
        public void CreateAppointmentAsync_ThrowsUnauthorized_WhenNotAuthorized()
        {
            // Arrange
            var model = new AppointmentCreateViewModel
            {
                CustomerId = _customers[1].Id, // Belongs to user2
                VehicleId = _vehicles[1].Id,
                ServiceId = _services[0].Id,
                MechanicId = _mechanics[0].Id,
                AppointmentDate = DateTime.UtcNow.AddDays(10),
                Status = AppointmentStatus.Pending,
                Notes = "New appointment"
            };
            string userId = "user1";
            bool isAdminOrMechanic = false;

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _appointmentService.CreateAppointmentAsync(model, userId, isAdminOrMechanic));
        }

        [Test]
        public async Task GetAppointmentForEditAsync_ReturnsModel_WhenAuthorized()
        {
            // Arrange
            var appointment = _appointments[0];
            string userId = "user1";
            bool isAdminOrMechanic = true;

            // Act
            var result = await _appointmentService.GetAppointmentForEditAsync(appointment.Id, userId, isAdminOrMechanic);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(appointment.Id, result.Id);
            Assert.AreEqual(appointment.CustomerId, result.CustomerId);
        }

        [Test]
        public async Task UpdateAppointmentAsync_UpdatesAppointment_WhenAuthorized()
        {
            // Arrange
            var appointment = _appointments[0];
            var model = new AppointmentCreateViewModel
            {
                CustomerId = _customers[0].Id,
                VehicleId = _vehicles[0].Id,
                ServiceId = _services[0].Id,
                MechanicId = _mechanics[0].Id,
                AppointmentDate = DateTime.UtcNow.AddDays(20),
                Status = AppointmentStatus.Confirmed,
                Notes = "Updated appointment"
            };
            string userId = "user1";
            bool isAdminOrMechanic = true;

            // Act
            await _appointmentService.UpdateAppointmentAsync(appointment.Id, model, userId, isAdminOrMechanic);
            var updated = await _context.Appointments.FindAsync(appointment.Id);

            // Assert
            Assert.IsNotNull(updated);
            Assert.AreEqual("Updated appointment", updated.Notes);
            Assert.AreEqual(AppointmentStatus.Confirmed, updated.Status);
        }

        [Test]
        public async Task DeleteAppointmentAsync_SoftDeletesAppointment_WhenAuthorized()
        {
            // Arrange
            var appointment = _appointments[0];
            string userId = "user1";
            bool isAdminOrMechanic = true;

            // Act
            await _appointmentService.DeleteAppointmentAsync(appointment.Id, userId, isAdminOrMechanic);
            var deleted = await _context.Appointments.FindAsync(appointment.Id);

            // Assert
            Assert.IsTrue(deleted.IsDeleted);
            Assert.IsNotNull(deleted.DeletedOn);
        }

        [Test]
        public async Task GetCustomersAsync_ReturnsAllCustomers_ForAdmin()
        {
            // Act
            var result = await _appointmentService.GetCustomersAsync(true, "user1");

            // Assert
            Assert.AreEqual(2, result.Count); // 2 non-deleted customers
            Assert.IsTrue(result.Any(c => c.Text == "John Doe"));
        }

        [Test]
        public async Task GetVehiclesAsync_ReturnsUserVehicles_ForNonAdmin()
        {
            // Act
            var result = await _appointmentService.GetVehiclesAsync(false, "user1");

            // Assert
            Assert.AreEqual(1, result.Count); // 1 non-deleted vehicle for user1
            Assert.IsTrue(result.Any(v => v.Text == "ABC123"));
        }

        [Test]
        public async Task GetServicesAsync_ReturnsAllServices()
        {
            // Act
            var result = await _appointmentService.GetServicesAsync();

            // Assert
            Assert.AreEqual(2, result.Count); // 2 non-deleted services
            Assert.IsTrue(result.Any(s => s.Text == "Oil Change"));
        }

        [Test]
        public async Task GetMechanicsAsync_ReturnsAllMechanics()
        {
            // Act
            var result = await _appointmentService.GetMechanicsAsync();

            // Assert
            Assert.AreEqual(2, result.Count); // 2 non-deleted mechanics
            Assert.IsTrue(result.Any(m => m.Text == "Mike Mechanic"));
        }
        #endregion

        #region CustomerService Tests
        [Test]
        public async Task GetCustomersAsync_ReturnsPaginatedCustomers()
        {
            // Arrange
            int page = 1, pageSize = 2;

            // Act
            var result = await _customerService.GetCustomersAsync(page, pageSize, null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Customers.Count);
            Assert.AreEqual(1, result.TotalPages); // 2 non-deleted customers
            Assert.IsTrue(result.Customers.Any(c => c.Name == "John Doe"));
        }

        [Test]
        public async Task GetCustomersAsync_FiltersBySearchTerm()
        {
            // Arrange
            string searchTerm = "john";

            // Act
            var result = await _customerService.GetCustomersAsync(1, 10, searchTerm);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Customers.Count);
            Assert.AreEqual("John Doe", result.Customers[0].Name);
        }

        [Test]
        public async Task GetCustomerByIdAsync_ReturnsCustomer_WhenAuthorized()
        {
            // Arrange
            var customer = _customers[0];
            string userId = "user1";
            bool isAdminOrMechanic = true;

            // Act
            var result = await _customerService.GetCustomerByIdAsync(customer.Id, userId, isAdminOrMechanic);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(customer.Name, result.Name);
            Assert.AreEqual(1, result.VehicleCount);
            Assert.AreEqual(3, result.AppointmentCount);
        }

        [Test]
        public async Task GetCustomerByIdAsync_ReturnsNull_WhenUnauthorized()
        {
            // Arrange
            var customer = _customers[1]; // Belongs to user2
            string userId = "user1";
            bool isAdminOrMechanic = false;

            // Act
            var result = await _customerService.GetCustomerByIdAsync(customer.Id, userId, isAdminOrMechanic);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetCustomerForEditAsync_ReturnsModel_WhenAuthorized()
        {
            // Arrange
            var customer = _customers[0];
            string userId = "user1";
            bool isAdminOrMechanic = true;

            // Act
            var result = await _customerService.GetCustomerForEditAsync(customer.Id, userId, isAdminOrMechanic);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(customer.Name, result.Name);
            Assert.AreEqual("john.doe@auto.com", result.Email);
        }

        [Test]
        public async Task UpdateCustomerAsync_UpdatesCustomer_WhenAuthorized()
        {
            // Arrange
            var customer = _customers[0];
            var model = new CustomerCreateViewModel
            {
                Id = customer.Id,
                Name = "Updated John",
                Email = "updated.john@auto.com",
                Address = "999 New St"
            };
            string userId = "user1";
            bool isAdminOrMechanic = true;

            // Act
            await _customerService.UpdateCustomerAsync(customer.Id, model, userId, isAdminOrMechanic);
            var updated = await _context.Customers.FindAsync(customer.Id);

            // Assert
            Assert.IsNotNull(updated);
            Assert.AreEqual("Updated John", updated.Name);
            Assert.AreEqual("999 New St", updated.Address);
            _userManagerMock.Verify(m => m.UpdateAsync(It.IsAny<IdentityUser>()), Times.Once());
        }

        [Test]
        public void UpdateCustomerAsync_ThrowsUnauthorized_WhenNotAuthorized()
        {
            // Arrange
            var customer = _customers[1]; // Belongs to user2
            var model = new CustomerCreateViewModel
            {
                Id = customer.Id,
                Name = "Updated Jane",
                Email = "updated.jane@auto.com",
                Address = "999 New St"
            };
            string userId = "user1";
            bool isAdminOrMechanic = false;

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _customerService.UpdateCustomerAsync(customer.Id, model, userId, isAdminOrMechanic));
        }

        [Test]
        public async Task DeleteCustomerAsync_SoftDeletesCustomerAndRelated()
        {
            // Arrange
            var customer = _customers[0];
            string userId = "user1";
            bool isAdminOrMechanic = true;

            // Act
            await _customerService.DeleteCustomerAsync(customer.Id, userId, isAdminOrMechanic);
            var deletedCustomer = await _context.Customers.FindAsync(customer.Id);
            var deletedVehicles = await _context.Vehicles.Where(v => v.CustomerId == customer.Id).ToListAsync();
            var deletedAppointments = await _context.Appointments.Where(a => a.CustomerId == customer.Id).ToListAsync();

            // Assert
            Assert.IsTrue(deletedCustomer.IsDeleted);
            Assert.IsTrue(deletedVehicles.All(v => v.IsDeleted));
            Assert.IsTrue(deletedAppointments.All(a => a.IsDeleted));
        }
        #endregion

        #region MechanicService Tests
        [Test]
        public async Task GetMechanicsAsync_ReturnsPaginatedMechanics()
        {
            // Arrange
            int page = 1, pageSize = 2;

            // Act
            var result = await _mechanicService.GetMechanicsAsync(page, pageSize);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Mechanics.Count);
            Assert.AreEqual(1, result.TotalPages); // 2 non-deleted mechanics
            Assert.IsTrue(result.Mechanics.Any(m => m.Name == "Mike Mechanic"));
        }

        [Test]
        public async Task GetMechanicByIdAsync_ReturnsMechanic()
        {
            // Arrange
            var mechanic = _mechanics[0];

            // Act
            var result = await _mechanicService.GetMechanicByIdAsync(mechanic.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(mechanic.Name, result.Name);
            Assert.AreEqual("Engine Repair", result.Specialization);
            Assert.AreEqual(3, result.AppointmentCount);
        }

        [Test]
        public async Task CreateMechanicAsync_CreatesMechanicAndUser()
        {
            // Arrange
            var model = new MechanicCreateViewModel
            {
                Name = "New Mechanic",
                Email = "new.mechanic@auto.com",
                Specialization = "Transmission",
                ExperienceYears = 4
            };

            // Act
            await _mechanicService.CreateMechanicAsync(model);
            var mechanic = await _context.Mechanics.FirstOrDefaultAsync(m => m.Name == "New Mechanic");

            // Assert
            Assert.IsNotNull(mechanic);
            Assert.AreEqual("Transmission", mechanic.Specialization);
            _userManagerMock.Verify(m => m.CreateAsync(It.IsAny<IdentityUser>()), Times.Once());
            _userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<IdentityUser>(), "Mechanic"), Times.Once());
        }

        [Test]
        public async Task GetMechanicForEditAsync_ReturnsModel()
        {
            // Arrange
            var mechanic = _mechanics[0];

            // Act
            var result = await _mechanicService.GetMechanicForEditAsync(mechanic.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(mechanic.Name, result.Name);
            Assert.AreEqual("mechanic@auto.com", result.Email);
        }

        [Test]
        public async Task UpdateMechanicAsync_UpdatesMechanic()
        {
            // Arrange
            var mechanic = _mechanics[0];
            var model = new MechanicCreateViewModel
            {
                Id = mechanic.Id,
                Name = "Updated Mike",
                Email = "updated.mechanic@auto.com",
                Specialization = "Suspension",
                ExperienceYears = 6
            };

            // Act
            await _mechanicService.UpdateMechanicAsync(mechanic.Id, model);
            var updated = await _context.Mechanics.FindAsync(mechanic.Id);

            // Assert
            Assert.IsNotNull(updated);
            Assert.AreEqual("Updated Mike", updated.Name);
            Assert.AreEqual("Suspension", updated.Specialization);
            _userManagerMock.Verify(m => m.UpdateAsync(It.IsAny<IdentityUser>()), Times.Once());
        }
        #endregion

        #region ServiceService Tests
        [Test]
        public async Task GetServicesAsync_ReturnsPaginatedServices()
        {
            // Arrange
            int page = 1, pageSize = 2;

            // Act
            var result = await _serviceService.GetServicesAsync(page, pageSize);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Services.Count);
            Assert.AreEqual(1, result.TotalPages); // 2 non-deleted services
            Assert.IsTrue(result.Services.Any(s => s.Name == "Oil Change"));
        }

        [Test]
        public async Task GetServiceByIdAsync_ReturnsService()
        {
            // Arrange
            var service = _services[0];

            // Act
            var result = await _serviceService.GetServiceByIdAsync(service.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(service.Name, result.Name);
            Assert.AreEqual(3, result.AppointmentCount);
        }

        [Test]
        public async Task CreateServiceAsync_CreatesService()
        {
            // Arrange
            var model = new ServiceCreateViewModel
            {
                Name = "New Service",
                Description = "New service description",
                Price = 75.00m
            };

            // Act
            await _serviceService.CreateServiceAsync(model);
            var service = await _context.Services.FirstOrDefaultAsync(s => s.Name == "New Service");

            // Assert
            Assert.IsNotNull(service);
            Assert.AreEqual(75.00m, service.Price);
            Assert.IsFalse(service.IsDeleted);
        }

        [Test]
        public async Task GetServiceForEditAsync_ReturnsModel()
        {
            // Arrange
            var service = _services[0];

            // Act
            var result = await _serviceService.GetServiceForEditAsync(service.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(service.Name, result.Name);
            Assert.AreEqual(service.Price, result.Price);
        }

        [Test]
        public async Task UpdateServiceAsync_UpdatesService()
        {
            // Arrange
            var service = _services[0];
            var model = new ServiceCreateViewModel
            {
                Name = "Updated Oil Change",
                Description = "Updated description",
                Price = 60.00m
            };

            // Act
            await _serviceService.UpdateServiceAsync(service.Id, model);
            var updated = await _context.Services.FindAsync(service.Id);

            // Assert
            Assert.IsNotNull(updated);
            Assert.AreEqual("Updated Oil Change", updated.Name);
            Assert.AreEqual(60.00m, updated.Price);
        }

        [Test]
        public async Task DeleteServiceAsync_SoftDeletesService()
        {
            // Arrange
            var service = _services[0];

            // Act
            await _serviceService.DeleteServiceAsync(service.Id);
            var deleted = await _context.Services.FindAsync(service.Id);

            // Assert
            Assert.IsTrue(deleted.IsDeleted);
            Assert.IsNotNull(deleted.DeletedOn);
        }
        #endregion
    }
}