using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Serko.Expense.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] string expenseText)
        {
            return Ok("post");
        }
    }
}
