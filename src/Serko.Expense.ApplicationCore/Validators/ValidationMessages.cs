using System;
using System.Collections.Generic;
using System.Text;

namespace Serko.Expense.ApplicationCore.Validators
{
    public class ValidationMessages
    {
        public static readonly string ExpenseClaimTextNotBeBlank =
            "The expense claim text should not be blank.";

        public static readonly string ExpenseClaimTextOpeningClosingTagsMatched =
            "The expense claim text should have its opening and closing XML tags matched.";

        public static readonly string ExpenseClaimTextSpecifyNumericTotalValue =
            "The expense claim text should specify a numeric value with <total> XML tag.";
    }
}
