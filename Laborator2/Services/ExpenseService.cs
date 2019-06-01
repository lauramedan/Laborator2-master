using Laborator2.Models;
using Laborator2.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laborator2.Services
{
    public interface IExpenseService
    {
        PaginatedList<Expense> GetAll(int page, Type? type = null, DateTime? from = null, DateTime? to = null);
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

        public PaginatedList<Expense> GetAll(int page, Type? type = null, DateTime? from = null, DateTime? to = null)
        {
            IQueryable<Expense> result = context.Expenses
                .OrderBy(e => e.Id)
                .Include(c => c.Comments);

            PaginatedList<Expense> paginatedResult = new PaginatedList<Expense>();

            paginatedResult.CurrentPage = page;

            //if (from == null && to == null && type == null)
            //{
            //    return result;
            //}
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
          //  return result;

            paginatedResult.NumberOfPages = (result.Count() - 1) / PaginatedList<Expense>.EntriesPerPage + 1;

            result = result.Skip((page - 1) * PaginatedList<Expense>.EntriesPerPage)
                            .Take(PaginatedList<Expense>.EntriesPerPage);

            paginatedResult.Entries = result.ToList();
            return paginatedResult;
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
