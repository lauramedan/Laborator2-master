using Laborator2.Models;
using Laborator2.Services;
using Laborator2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laborator2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private IExpenseService expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            this.expenseService = expenseService;
        }

        /// <summary>
        /// GET: api/Expenses
        /// </summary>
        /// <param name="type">Optional , filter by type of expense</param>
        /// <param name="from">Optional , filter by minimum date </param>
        /// <param name="to">Optional , filter by maximum date</param>
        /// <returns>List of expenses objects</returns>
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        [Authorize(Roles = "Regular,Admin")]
        [HttpGet]
        public PaginatedList<Expense> Get([FromQuery]Type? type, [FromQuery]DateTime? from, [FromQuery]DateTime? to, [FromQuery]int page = 1)
        {
            page = Math.Max(page, 1);
            return expenseService.GetAll(page, type, from, to);
        }


        /// <summary>
        /// GET: api/Expenses/1
        /// </summary>
        /// <param name="id">Expense id</param>
        /// <returns>The expense with the given id</returns>
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        [Authorize(Roles = "Regular,Admin")]
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            var found = expenseService.GetById(id);

            if (found == null)
            {
                return NotFound();
            }
            return Ok(found);
        }

        /// <summary>
        /// PUT: api/Expenses
        /// </summary>
        ///<remarks>
        /// Sample request:
        ///  {
        ///     id: 7,
        ///     description: "red dress",
        ///     sum: 500,
        ///     location: "Iulius Mall",
        ///     date: "2011-04-22T00:00:00",
        ///     currency: "lei",
        ///     type: 3,
        ///     comments: [
        ///                  {
        ///                   id: 3,
        ///                   text: "first comment",
        ///                   importan: false
        ///                  },
        ///                  {
        ///                   id: 4,
        ///                   text: "second comment",
        ///                   importan: false
        ///                  }
        ///               ]
        ///  }
        /// </remarks>
        /// <param name="expense">The expense to add</param>
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        [Authorize(Roles = "Admin,Regular")]
        [HttpPost]
        public void Post([FromBody] Expense expense)
        {
            expenseService.Create(expense);
        }


        /// <summary>
        ///  PUT: api/Expenses/3
        /// </summary>
        /// <param name="id">the expense id to upsert</param>
        /// <param name="expense">Expense to upsert</param>
        /// <returns>The upsert expense</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [Authorize(Roles = "Regular,Admin")]
        public IActionResult Put(int id, [FromBody] Expense expense)
        {
            var result = expenseService.Upsert(id, expense);
            return Ok(expense);
        }


        /// <summary>
        /// DELETE: api/ApiWithActions/5
        /// </summary>
        /// <param name="id">Expense id to delete</param>
        /// <returns>The deleted expense or null if there is no expense with the given id</returns>
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        [Authorize(Roles = "Regular,Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = expenseService.Delete(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}