using backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace backend.Data
{
    public static class DbSeederStatic
    {
        public static void Seed(AppDbContext context)
        {
            Console.WriteLine("[DbSeederStatic] Start seeding basisdata...");

            // 1. Rollen (Idempotent)
            string[] rolesToSeed = { "Patiënt", "Admin", "Huisarts", "Specialist", "Administratiemedewerker" };
            var existingRoles = context.Roles.Select(r => r.RoleName).ToHashSet();
            
            foreach (var roleName in rolesToSeed)
            {
                if (!existingRoles.Contains(roleName))
                {
                    context.Roles.Add(new Role { RoleName = roleName });
                    Console.WriteLine($"[DbSeederStatic] Rol toegevoegd: {roleName}");
                }
            }
            context.SaveChanges();

            // 2. Specialismen (Caching in Dictionary voor snelheid)
            var specialismNames = new[]
            {
                "Cardiologie", "Dermatologie", "Longgeneeskunde", "Oogheelkunde", "KNO",
                "Orthopedie", "Chirurgie", "Radiologie", "Laboratoriumgeneeskunde",
                "Revalidatiegeneeskunde", "Psychiatrie", "Algemeen"
            };

            var existingSpecs = context.Specialisms.ToDictionary(s => s.Name, s => s.Id);
            foreach (var name in specialismNames)
            {
                if (!existingSpecs.ContainsKey(name))
                {
                    var newSpec = new Specialism { Name = name };
                    context.Specialisms.Add(newSpec);
                    context.SaveChanges(); // Direct saven om ID te genereren voor de Dictionary
                    existingSpecs.Add(name, newSpec.Id);
                    Console.WriteLine($"[DbSeederStatic] Specialisme toegevoegd: {name}");
                }
            }

            // 3. Behandelingen (Efficiente Bulk Check)
            var treatmentsToEnsure = GetTreatmentList(existingSpecs);
            var existingTreatmentCodes = context.Treatments.Select(t => t.Code).ToHashSet();
            
            int addedCount = 0;
            foreach (var t in treatmentsToEnsure)
            {
                if (!existingTreatmentCodes.Contains(t.Code))
                {
                    context.Treatments.Add(t);
                    addedCount++;
                }
            }

            if (addedCount > 0)
            {
                context.SaveChanges();
                Console.WriteLine($"[DbSeederStatic] {addedCount} nieuwe behandelingen toegevoegd.");
            }

            Console.WriteLine("[DbSeederStatic] Seeding succesvol voltooid.");
        }

        private static List<Treatment> GetTreatmentList(Dictionary<string, int> specs)
        {
            // We gebruiken een List met object initializers voor maximale leesbaarheid in de PR
            return new List<Treatment>
            {
                new() { Code = "Z0001", Description = "Consult hart", SpecialismId = specs["Cardiologie"], DurationInQuarters = 3, Cost = 144 },
                new() { Code = "Z0002", Description = "Consult huid", SpecialismId = specs["Dermatologie"], DurationInQuarters = 3, Cost = 51 },
                new() { Code = "Z0003", Description = "Consult longen", SpecialismId = specs["Longgeneeskunde"], DurationInQuarters = 2, Cost = 117 },
                new() { Code = "Z0004", Description = "Consult ogen", SpecialismId = specs["Oogheelkunde"], DurationInQuarters = 3, Cost = 114 },
                new() { Code = "Z0005", Description = "Consult oren", SpecialismId = specs["KNO"], DurationInQuarters = 3, Cost = 65 },
                new() { Code = "Z0006", Description = "Operatie knie", SpecialismId = specs["Orthopedie"], DurationInQuarters = 2, Cost = 1084 },
                new() { Code = "Z0007", Description = "Operatie heup", SpecialismId = specs["Orthopedie"], DurationInQuarters = 3, Cost = 2909 },
                new() { Code = "Z0008", Description = "Operatie hart", SpecialismId = specs["Cardiologie"], DurationInQuarters = 4, Cost = 2572 },
                new() { Code = "Z0009", Description = "Operatie huid", SpecialismId = specs["Chirurgie"], DurationInQuarters = 4, Cost = 2739 },
                new() { Code = "Z0010", Description = "Operatie rug", SpecialismId = specs["Orthopedie"], DurationInQuarters = 3, Cost = 2903 },
                new() { Code = "Z0011", Description = "Controle hart", SpecialismId = specs["Cardiologie"], DurationInQuarters = 3, Cost = 187 },
                new() { Code = "Z0012", Description = "Controle huid", SpecialismId = specs["Dermatologie"], DurationInQuarters = 4, Cost = 172 },
                new() { Code = "Z0013", Description = "Controle longen", SpecialismId = specs["Longgeneeskunde"], DurationInQuarters = 3, Cost = 72 },
                new() { Code = "Z0014", Description = "Controle ogen", SpecialismId = specs["Oogheelkunde"], DurationInQuarters = 3, Cost = 96 },
                new() { Code = "Z0015", Description = "Controle oren", SpecialismId = specs["KNO"], DurationInQuarters = 4, Cost = 150 },
                new() { Code = "Z0016", Description = "MRI-scan hoofd", SpecialismId = specs["Radiologie"], DurationInQuarters = 2, Cost = 609 },
                new() { Code = "Z0017", Description = "MRI-scan knie", SpecialismId = specs["Radiologie"], DurationInQuarters = 3, Cost = 223 },
                new() { Code = "Z0018", Description = "MRI-scan rug", SpecialismId = specs["Radiologie"], DurationInQuarters = 3, Cost = 314 },
                new() { Code = "Z0019", Description = "MRI-scan hart", SpecialismId = specs["Radiologie"], DurationInQuarters = 2, Cost = 728 },
                new() { Code = "Z0020", Description = "MRI-scan buik", SpecialismId = specs["Radiologie"], DurationInQuarters = 2, Cost = 395 },
                new() { Code = "Z0021", Description = "CT-scan hoofd", SpecialismId = specs["Radiologie"], DurationInQuarters = 3, Cost = 682 },
                new() { Code = "Z0022", Description = "CT-scan knie", SpecialismId = specs["Radiologie"], DurationInQuarters = 3, Cost = 616 },
                new() { Code = "Z0023", Description = "CT-scan rug", SpecialismId = specs["Radiologie"], DurationInQuarters = 3, Cost = 378 },
                new() { Code = "Z0024", Description = "CT-scan hart", SpecialismId = specs["Radiologie"], DurationInQuarters = 3, Cost = 793 },
                new() { Code = "Z0025", Description = "CT-scan buik", SpecialismId = specs["Radiologie"], DurationInQuarters = 4, Cost = 448 },
                new() { Code = "Z0026", Description = "Bloedonderzoek algemeen", SpecialismId = specs["Laboratoriumgeneeskunde"], DurationInQuarters = 1, Cost = 55 },
                new() { Code = "Z0027", Description = "Bloedonderzoek hormonen", SpecialismId = specs["Laboratoriumgeneeskunde"], DurationInQuarters = 2, Cost = 92 },
                new() { Code = "Z0028", Description = "Bloedonderzoek infecties", SpecialismId = specs["Laboratoriumgeneeskunde"], DurationInQuarters = 1, Cost = 71 },
                new() { Code = "Z0029", Description = "Bloedonderzoek suiker", SpecialismId = specs["Laboratoriumgeneeskunde"], DurationInQuarters = 4, Cost = 98 },
                new() { Code = "Z0030", Description = "Bloedonderzoek cholesterol", SpecialismId = specs["Laboratoriumgeneeskunde"], DurationInQuarters = 3, Cost = 38 },
                new() { Code = "Z0031", Description = "Fysiotherapie knie", SpecialismId = specs["Revalidatiegeneeskunde"], DurationInQuarters = 1, Cost = 43 },
                new() { Code = "Z0032", Description = "Fysiotherapie rug", SpecialismId = specs["Revalidatiegeneeskunde"], DurationInQuarters = 3, Cost = 70 },
                new() { Code = "Z0033", Description = "Fysiotherapie schouder", SpecialismId = specs["Revalidatiegeneeskunde"], DurationInQuarters = 2, Cost = 101 },
                new() { Code = "Z0034", Description = "Fysiotherapie heup", SpecialismId = specs["Revalidatiegeneeskunde"], DurationInQuarters = 4, Cost = 103 },
                new() { Code = "Z0035", Description = "Fysiotherapie nek", SpecialismId = specs["Revalidatiegeneeskunde"], DurationInQuarters = 1, Cost = 93 },
                new() { Code = "Z0036", Description = "Psychologisch gesprek depressie", SpecialismId = specs["Psychiatrie"], DurationInQuarters = 4, Cost = 123 },
                new() { Code = "Z0037", Description = "Psychologisch gesprek angst", SpecialismId = specs["Psychiatrie"], DurationInQuarters = 3, Cost = 132 },
                new() { Code = "Z0038", Description = "Psychologisch gesprek trauma", SpecialismId = specs["Psychiatrie"], DurationInQuarters = 1, Cost = 72 },
                new() { Code = "Z0039", Description = "Psychologisch gesprek burn-out", SpecialismId = specs["Psychiatrie"], DurationInQuarters = 3, Cost = 172 },
                new() { Code = "Z0040", Description = "Psychologisch gesprek stress", SpecialismId = specs["Psychiatrie"], DurationInQuarters = 4, Cost = 135 },
                new() { Code = "Z0041", Description = "Vaccinatie griep", SpecialismId = specs["Algemeen"], DurationInQuarters = 3, Cost = 32 },
                new() { Code = "Z0042", Description = "Vaccinatie hepatitis", SpecialismId = specs["Algemeen"], DurationInQuarters = 2, Cost = 57 },
                new() { Code = "Z0043", Description = "Vaccinatie tetanus", SpecialismId = specs["Algemeen"], DurationInQuarters = 1, Cost = 61 },
                new() { Code = "Z0044", Description = "Vaccinatie covid", SpecialismId = specs["Algemeen"], DurationInQuarters = 2, Cost = 45 },
                new() { Code = "Z0045", Description = "Vaccinatie HPV", SpecialismId = specs["Algemeen"], DurationInQuarters = 4, Cost = 30 },
                new() { Code = "Z0046", Description = "Wondverzorging arm", SpecialismId = specs["Chirurgie"], DurationInQuarters = 2, Cost = 55 },
                new() { Code = "Z0047", Description = "Wondverzorging been", SpecialismId = specs["Chirurgie"], DurationInQuarters = 2, Cost = 111 },
                new() { Code = "Z0048", Description = "Wondverzorging hoofd", SpecialismId = specs["Chirurgie"], DurationInQuarters = 2, Cost = 33 },
                new() { Code = "Z0049", Description = "Wondverzorging buik", SpecialismId = specs["Chirurgie"], DurationInQuarters = 4, Cost = 56 },
                new() { Code = "Z0050", Description = "Wondverzorging rug", SpecialismId = specs["Chirurgie"], DurationInQuarters = 4, Cost = 103 },
                new() { Code = "Z0100", Description = "Bloedonderzoek infecties", SpecialismId = specs["Laboratoriumgeneeskunde"], DurationInQuarters = 4, Cost = 37 }
            };
        }
    }
}