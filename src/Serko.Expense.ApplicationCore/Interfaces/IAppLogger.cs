using System;
using System.Collections.Generic;
using System.Text;

namespace Serko.Expense.ApplicationCore.Interfaces
{
    public interface IAppLogger<T>
    {
        void Info(string message, params object[] args);
        void Warn(string message, params object[] args);
        void Error(string message, params object[] args);
    }
}
