using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
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
                    .Must(OpeningClosingTagsMatched)
                    .WithMessage(ValidationMessages.ExpenseClaimTextOpeningClosingTagsMatched);

                RuleFor(x => x.ExpenseClaimText)
                    .Must(NumericTotalValuePresent)
                    .WithMessage(ValidationMessages.ExpenseClaimTextSpecifyNumericTotalValue);
            });
        }

        private static bool NumericTotalValuePresent(string xmlText)
        {
            var xmlRex = new Regex(@"\<total\>(?<value>.+)\</total\>", RegexOptions.Singleline);

            var match = xmlRex.Match(xmlText);

            return match.Success && 
                decimal.TryParse(match.Groups["value"].ToString().Trim(), out var dummy);
        }

        private static bool OpeningClosingTagsMatched(string xmlText)
        {
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
                    if (openingTags.Count == 0 || !tag.Equals($"/{openingTags.Pop()}"))
                    {
                        return false;
                    }
                }
            }

            return openingTags.Count == 0;
        }
    }
}
