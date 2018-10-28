using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FluentValidation;
using Serko.Expense.ApplicationCore.Dtos;

namespace Serko.Expense.ApplicationCore.Validators
{
    public class ExpenseClaimInputValidator : AbstractValidator<ExpenseClaimInput>
    {
        public ExpenseClaimInputValidator()
        {
			// TODO: refactor the hard-coded messages

            RuleFor(x => x.ExpenseClaimText)
                .NotEmpty() // NotEmpty covers both the null and empty.
                .WithMessage("The expense claim text should not be blank.");

			RuleFor(x => x.ExpenseClaimText)
		        .Must(TotalTagPresent)
		        .WithMessage("The expense claim text should specify amount with <total> XML tag.");

	        RuleFor(x => x.ExpenseClaimText)
		        .Must(OpeningClosingTagsMatched)
		        .WithMessage("The expense claim text should have its opening and closing XML tags matched.");
        }

        private static bool TotalTagPresent(string xmlText)
        {
            return string.IsNullOrEmpty(xmlText) ||
                   xmlText.ToLowerInvariant().Contains("<total>");
        }

        private static bool OpeningClosingTagsMatched(string xmlText)
        {
            if (string.IsNullOrEmpty(xmlText))
            {
                return true;
            }

            var tagsPresent = ExtractXmlTagsPresentInText(xmlText);

            return ValidateOpeningClosingTagsMatched(tagsPresent);
        }

        private static IEnumerable<string> ExtractXmlTagsPresentInText(string xmlText)
        {
            var xmlRegex = new Regex(@"\<(/?\w+)\>");

            var tags = from Match match in xmlRegex.Matches(xmlText)
                       select match.Groups[1].ToString();

            return tags;
        }

        private static bool ValidateOpeningClosingTagsMatched(IEnumerable<string> tags)
        {
            var openingTags = new Stack<string>();

            foreach (var tag in tags)
            {
                if (!tag.StartsWith("/"))
                {
                    openingTags.Push(tag);
                }
                else
                {
                    if (openingTags.Count == 0 ||
                        !tag.Equals($"/{openingTags.Pop()}", StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
            }

            return openingTags.Count == 0;
        }
    }
}
