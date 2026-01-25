using backend.Models;

namespace backend.Data
{
    public static class DbSeederStatic
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.EnsureCreated();

            // Lijst met rollen die we sowieso nodig hebben
            string[] rolesToSeed = { "Patiënt", "Admin", "Huisarts", "Specialist", "Administratiemedewerker" };

            foreach (var roleName in rolesToSeed)
            {
                // Check per rol of deze al bestaat
                if (!context.Roles.Any(r => r.RoleName == roleName))
                {
                    context.Roles.Add(new Role { RoleName = roleName });
                }
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
            Console.WriteLine("[DbSeederStatic] Basisrollen gecontroleerd/aangemaakt.");
        }
    }
}
