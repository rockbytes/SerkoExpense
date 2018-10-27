using System;
using System.Collections.Generic;
using System.Text;

namespace Serko.Expense.ApplicationCore.Interfaces
{
    public interface IExpenseTextValidator
    {
        bool Validate(string expenseText);
    }
}
