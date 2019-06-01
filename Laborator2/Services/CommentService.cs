using Laborator2.Models;
using Laborator2.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laborator2.Services
{
    public interface ICommentService
    {
        IEnumerable<CommentGetModel> GetAllComments(string text);
    }

    public class CommentService : ICommentService
    {
        private ExpensesDbContext context;

        public CommentService(ExpensesDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<CommentGetModel> GetAllComments(string filterText)
        {
            IQueryable<Expense> result = context.Expenses.Include(c => c.Comments);

            List<CommentGetModel> resultComments = new List<CommentGetModel>();
            List<CommentGetModel> resultCommentsNoFilter = new List<CommentGetModel>();

            foreach (Expense expense in result)
            {
                expense.Comments.ForEach(comment =>
                {
                    CommentGetModel newComment = CommentGetModel.ConvertToCommentsGetModel(comment, expense);
                    
                    if (comment.Text == null || filterText == null)
                    {
                        resultCommentsNoFilter.Add(newComment);
                    }
                    else if (comment.Text.Contains(filterText))
                    {
                        resultComments.Add(newComment);
                    }
                });
            }

            //dysplay result 
            if (filterText == null)
            {
                return resultCommentsNoFilter;
            }
                return resultComments;
        }

    }
}
