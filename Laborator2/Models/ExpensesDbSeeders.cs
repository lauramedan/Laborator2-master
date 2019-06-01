using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laborator2.Models
{
    public class ExpensesDbSeeders
    {
        public static void Initialize(ExpensesDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any expenses.
            if (context.Expenses.Any())
            {
                return;   // DB has been seeded
            }

            context.Expenses.AddRange(
                new Expense
                {
                    Description = "shirt",
                    Sum = 50,
                    Location = "Iulius Mall",
                    Date = new DateTime(2011, 4, 22),
                    Currency = "lei",
                    Type = Type.clothes
                },
                 new Expense
                 {
                     Description = "hat",
                     Sum = 10,
                     Location = "Iulius Mall",
                     Date = new DateTime(2011, 5, 22),
                     Currency = "lei",
                     Type = Type.clothes
                 },
                 new Expense
                 {
                     Description = "Bus ticket",
                     Sum = 5,
                     Location = "Cluj-Napoca",
                     Date = new DateTime(2011, 5, 21),
                     Currency = "lei",
                     Type = Type.transportation
                 },
                 new Expense
                 {
                     Description = "Bus tickets",
                     Sum = 10,
                     Location = "Cluj-Napoca",
                     Date = new DateTime(2011, 6, 04),
                     Currency = "lei",
                     Type = Type.transportation
                 }
            );
            context.SaveChanges();
        }
    }
}
