using System.Collections.Generic;
using Moq;
using Serko.Expense.ApplicationCore.Dtos;
using Serko.Expense.ApplicationCore.Interfaces;
using Serko.Expense.ApplicationCore.Services;
using Serko.Expense.ApplicationCore.Validators;
using Serko.Expense.TestHelper;
using Xunit;
using ValidationException = Serko.Expense.ApplicationCore.Exceptions.ValidationException;

namespace Serko.Expense.UnitTests.ApplicationCore
{
	public class ExpenseServiceTests
	{
	    private readonly ExpenseService _service = GetExpenseService();

		#region CreateExpenseClaimFromInput

		[Fact]
		public void CreateExpenseClaimFromInput_ValidInputSeperateLines()
		{
            ActAndAssert(CommonData.ValidInput,
                CommonData.ValidExtractedOutput);
		}

		[Fact]
		public void CreateExpenseClaimFromInput_ValidInputSingleLine()
		{
			// Arrange
			var expenseClaimText = @"...
<expense><cost_centre>DEV002</cost_centre><total>1024.01</total><payment_method>personal card</payment_method></expense>
...
Please create a reservation at the<vendor>Viaduct Steakhouse</vendor> our
<description> development team’s project end celebration dinner </description> on
<date> Tuesday 27 April 2017 </date>.We expect to arrive around 7.15pm.
    ...";

            ActAndAssert(expenseClaimText, CommonData.ValidExtractedOutput);
        }

		[Fact]
		public void CreateExpenseClaimFromInput_ValidInputWithCostCentreMissing()
		{
            ActAndAssert(CommonData.ValidInput_CostCentreMissing, 
                CommonData.ValidExtractedOutput_Unknown);
		}

	    private void ActAndAssert(string inputText, Dictionary<string, string> expectedResult)
	    {
            var input = new ExpenseClaimInput
            {
                ExpenseClaimText = inputText
            };

            var actualResult = _service.CreateExpenseClaimFromInput(input);

            Assert.Equal(actualResult, expectedResult);
        }

		[Fact]
		public void CreateExpenseClaimFromInput_InvalidTotalValue()
		{
			// Arrange
			var input = new ExpenseClaimInput
			{
				ExpenseClaimText = @"...
<expense>
<total>1024.01abc</total><payment_method>personal card</payment_method>
</expense>..."
			};

			// Act
			var exception = Assert.Throws<ValidationException>(() =>
				_service.CreateExpenseClaimFromInput(input));

			// Assert
			Assert.True(exception.Failures.Count > 0);
		}

		[Fact]
		public void CreateExpenseClaimFromInput_TotalTagNotPresent()
		{
			// Arrange
			var input = new ExpenseClaimInput
			{
				ExpenseClaimText = @"...
<expense>
1024.01</total><payment_method>personal card</payment_method>
</expense>..."
			};

			// Act
			var exception = Assert.Throws<ValidationException>(() =>
				_service.CreateExpenseClaimFromInput(input));

			// Assert
			Assert.True(exception.Failures.Count > 0);
		}

		[Fact]
		public void CreateExpenseClaimFromInput_OpeningClosingTagsUnmatched()
		{
			// Arrange
			var input = new ExpenseClaimInput
			{
				ExpenseClaimText = @"...
<expenseXX>
<total>1024.01</total><payment_method>personal card</payment_method>
</expense>..."
			};

			// Act
			var exception = Assert.Throws<ValidationException>(() =>
				_service.CreateExpenseClaimFromInput(input));

			// Assert
			Assert.True(exception.Failures.Count > 0);
		}

		#endregion

	    private static ExpenseService GetExpenseService()
	    {
	        var loggerMock = new Mock<IAppLogger<ExpenseService>>();
	        var localizer = TestStringLocalizerFactory<ExpenseClaimInputValidator>.Localizer;

            return new ExpenseService(new ExpenseClaimInputValidator(localizer),
                loggerMock.Object);
        }
	}
}
