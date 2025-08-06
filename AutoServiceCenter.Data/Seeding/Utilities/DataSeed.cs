using AutoServiceCenter.Data.Common.Enums;
using AutoServiceCenter.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace AutoServiceCenter.Data.Seeding.Utilities
{
    public static class DataSeed
    {
        public static async Task SeedData(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed Roles
            if (!await roleManager.RoleExistsAsync("Administrator"))
            {
                await roleManager.CreateAsync(new IdentityRole("Administrator"));
            }
            if (!await roleManager.RoleExistsAsync("Mechanic"))
            {
                await roleManager.CreateAsync(new IdentityRole("Mechanic"));
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Seed Admin User (1 IdentityUser)
            IdentityUser adminUser = new IdentityUser { UserName = "admin@auto.com", Email = "admin@auto.com", EmailConfirmed = true };
            if (await userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                IdentityResult result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                }
            }

            // Seed Regular Users (11 to reach 12 IdentityUsers with 1 mechanic)
            string[] userNames = { "john.doe", "jane.smith", "bob.jones", "alice.brown", "charlie.davis",
                                   "emma.wilson", "david.moore", "sarah.taylor", "mike.jackson", "lisa.white",
                                   "tom.harris" };
            for (int i = 0; i < userNames.Length && context.Users.Count(u => u.UserName.Contains("@auto.com")) < 11; i++)
            {
                string email = $"{userNames[i]}@auto.com";
                IdentityUser user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    IdentityResult result = await userManager.CreateAsync(user, "User123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "User");
                    }
                }
            }

            // Seed Mechanic User (1 already seeded, add 11 more for 12 total IdentityUsers)
            string[] mechanicNames = { "mike.smith", "tom.wilson", "anna.jackson", "steve.brown", "lucy.davis",
                                      "peter.moore", "kate.taylor", "jim.white", "sophia.jones", "mark.thomas",
                                      "emma.green", "oliver.martin" };
            for (int i = 0; i < mechanicNames.Length && context.Users.Count(u => u.UserName.Contains("@auto.com")) < 12; i++)
            {
                string email = $"{mechanicNames[i]}@auto.com";
                IdentityUser mechanicUser = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    IdentityResult result = await userManager.CreateAsync(mechanicUser, "Mechanic123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(mechanicUser, "Mechanic");
                    }
                }
            }

            await context.SaveChangesAsync();

            // Seed Customers (12 total)
            string[] customerNames = { "John Doe", "Jane Smith", "Bob Jones", "Alice Brown", "Charlie Davis",
                                      "Emma Wilson", "David Moore", "Sarah Taylor", "Mike Jackson", "Lisa White",
                                      "Tom Harris", "Laura Clark" };
            string[] addresses = { "Liulin 10 blok 122 vhod A ap 4", "123 Main St", "456 Oak Ave", "789 Pine Rd",
                                  "101 Maple Dr", "202 Birch Ln", "303 Cedar Ct", "404 Elm St", "505 Walnut Ave",
                                  "606 Spruce Rd", "707 Chestnut Dr", "808 Sycamore Ln" };
            List<IdentityUser> users = context.Users.Where(u => u.Email != adminUser.Email && !u.Email.Contains("mechanic")).Take(12).ToList();
            for (int i = 0; i < customerNames.Length && context.Customers.Count() < 12; i++)
            {
                IdentityUser? user = i < users.Count ? users[i] : null;
                if (user != null && !context.Customers.Any(c => c.UserId == user.Id))
                {
                    context.Customers.Add(new Customer
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        Name = customerNames[i],
                        Address = addresses[i],
                        IsDeleted = false
                    });
                }
            }
            await context.SaveChangesAsync();

            // Seed Mechanics (12 total)
            string[] specializations = { "General Repair", "Engine Diagnostics", "Brake Systems", "Transmission",
                                        "Suspension", "Electrical Systems", "Tire Services", "AC Repair",
                                        "Oil Systems", "Exhaust Systems", "Bodywork", "Diagnostics" };
            List<IdentityUser> mechanicUsers = context.Users.Where(u => u.Email.Contains("mechanic") || u.Email.Contains("smith") ||
                                                                        u.Email.Contains("wilson") || u.Email.Contains("jackson") ||
                                                                        u.Email.Contains("brown") || u.Email.Contains("davis") ||
                                                                        u.Email.Contains("moore") || u.Email.Contains("taylor") ||
                                                                        u.Email.Contains("white") || u.Email.Contains("jones") ||
                                                                        u.Email.Contains("thomas") || u.Email.Contains("green") ||
                                                                        u.Email.Contains("martin")).Take(12).ToList();
            for (int i = 0; i < specializations.Length && context.Mechanics.Count() < 12; i++)
            {
                IdentityUser? mechanicUser = i < mechanicUsers.Count ? mechanicUsers[i] : null;
                if (mechanicUser != null && !context.Mechanics.Any(m => m.UserId == mechanicUser.Id))
                {
                    context.Mechanics.Add(new Mechanic
                    {
                        Id = Guid.NewGuid(),
                        UserId = mechanicUser.Id,
                        Name = mechanicNames[i],
                        Specialization = specializations[i],
                        ExperienceYears = 5 + i,
                        IsDeleted = false
                    });
                }
            }
            await context.SaveChangesAsync();

            // Seed Vehicles (12 total)
            string[] makes = { "Toyota", "Honda", "Ford", "BMW", "Mercedes", "Volkswagen", "Chevrolet", "Nissan",
                               "Hyundai", "Audi", "Kia", "Subaru" };
            string[] models = { "Camry", "Civic", "F-150", "X5", "C-Class", "Golf", "Silverado", "Altima",
                               "Tucson", "A4", "Sportage", "Outback" };
            int[] years = { 2020, 2019, 2021, 2018, 2022, 2020, 2017, 2019, 2021, 2020, 2018, 2022 };
            string[] plates = { "CB1234AA", "CA5678BB", "CC9012CC", "CB3456DD", "CA7890EE", "CC1234FF",
                                "CB5678GG", "CA9012HH", "CC3456II", "CB7890JJ", "CA1234KK", "CC5678LL" };
            List<Customer> customers = context.Customers.Take(12).ToList();
            for (int i = 0; i < customers.Count && context.Vehicles.Count() < 12; i++)
            {
                Customer customer = customers[i];
                if (!context.Vehicles.Any(v => v.CustomerId == customer.Id))
                {
                    context.Vehicles.Add(new Vehicle
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = customer.Id,
                        Make = makes[i % makes.Length],
                        Model = models[i % models.Length],
                        Year = years[i % years.Length],
                        LicensePlate = plates[i % plates.Length],
                        IsDeleted = false
                    });
                }
            }
            await context.SaveChangesAsync();

            // Seed Services (12 total)
            string[] serviceNames = { "Oil Change", "Tire Rotation", "Brake Inspection", "Engine Tune-Up",
                                     "Wheel Alignment", "Battery Replacement", "AC Recharge", "Transmission Flush",
                                     "Coolant Flush", "Spark Plug Replacement", "Exhaust Repair", "Diagnostic Check" };
            string[] serviceDescs = { "Standard oil change service", "Rotate tires for even wear",
                                      "Inspect and service brake system", "Tune engine for optimal performance",
                                      "Align wheels for better handling", "Replace vehicle battery",
                                      "Recharge air conditioning system", "Flush and replace transmission fluid",
                                      "Flush and replace coolant", "Replace spark plugs",
                                      "Repair exhaust system", "Perform diagnostic check" };
            decimal[] prices = { 50.00m, 30.00m, 75.00m, 120.00m, 80.00m, 100.00m, 60.00m, 150.00m, 90.00m,
                                70.00m, 110.00m, 40.00m };
            for (int i = 0; i < serviceNames.Length && context.Services.Count() < 12; i++)
            {
                if (!context.Services.Any(s => s.Name == serviceNames[i]))
                {
                    context.Services.Add(new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = serviceNames[i],
                        Description = serviceDescs[i],
                        Price = prices[i],
                        IsDeleted = false
                    });
                }
            }
            await context.SaveChangesAsync();

            // Seed Appointments (12 total)
            List<Service> services = context.Services.Take(12).ToList();
            List<Mechanic> mechanics = context.Mechanics.Take(12).ToList();
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            for (int i = 0; i < customers.Count && context.Appointments.Count() < 12; i++)
            {
                Customer customer = customers[i % customers.Count];
                Vehicle? vehicle = context.Vehicles.FirstOrDefault(v => v.CustomerId == customer.Id);
                Service service = services[i % services.Count];
                Mechanic mechanic = mechanics[i % mechanics.Count];
                if (vehicle != null)
                {
                    context.Appointments.Add(new Appointment
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = customer.Id,
                        VehicleId = vehicle.Id,
                        ServiceId = service.Id,
                        MechanicId = mechanic.Id,
                        Date = startDate.AddDays(i),
                        Status = AppointmentStatus.Confirmed,
                        Notes = $"Appointment for {service.Name}",
                        IsDeleted = false
                    });
                }
            }
            await context.SaveChangesAsync();
        }
    }
}
