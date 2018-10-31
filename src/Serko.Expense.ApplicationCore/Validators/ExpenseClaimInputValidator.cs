using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;
using Serko.Expense.ApplicationCore.Dtos;

namespace Serko.Expense.ApplicationCore.Validators
{
	public class ExpenseClaimInputValidator : AbstractValidator<ExpenseClaimInput>
	{
		public ExpenseClaimInputValidator()
		{
			RuleFor(x => x.ExpenseClaimText)
				.NotEmpty() // NotEmpty covers both the null and empty.
				.WithMessage(ValidationMessages.ExpenseClaimTextNotBeBlank);

			When(x => !string.IsNullOrEmpty(x.ExpenseClaimText), () =>
			{
				RuleFor(x => x.ExpenseClaimText)
					.Custom(OpeningClosingTagsMatched);

				RuleFor(x => x.ExpenseClaimText)
					.Custom(NumericTotalValuePresent);
			});
		}

		private static void NumericTotalValuePresent(string xmlText, CustomContext context)
		{
			// Assume the text block contains single <total> amount. If there are multiple ones,
			// the first will be used.

			var xmlRex = new Regex(@"\<total\>(?<value>.+)\</total\>", RegexOptions.Singleline);

			var match = xmlRex.Match(xmlText);

			if (!match.Success)
			{
				// <total> amount is missing
				context.AddFailure(ValidationMessages.TotalAmountNotPresentInExpenseClaimText);
			}
			else
			{
				// Handle invalid <total> data, e.g. <total>123.123abc</total>
				var total = match.Groups["value"].ToString();
				if (!decimal.TryParse(total.Trim(), out var dummy))
				{
					context.AddFailure(string.Format(ValidationMessages.TotalAmountShouldBeNumeric, total));
				}
			}
		}

		private static void OpeningClosingTagsMatched(string xmlText, CustomContext context)
		{
			var tagsPresent = ExtractXmlTagsPresentInText(xmlText);

			ValidateOpeningClosingTagsMatched(tagsPresent, context);
		}

		private static IEnumerable<string> ExtractXmlTagsPresentInText(string xmlText)
		{
			// Extract all the opening and closing tags appearing in the text

			var xmlRegex = new Regex(@"\<(/?\w+)\>");

			var tags = from Match match in xmlRegex.Matches(xmlText)
					   select match.Groups[1].ToString();

			return tags;
		}

		private static void ValidateOpeningClosingTagsMatched(IEnumerable<string> tags, CustomContext context)
		{
			var closingTags = new Stack<string>();

			foreach (var tag in tags.Reverse())
			{
				if (tag.StartsWith("/"))
				{
					closingTags.Push(tag);
				}
				else // tag is an opening tag
				{
					if (closingTags.Count == 0)
					{
						context.AddFailure(string.Format(
							ValidationMessages.OpeningTagXHasNoCorrespondingClosingTags, tag));

						return;
					}

					var currClosingTag = closingTags.Pop();
					if (!currClosingTag.Equals($"/{tag}"))
					{
						var msg = closingTags.Contains($"/{tag}")
							? string.Format(ValidationMessages.ClosingTagXHasNoCorrespondingOpeningTags, currClosingTag)
							: string.Format(ValidationMessages.OpeningTagXHasNoCorrespondingClosingTags, tag);

						context.AddFailure(msg);

						return;
					}
				}
			}

			if (closingTags.Count > 0)
			{
				context.AddFailure(string.Format(
					ValidationMessages.ClosingTagXHasNoCorrespondingOpeningTags, closingTags.Peek()));
			}
		}
	}
}
