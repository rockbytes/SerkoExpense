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

		/// <summary>
		/// Extract expense claim data from text
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		///     POST /api/expense 
		///        "<expense>
		/// <cost_centre>DEV002</cost_centre>
		/// <total>1024.01</total>
		/// <payment_method>personal card</payment_method>
		/// </expense>"
		///
		/// </remarks>
		/// <param name="expenseClaimText"></param>
		/// <returns>Expense claim data extracted from input text</returns>
		/// <response code="200">Return the extracted expense claim data</response>
		/// <response code="400">Invalid input</response>
		[HttpPost]
		public IActionResult Create([FromBody] string expenseClaimText)
		{
			return Ok(_expenseService.CreateExpenseClaimFromInput(
				new ExpenseClaimInput
				{
					ExpenseClaimText = expenseClaimText
				}));
		}
	}
}
