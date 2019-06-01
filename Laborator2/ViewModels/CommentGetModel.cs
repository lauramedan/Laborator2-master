using Laborator2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laborator2.ViewModels
{
    public class CommentGetModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool Importan { get; set; }
        public int ExpenseId { get; set; }

        public static CommentGetModel ConvertToCommentsGetModel(Comment comment, Expense expense)
        {
            return new CommentGetModel
            {
                Id = comment.Id,
                Importan = comment.Importan,
                Text = comment.Text,
                ExpenseId = expense.Id
            };
        }
    }
}
