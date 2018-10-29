using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Serko.Expense.ApplicationCore.Dtos;
using Serko.Expense.ApplicationCore.Interfaces;

namespace Serko.Expense.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] string expenseClaimText)
        {
            return Ok(_expenseService.CreateExpenseClaimFromInput(
				new ExpenseClaimInput
				{
					ExpenseClaimText = expenseClaimText
				}));
        }
    }
}
