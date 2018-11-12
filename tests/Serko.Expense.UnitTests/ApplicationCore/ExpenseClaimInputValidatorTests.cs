using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Localization;
using Serko.Expense.ApplicationCore.Dtos;
using Serko.Expense.ApplicationCore.Validators;
using Serko.Expense.TestHelper;
using Xunit;

namespace Serko.Expense.UnitTests.ApplicationCore
{
	public class ExpenseClaimInputValidatorTests
	{
	    private readonly IStringLocalizer<ExpenseClaimInputValidator> _localizer;
	    private readonly ExpenseClaimInputValidator _inputValidator;

	    public ExpenseClaimInputValidatorTests()
	    {
	        _localizer = TestStringLocalizerFactory<ExpenseClaimInputValidator>.Localizer;
	        _inputValidator = CreateValidator(_localizer);
	    }

        #region NotNullEmpty

        [Fact]
		public void NotNullEmpty_Empty()
		{
			// Arrange
			var expenseClaimText = "";

			var expectedErrorMsgs = new List<string>
			{
                _localizer["ExpenseClaimTextNotBeBlank"]
                
			};

			ActAndAssert(expenseClaimText, expectedErrorMsgs);
		}

		[Fact]
		public void NotNullEmpty_Null()
		{
			// Arrange
			string expenseClaimText = null;

			var expectedErrorMsgs = new List<string>
			{
			    _localizer["ExpenseClaimTextNotBeBlank"]
            };

			ActAndAssert(expenseClaimText, expectedErrorMsgs);
		}

		#endregion

		#region TotalTagPresent

		[Fact]
		public void TotalTagPresent_ValidData()
		{
			// Arrange
			var input = new ExpenseClaimInput
			{
				ExpenseClaimText = @"...
<total>1024.01</total><payment_method>personal card</payment_method>"
			};

			// Act
			var result = _inputValidator.Validate(input);

			// Assert
			Assert.True(result.Errors.Count == 0);
		}

		[Fact]
		public void TotalTagPresent_NotNumericValue()
		{
			// Arrange
			var amount = "1024.01abc";

			var expenseClaimText = $@"...
<total>{amount}</total><payment_method>personal card</payment_method>";

			var expectedErrorMsgs = new List<string>
			{
                _localizer["TotalAmountShouldBeNumeric", amount]
			};

			ActAndAssert(expenseClaimText, expectedErrorMsgs);
		}

		[Fact]
		public void TotalTagPresent_WithMixedUpperLowerCase()
		{
			// Arrange
			var expenseClaimText = @"...
<tOtAl>1024.01</tOtAl><payment_method>personal card</payment_method>";

			var expectedErrorMsgs = new List<string>
			{
                _localizer["TotalAmountNotPresentInExpenseClaimText"]
			};

			ActAndAssert(expenseClaimText, expectedErrorMsgs);
		}

		[Fact]
		public void TotalTagPresent_TotalTagMissing()
		{
			// Arrange
			var tag = "/total";
			var expenseClaimText = $@"...
total>1024.01<{tag}><payment_method>personal card</payment_method>";

			var expectedErrorMsgs = new List<string>
			{
                _localizer["ClosingTagXHasNoCorrespondingOpeningTags", tag],
                _localizer["TotalAmountNotPresentInExpenseClaimText"]
			};

			ActAndAssert(expenseClaimText, expectedErrorMsgs);
		}

		#endregion

		#region OpeningClosingTagsMatched

		[Fact]
		public void OpeningClosingTagsMatched_Matched()
		{
			// Arrange
			var input = new ExpenseClaimInput
			{
				ExpenseClaimText = CommonData.ValidInput
			};

			// Act
			var result = _inputValidator.Validate(input);

			// Assert
			Assert.True(result.Errors.Count == 0);
		}

		[Fact]
		public void OpeningClosingTagsMatched_OpeningTagsMissing()
		{
			// Arrange
			var tag = "/expense";

			var expenseClaimText = $@"...
<cost_centre>DEV002</cost_centre><total>1024.01</total><{tag}>";

			var expectedErrorMsgs = new List<string>
			{
                _localizer["ClosingTagXHasNoCorrespondingOpeningTags", tag]
			};

			ActAndAssert(expenseClaimText, expectedErrorMsgs);
		}

		[Fact]
		public void OpeningClosingTagsMatched_ClosingTagsMissing()
		{
			// Arrange
			var tag = "cost_centre";
			var expenseClaimText = $@"...
<expense><total>1024.01</total><{tag}>DEV002</expense> ";

			var expectedErrorMsgs = new List<string>
			{
			    _localizer["OpeningTagXHasNoCorrespondingClosingTags", tag]
            };

			ActAndAssert(expenseClaimText, expectedErrorMsgs);
		}

		[Fact]
		public void OpeningClosingTagsMatched_BothOpeningAndClosingTagsMissing()
		{
			// Arrange
			var tag = "total";
			var expenseClaimText = $@"...
<cost_centre>DEV002</cost_centre><{tag}>1024.01</expense>";

			var expectedErrorMsgs = new List<string>
			{
			    _localizer["OpeningTagXHasNoCorrespondingClosingTags", tag],
			    _localizer["TotalAmountNotPresentInExpenseClaimText"]
            };

			ActAndAssert(expenseClaimText, expectedErrorMsgs);
		}

		[Fact]
		public void OpeningClosingTagsMatched_OnlyHaveOpeningTags()
		{
			// Arrange
			var expenseClaimText = @"...
<cost_centre>DEV002<payment_method><total>1024.01</total>";

			var expectedErrorMsgs = new List<string>
			{
                _localizer["OpeningTagXHasNoCorrespondingClosingTags", "payment_method"]
			};

			ActAndAssert(expenseClaimText, expectedErrorMsgs);
		}

		[Fact]
		public void OpeningClosingTagsMatched_OnlyHaveClosingTags()
		{
			// Arrange
			var expenseClaimText = @"...
</cost_centre>DEV002</payment_method><total>1024.01</total>";

			var expectedErrorMsgs = new List<string>
			{
			    _localizer["ClosingTagXHasNoCorrespondingOpeningTags", "/cost_centre"]
            };

			ActAndAssert(expenseClaimText, expectedErrorMsgs);
		}

		#endregion

		private void ActAndAssert(string expenseClaimTextInput, IList<string> expectedErrorMsgs)
		{
			// Act
			var result = _inputValidator.Validate(new ExpenseClaimInput
			{
				ExpenseClaimText = expenseClaimTextInput
			});

			// Assert
			Assert.Equal(expectedErrorMsgs, result.Errors.Select(e => e.ErrorMessage));
		}

        private static ExpenseClaimInputValidator CreateValidator(
            IStringLocalizer<ExpenseClaimInputValidator> localizer)
        {
            return new ExpenseClaimInputValidator(localizer);
        }
    }
}
