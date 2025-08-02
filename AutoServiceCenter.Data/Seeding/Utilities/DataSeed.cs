using AutoServiceCenter.Data;
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
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Seed Admin User
            var adminUser = new IdentityUser { UserName = "admin@auto.com", Email = "admin@auto.com", EmailConfirmed = true };
            if (await userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                }
            }

            // Seed Regular User
            var regularUser = new IdentityUser { UserName = "user@auto.com", Email = "user@auto.com", EmailConfirmed = true };
            if (await userManager.FindByEmailAsync(regularUser.Email) == null)
            {
                var result = await userManager.CreateAsync(regularUser, "User123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(regularUser, "User");
                }
            }

            // Seed Services
            if (!context.Services.Any())
            {
                context.Services.AddRange(
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Oil Change",
                        Description = "Standard oil change service",
                        Price = 50.00m,
                        IsDeleted = false
                    },
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Tire Rotation",
                        Description = "Rotate tires for even wear",
                        Price = 30.00m,
                        IsDeleted = false
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed Mechanic
            if (!context.Mechanics.Any())
            {
                var mechanicUser = new IdentityUser { UserName = "mechanic@auto.com", Email = "mechanic@auto.com", EmailConfirmed = true };
                if (await userManager.FindByEmailAsync(mechanicUser.Email) == null)
                {
                    var result = await userManager.CreateAsync(mechanicUser, "Mechanic123!");
                    if (result.Succeeded)
                    {
                        context.Mechanics.Add(new Mechanic
                        {
                            Id = Guid.NewGuid(),
                            UserId = mechanicUser.Id,
                            Specialization = "General Repair",
                            ExperienceYears = 5,
                            IsDeleted = false
                        });
                        await context.SaveChangesAsync();
                    }
                }
            }

            // Seed Customer and Vehicle
            if (!context.Customers.Any())
            {
                var customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    UserId = regularUser.Id,
                    Address = "Liulin 10 blok 122 vhod A ap 4",
                    IsDeleted = false
                };
                context.Customers.Add(customer);
                await context.SaveChangesAsync();

                context.Vehicles.Add(new Vehicle
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customer.Id,
                    Make = "Toyota",
                    Model = "Camry",
                    Year = 2020,
                    LicensePlate = "CB1234AA",
                    IsDeleted = false
                });
                await context.SaveChangesAsync();
            }
        }
    }
}