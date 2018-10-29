using System.Collections.Generic;
using Serko.Expense.ApplicationCore.Dtos;

namespace Serko.Expense.ApplicationCore.Interfaces
{
    public interface IExpenseService
    {
		IDictionary<string, string> CreateExpenseClaimFromInput(ExpenseClaimInput input);
    }
}
