using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laborator2.ViewModels
{
    public class PaginatedList<T>
    {

        public const int EntriesPerPage = 3;

        public int NumberOfPages { get; set; }

        public int CurrentPage { get; set; }

        public List<T> Entries { get; set; }
    }
}