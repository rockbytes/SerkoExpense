using System.Collections.Generic;
using Serko.Expense.ApplicationCore.Dtos;
using Serko.Expense.ApplicationCore.Services;
using Serko.Expense.ApplicationCore.Validators;
using Xunit;
using ValidationException = Serko.Expense.ApplicationCore.Exceptions.ValidationException;

namespace Serko.Expense.UnitTests.ApplicationCore
{
	public class ExpenseServiceTests
	{
		private readonly ExpenseService _service = 
			new ExpenseService(new ExpenseClaimInputValidator());

		#region CreateExpenseClaimFromInput

		[Fact]
		public void CreateExpenseClaimFromInput_ValidInput()
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

			var expectedResult = new Dictionary<string, string>
			{
				{"cost_centre", "DEV002"},
				{"total", "1024.01"},
				{"payment_method", "personal card"},
				{"vendor", "Viaduct Steakhouse"},
				{"description", "development team’s project end celebration dinner"},
				{"date", "Tuesday 27 April 2017"},
				{"total_without_gst", "890.44"},
				{"gst", "133.57"}
			};

			// Act
			var actualResult =_service.CreateExpenseClaimFromInput(input);

			// Assert
			Assert.Equal(actualResult, expectedResult);
		}

		[Fact]
		public void CreateExpenseClaimFromInput_ValidInputWithCostCentreMissing()
		{
			// Arrange
			var input = new ExpenseClaimInput
			{
				ExpenseClaimText = @"...
<expense>
<total>1024.01</total><payment_method>personal card</payment_method>
</expense>
...
Please create a reservation at the<vendor>Viaduct Steakhouse</vendor> our
<description> development team’s project end celebration dinner </description> on
<date> Tuesday 27 April 2017 </date>.We expect to arrive around 7.15pm.
    ..."
			};

			var expectedResult = new Dictionary<string, string>
			{
				{"cost_centre", "UNKNOWN"},
				{"total", "1024.01"},
				{"payment_method", "personal card"},
				{"vendor", "Viaduct Steakhouse"},
				{"description", "development team’s project end celebration dinner"},
				{"date", "Tuesday 27 April 2017"},
				{"total_without_gst", "890.44"},
				{"gst", "133.57"}
			};

			// Act
			var actualResult = _service.CreateExpenseClaimFromInput(input);

			// Assert
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
	}
}
