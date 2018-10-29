using System.Collections.Generic;
using System.Linq;
using Serko.Expense.ApplicationCore.Dtos;
using Serko.Expense.ApplicationCore.Validators;
using Xunit;

namespace Serko.Expense.UnitTests.ApplicationCore
{
	public class ExpenseClaimInputValidatorTests
	{
		private readonly ExpenseClaimInputValidator _inputValidator = 
			new ExpenseClaimInputValidator();

		#region NotNullEmpty

		[Fact]
		public void NotNullEmpty_Empty()
		{
            // Arrange
            var expenseClaimText = "";

            var expectedErrorMsgs = new List<string>
            {
                ValidationMessages.ExpenseClaimTextNotBeBlank            
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
                ValidationMessages.ExpenseClaimTextNotBeBlank
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

            var expenseClaimText = @"...
<total>1024.01abc</total><payment_method>personal card</payment_method>";

            var expectedErrorMsgs = new List<string>
            {
                ValidationMessages.ExpenseClaimTextSpecifyNumericTotalValue
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
                ValidationMessages.ExpenseClaimTextSpecifyNumericTotalValue
            };

            ActAndAssert(expenseClaimText, expectedErrorMsgs);
        }

		[Fact]
		public void TotalTagPresent_TotalTagMissing()
		{
            // Arrange
            var expenseClaimText = @"...
<totalDummySuffix>1024.01</total><payment_method>personal card</payment_method>";

            var expectedErrorMsgs = new List<string>
            {
                ValidationMessages.ExpenseClaimTextOpeningClosingTagsMatched,
                ValidationMessages.ExpenseClaimTextSpecifyNumericTotalValue
            };

            ActAndAssert(expenseClaimText, expectedErrorMsgs);
        }

		[Fact]
		public void TotalTagPresent_TotalIsNotXmlTag()
		{
            // Arrange
            var expenseClaimText = @"...
total>1024.01</total><payment_method>personal card</payment_method>";

            var expectedErrorMsgs = new List<string>
            {
                ValidationMessages.ExpenseClaimTextOpeningClosingTagsMatched,
                ValidationMessages.ExpenseClaimTextSpecifyNumericTotalValue
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
				ExpenseClaimText = @"...
<expense><cost_centre>DEV002</cost_centre>
<total>1024.01</total><payment_method>personal card</payment_method>
</expense>
...
Please create a reservation at the<vendor>Viaduct Steakhouse</vendor> our
<description> development team’s project end celebration dinner </description> on
<date> Tuesday 27 April 2017 </date>.We expect to arrive around 7.15pm.
    ..."
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
            var expenseClaimText = @"...
<cost_centre>DEV002</cost_centre><total>1024.01</total></expense>";

            var expectedErrorMsgs = new List<string>
            {
                ValidationMessages.ExpenseClaimTextOpeningClosingTagsMatched
            };

            ActAndAssert(expenseClaimText, expectedErrorMsgs);
        }

		[Fact]
		public void OpeningClosingTagsMatched_ClosingTagsMissing()
		{
            // Arrange
            var expenseClaimText = @"...
<expense><cost_centre>DEV002</cost_centre><total>1024.01</expense> ";

            var expectedErrorMsgs = new List<string>
            {
                ValidationMessages.ExpenseClaimTextOpeningClosingTagsMatched,
                ValidationMessages.ExpenseClaimTextSpecifyNumericTotalValue
            };

            ActAndAssert(expenseClaimText, expectedErrorMsgs);
        }

		[Fact]
		public void OpeningClosingTagsMatched_BothOpeningAndClosingTagsMissing()
		{
			// Arrange
		    var expenseClaimText = @"...
<cost_centre>DEV002</cost_centre><total>1024.01</expense>";

			var expectedErrorMsgs = new List<string>
			{
                ValidationMessages.ExpenseClaimTextOpeningClosingTagsMatched,
                ValidationMessages.ExpenseClaimTextSpecifyNumericTotalValue
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
	}
}
