using System;
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

        public bool ValidateMatchOfOpeningClosingTags(string xmlText)
        {
            if (xmlText == null)
            {
                return true;
            }

            var xmlRegex = new Regex(@"\<(/?\w+)\>");
            var matches = xmlRegex.Matches(xmlText);

            var xmlTags = new Stack<string>();
            foreach (Match match in matches)
            {
                var tag = match.Groups[1].ToString();
                if (!tag.StartsWith("/"))
                {
                    xmlTags.Push(tag);
                }
                else
                {
                    if (xmlTags.Count == 0 ||
                        !tag.Equals($"/{xmlTags.Pop()}", StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
            }

            return xmlTags.Count == 0;
        }

        public bool ValidatePresenceOfTotalTag(string xmlText)
        {
            return xmlText != null && xmlText.Contains("<total>");
        }
    }
}
