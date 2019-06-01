using Laborator2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laborator2.Services
{
    public interface IExpenseService
    {
        IEnumerable<Expense> GetAll(Type? type = null, DateTime? from = null, DateTime? to = null);
        Expense GetById(int id);
        Expense Create(Expense expense);
        Expense Upsert(int id, Expense expense);
        Expense Delete(int id);
    }

    public class ExpenseService : IExpenseService
    {
        private ExpensesDbContext context;

        public ExpenseService(ExpensesDbContext context)
        {
            this.context = context;
        }

        public Expense Create(Expense expense)
        {
            context.Expenses.Add(expense);
            context.SaveChanges();
            return expense;
        }

        public Expense Delete(int id)
        {
            var existing = context.Expenses.Include(e => e.Comments).FirstOrDefault(expense => expense.Id == id);
            if (existing == null)
            {
                return null;
            }
            context.Expenses.Remove(existing);
            context.SaveChanges();
            return existing;
        }

        public IEnumerable<Expense> GetAll(Type? type = null, DateTime? from = null, DateTime? to = null)
        {
            IQueryable<Expense> result = context.Expenses.Include(c => c.Comments);

            if (from == null && to == null && type == null)
            {
                return result;
            }
            if (type != null)
            {
                result = result.Where(e => e.Type.Equals(type));
            }

            if (from != null)
            {
                result = result.Where(e => e.Date >= from);
            }
            if (to != null)
            {
                result = result.Where(e => e.Date <= to);
            }
            return result;
        }

        public Expense Upsert(int id, Expense expense)
        {
            var existing = context.Expenses.AsNoTracking().FirstOrDefault(f => f.Id == id);
            if (existing == null)
            {
                context.Expenses.Add(expense);
                context.SaveChanges();
                return expense;
            }
            expense.Id = id;
            context.Expenses.Update(expense);
            context.SaveChanges();
            return expense;
        }

        public Expense GetById(int id)
        {
            return context.Expenses.Include(e => e.Comments).FirstOrDefault(e => e.Id == id);
        }
    }
}
