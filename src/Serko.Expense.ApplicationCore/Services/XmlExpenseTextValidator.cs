using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Serko.Expense.ApplicationCore.Interfaces;

namespace Serko.Expense.ApplicationCore.Services
{
    public class XmlExpenseTextValidator : IExpenseTextValidator
    {
        public bool Validate(string expenseText)
        {
            throw new NotImplementedException();
        }

        public bool ValidatePresenceOfTotalTag(string xmlText)
        {
            return xmlText != null && xmlText.Contains("<total>");
        }

        public bool ValidateMatchOfOpeningClosingTags(string xmlText)
        {
            if (xmlText == null)
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
