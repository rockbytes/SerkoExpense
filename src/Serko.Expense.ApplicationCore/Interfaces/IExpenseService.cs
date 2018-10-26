using System;
using System.Collections.Generic;
using System.Text;
using Serko.Expense.ApplicationCore.Dtos;

namespace Serko.Expense.ApplicationCore.Interfaces
{
    public interface IExpenseService
    {
        ExpenseDto ExtractExpenseFromText(string expenseText);
    }
}
