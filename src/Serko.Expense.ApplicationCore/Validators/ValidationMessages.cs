namespace Serko.Expense.ApplicationCore.Validators
{
    public class ValidationMessages
    {
        public static readonly string ExpenseClaimTextNotBeBlank =
            "The expense claim text should not be blank.";

        public static readonly string OpeningTagXHasNoCorrespondingClosingTags =
            "The opening tag <{0}> has no corresponding closing tag.";

        public static readonly string ClosingTagXHasNoCorrespondingOpeningTags =
            "The closing tag <{0}> has no corresponding opening tag.";

        public static readonly string TotalAmountNotPresentInExpenseClaimText =
            "The <total> amount is not present in the expense claim text.";

        public static readonly string TotalAmountShouldBeNumeric =
            "The amount '{0}' specified by <total> should be a numeric value.";

    }
}
