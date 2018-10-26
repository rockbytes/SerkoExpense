using System;
using System.Collections.Generic;
using System.Text;

namespace Serko.Expense.ApplicationCore.Dtos
{
    public class ExpenseDto
    {
        public decimal Gst { get; set; }
        public decimal TotalWithGstExcluded { get; set; }
        public decimal TotalWithGstIncluded { get; set; }
        public string CostCentre { get; set; }
    }
}
