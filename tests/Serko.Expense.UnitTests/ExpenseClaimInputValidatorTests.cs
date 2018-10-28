using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentValidation.Results;
using Serko.Expense.ApplicationCore.Dtos;
using Serko.Expense.ApplicationCore.Services;
using Serko.Expense.ApplicationCore.Validators;
using Xunit;

namespace Serko.Expense.UnitTests
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
			var input = new ExpenseClaimInput
			{
				ExpenseClaimXmlText = ""
			};

			var expectedErrorMsgs = new List<string>
			{
				@"The expense claim text should not be blank."
			};

			// Act
			var result = _inputValidator.Validate(input);

			// Assert
			Assert.Equal(expectedErrorMsgs, result.Errors.Select(e => e.ErrorMessage));
		}

		[Fact]
		public void NotNullEmpty_Null()
		{
			// Arrange
			var input = new ExpenseClaimInput
			{
				ExpenseClaimXmlText = null
			};

			var expectedErrorMsgs = new List<string>
			{
				@"The expense claim text should not be blank."
			};

			// Act
			var result = _inputValidator.Validate(input);

			// Assert
			Assert.Equal(expectedErrorMsgs, result.Errors.Select(e => e.ErrorMessage));
		}

		#endregion

		#region TotalTagPresent

		[Fact]
		public void TotalTagPresent_ValidData()
		{
			// Arrange
			var input = new ExpenseClaimInput
			{
				ExpenseClaimXmlText = @"...
<total>1024.01</total><payment_method>personal card</payment_method>"
			};

			// Act
			var result = _inputValidator.Validate(input);

			// Assert
			Assert.True(result.Errors.Count == 0);
		}

		[Fact]
		public void TotalTagPresent_WithMixedUpperLowerCase()
		{
			// Arrange
			var input = new ExpenseClaimInput
			{
				ExpenseClaimXmlText = @"...
<tOtAl>1024.01</total><payment_method>personal card</payment_method>"
			};

			// Act
			var result = _inputValidator.Validate(input);

			// Assert
			Assert.True(result.Errors.Count == 0);
		}

		[Fact]
		public void TotalTagPresent_TotalTagMissing()
		{
			// Arrange
			var input = new ExpenseClaimInput
			{
				ExpenseClaimXmlText = @"...
<totalDummySuffix>1024.01</total><payment_method>personal card</payment_method>"
			};

			var expectedErrorMsgs = new List<string>
			{
				@"The expense claim text should specify amount with <total> XML tag.",
				@"The expense claim text should have its opening and closing XML tags matched."
			};

			// Act
			var result = _inputValidator.Validate(input);

			// Assert
			Assert.Equal(expectedErrorMsgs, result.Errors.Select(e => e.ErrorMessage));
		}

		[Fact]
		public void TotalTagPresent_TotalIsNotXmlTag()
		{
			// Arrange
			var input = new ExpenseClaimInput
			{
				ExpenseClaimXmlText = @"...
total>1024.01</total><payment_method>personal card</payment_method>"
			};

			var expectedErrorMsgs = new List<string>
			{
				@"The expense claim text should specify amount with <total> XML tag.",
				@"The expense claim text should have its opening and closing XML tags matched."
			};

			// Act
			var result = _inputValidator.Validate(input);

			// Assert
			Assert.Equal(expectedErrorMsgs, result.Errors.Select(e => e.ErrorMessage));
		}

		#endregion

		#region OpeningClosingTagsMatched

		[Fact]
		public void OpeningClosingTagsMatched_Matched()
		{
			// Arrange
			var input = new ExpenseClaimInput
			{
				ExpenseClaimXmlText = @"...
<expense><cost_centre>DEV002</cost_centre>
<TOtal>1024.01</total><payment_method>personal card</payment_method>
</expense>
...
Please create a reservation at the<vendor>Viaduct Steakhouse</VENDOR> our
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
			var input = new ExpenseClaimInput
			{
				ExpenseClaimXmlText = @"...
<cost_centre>DEV002</cost_centre><total>1024.01</total></expense>"
			};

			var expectedErrorMsgs = new List<string>
			{
				@"The expense claim text should have its opening and closing XML tags matched."
			};

			// Act
			var result = _inputValidator.Validate(input);

			// Assert
			Assert.Equal(expectedErrorMsgs, result.Errors.Select(e => e.ErrorMessage));
		}

		[Fact]
		public void OpeningClosingTagsMatched_ClosingTagsMissing()
		{
			// Arrange
			var input = new ExpenseClaimInput
			{
				ExpenseClaimXmlText = @"...
<expense><cost_centre>DEV002</cost_centre><total>1024.01</expense>"
			};

			var expectedErrorMsgs = new List<string>
			{
				@"The expense claim text should have its opening and closing XML tags matched."
			};

			// Act
			var result = _inputValidator.Validate(input);

			// Assert
			Assert.Equal(expectedErrorMsgs, result.Errors.Select(e => e.ErrorMessage));
		}

		[Fact]
		public void OpeningClosingTagsMatched_BothOpeningAndClosingTagsMissing()
		{
			// Arrange
			var input = new ExpenseClaimInput
			{
				ExpenseClaimXmlText = @"...
<cost_centre>DEV002</cost_centre><total>1024.01</expense>"
			};

			var expectedErrorMsgs = new List<string>
			{
				@"The expense claim text should have its opening and closing XML tags matched."
			};

			// Act
			var result = _inputValidator.Validate(input);

			// Assert
			Assert.Equal(expectedErrorMsgs, result.Errors.Select(e => e.ErrorMessage));
		}

		#endregion
	}
}
