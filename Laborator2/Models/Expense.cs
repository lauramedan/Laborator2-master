using Laborator2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Laborator2
{
    public enum Type
    {
        food,
        utilities,
        transportation,
        outing,
        groceries,
        clothes,
        electronics,
        other
    }

    public class Expense
    {
        //[Key()]
        public int Id { get; set; }
        public string Description { get; set; }
        public int Sum { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        [EnumDataType(typeof(Type))]
        public Type Type { get; set; }
        public List<Comment> Comments { get; set; }

    }
}
