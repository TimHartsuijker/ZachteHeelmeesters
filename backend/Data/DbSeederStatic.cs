using backend.Models;
using Microsoft.AspNetCore.Identity;

namespace backend.Data
{
    public static class DbSeederStatic
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.EnsureCreated();

            var passwordHasher = new PasswordHasher<User>();

            // Rollen
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role {RoleName = "Patiënt" },
                    new Role {RoleName = "Admin" },
                    new Role {RoleName = "Huisarts" },
                    new Role {RoleName = "Specialist" }
                );
            }

            var adminRole = context.Roles.FirstOrDefault(r => r.RoleName == "Admin");
            var adminUser = context.Users.FirstOrDefault(u => u.Email == "admin@zhm.com");
            if (adminUser == null)
            {
                adminUser = new User
                {
                    FirstName = "Jaap",
                    LastName = "Ros",
                    Email = "admin@zhm.com",
                    StreetName = "Onderwijsboulevard",
                    HouseNumber = "215",
                    PostalCode = "5223DE",
                    PhoneNumber = "0612345678",
                    DateOfBirth = DateTime.Parse("2005-06-26"),
                    Gender = "Man",
                    CitizenServiceNumber = "123456789",
                    CreatedAt = DateTime.UtcNow,
                    RoleId = adminRole.Id
                };

                adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "AdminWachtwoord123!");
                context.Users.Add(adminUser);
                context.SaveChanges();
                Console.WriteLine($"[DbSeederTest] Created Patient user with ID: {adminUser.Id}");
            }
            else
            {
                Console.WriteLine($"[DbSeederTest] Patient user already exists with ID: {adminUser.Id}");
            }

            //// Departments
            //if (!context.Departments.Any())
            //{
            //    context.Departments.AddRange(
            //        new Department { Id = 1, Name = "Orthopedics" },
            //    );
            //}

            //// Specialisms
            //if (!context.Specialisms.Any())
            //{
            //    context.Specialisms.AddRange(
            //        new Specialism { Id = 1, Name = "Knee Surgery" }
            //    );
            //}

            context.SaveChanges();
        }
    }
}
