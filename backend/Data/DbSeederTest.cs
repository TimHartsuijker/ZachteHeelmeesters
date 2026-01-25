using System;
using System.Collections.Generic;
using System.Linq;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public static class DbSeederTest
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.EnsureCreated();
            var passwordHasher = new PasswordHasher<User>();

            // 1. Rollen ophalen of aanmaken (idempotent)
            Role GetRole(string name) => context.Roles.FirstOrDefault(r => r.RoleName == name) 
                ?? context.Roles.Add(new Role { RoleName = name }).Entity;

            var patientRole = GetRole("Patiënt");
            var specialistRole = GetRole("Specialist");
            var gpRole = GetRole("Huisarts");
            var adminRole = GetRole("Admin");
            var adminMedewerkerRole = GetRole("Administratiemedewerker");
            context.SaveChanges();

            // 2. Centrale Helper voor Gebruikers (voorkomt BSN en Email duplicaten)
            User EnsureUser(User user, string password)
            {
                var existing = context.Users.FirstOrDefault(u => u.Email == user.Email);
                if (existing != null) return existing;

                // Voorkom BSN (CitizenServiceNumber) conflicten bij herhaalde runs
                if (context.Users.Any(u => u.CitizenServiceNumber == user.CitizenServiceNumber))
                {
                    user.CitizenServiceNumber = new Random().Next(100000000, 999999999).ToString();
                }

                user.PasswordHash = passwordHasher.HashPassword(user, password);
                user.CreatedAt = DateTime.UtcNow;
                var newUser = context.Users.Add(user).Entity;
                context.SaveChanges();
                Console.WriteLine($"[DbSeederTest] Gebruiker aangemaakt: {user.Email}");
                return newUser;
            }

            // --- SEEDING START ---

            // 1. Administratie Medewerker
            EnsureUser(new User
            {
                FirstName = "Admin", LastName = "Medewerker", Email = "administratie@example.com",
                StreetName = "Adminstraat", HouseNumber = "50", PostalCode = "1234AB",
                PhoneNumber = "0611111111", DateOfBirth = DateTime.UtcNow.AddYears(-30),
                Gender = "Vrouw", CitizenServiceNumber = "111111111", RoleId = adminMedewerkerRole.Id
            }, "AdminMed123");

            // 2. Hoofd Specialist (Doctor)
            var mainDoctor = EnsureUser(new User
            {
                FirstName = "Test", LastName = "Doctor", Email = "testdoctor@example.com",
                StreetName = "Teststraat", HouseNumber = "1A", PostalCode = "1234AB",
                PhoneNumber = "0631234567", DateOfBirth = new DateTime(1990, 1, 1),
                CitizenServiceNumber = "012948356", Gender = "Man", RoleId = specialistRole.Id
            }, "password");

            // 3. Selenium Test Patiënt (gekoppeld aan Main Doctor)
            EnsureUser(new User
            {
                FirstName = "Test", LastName = "Gebruiker", Email = "gebruiker@example.com",
                StreetName = "Teststraat", HouseNumber = "1", PostalCode = "1234AB",
                CitizenServiceNumber = "123456789", DateOfBirth = new DateTime(2000, 1, 1),
                Gender = "Vrouw", PhoneNumber = "0612345678", RoleId = patientRole.Id,
                DoctorId = mainDoctor.Id
            }, "Wachtwoord123");

            // 4. Tweede Test Patiënt (patient2)
            EnsureUser(new User
            {
                FirstName = "Test", LastName = "PatiëntB", Email = "patient2@example.com",
                StreetName = "Teststraat", HouseNumber = "2", PostalCode = "1234AB",
                CitizenServiceNumber = "987654321", DateOfBirth = new DateTime(2001, 2, 2),
                Gender = "Vrouw", PhoneNumber = "0612345679", RoleId = patientRole.Id,
                DoctorId = mainDoctor.Id
            }, "Wachtwoord123");

            // 5. Systeem Admin
            EnsureUser(new User
            {
                FirstName = "System", LastName = "Administrator", Email = "admin@example.com",
                StreetName = "Adminstraat", HouseNumber = "99", PostalCode = "9999AA",
                PhoneNumber = "0600000000", DateOfBirth = DateTime.UtcNow,
                Gender = "Man", CitizenServiceNumber = "012345678", RoleId = adminRole.Id
            }, "Admin123");

            // 6. Extra Huisartsen (Doctor1 & Doctor2)
            EnsureUser(new User
            {
                FirstName = "Huisarts", LastName = "Een", Email = "doctor1@example.com",
                StreetName = "Doctorstraat", HouseNumber = "1", PostalCode = "1234AB",
                PhoneNumber = "0612345671", DateOfBirth = DateTime.UtcNow,
                Gender = "Man", CitizenServiceNumber = "987654322", RoleId = gpRole.Id
            }, "Huisarts123");

            EnsureUser(new User
            {
                FirstName = "Huisarts", LastName = "Twee", Email = "doctor2@example.com",
                StreetName = "Doctorstraat", HouseNumber = "2", PostalCode = "1234AB",
                PhoneNumber = "0612345672", DateOfBirth = DateTime.UtcNow,
                Gender = "Man", CitizenServiceNumber = "987654323", RoleId = gpRole.Id
            }, "Huisarts123");

            // 7. Huisarts Jan Jansen
            var gpJansen = EnsureUser(new User
            {
                FirstName = "Jan", LastName = "Jansen", Email = "gp.jansen@example.nl",
                StreetName = "Village Street", HouseNumber = "12A", PostalCode = "1234AB",
                PhoneNumber = "0612345673", RoleId = gpRole.Id, PracticeName = "Jansen General Practice",
                CitizenServiceNumber = "555666777", Gender = "Man", DateOfBirth = new DateTime(1975, 5, 20)
            }, "Wachtwoord123");

            // 8. Patiënt Emma de Vries (gekoppeld aan Jan Jansen)
            EnsureUser(new User
            {
                FirstName = "Emma", LastName = "de Vries", Email = "devries@example.com",
                StreetName = "Lime Tree Avenue", HouseNumber = "45", PostalCode = "5678CD",
                PhoneNumber = "0687654321", RoleId = patientRole.Id, CitizenServiceNumber = "223456789",
                Gender = "Vrouw", DateOfBirth = new DateTime(1990, 1, 12), DoctorId = gpJansen.Id
            }, "Wachtwoord123");

            Console.WriteLine("[DbSeederTest] Alle testgebruikers succesvol verwerkt.");
        }
    }
}