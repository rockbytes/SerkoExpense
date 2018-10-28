using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FluentValidation.Results;
using Serko.Expense.ApplicationCore.Dtos;
using Serko.Expense.ApplicationCore.Exceptions;
using Serko.Expense.ApplicationCore.Interfaces;
using Serko.Expense.ApplicationCore.Utilities;
using Serko.Expense.ApplicationCore.Validators;

namespace Serko.Expense.ApplicationCore.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly ExpenseClaimInputValidator _inputValidator;

        public ExpenseService()
        {
            // TODO: use dependency injection
            _inputValidator = new ExpenseClaimInputValidator();
        }

		public IDictionary<string, string> CreateExpenseClaimFromInput(ExpenseClaimInput input)
		{
			ValidateExpenseClaimInput(input);

			IDictionary<string, string> xmlData =
				ExtractXmlDataFromText(input.ExpenseClaimText);

			return GenerateOutputBasedOnXmlData(xmlData);
		}

	    private void ValidateExpenseClaimInput(ExpenseClaimInput input)
	    {
		    var result = _inputValidator.Validate(input);
		    if (!result.IsValid)
		    {
			    throw new ValidationException(result.Errors);
		    }
	    }

		private static IDictionary<string, string> ExtractXmlDataFromText(string xmlText)
		{
			var xmlRegex = new Regex(@"\<(?<tag>\w+)\>(?<value>.+)\</\k<tag>\>");
			var matches = xmlRegex.Matches(xmlText);

			// Assume tags does not duplicate in the text.
			var tagsAndValues = matches.Cast<Match>().ToDictionary(
				match => match.Groups["tag"].ToString(), // Key, e.g. "total" 
				match => match.Groups["value"].ToString().Trim()); // Value, e.g. "1024.01"

			return tagsAndValues;
		}

		private IDictionary<string, string> GenerateOutputBasedOnXmlData(
			IDictionary<string, string> xmlData)
		{
			// Parse total with gst
			if (!decimal.TryParse(xmlData["total"], out var totalWithGst))
			{
				throw new ValidationException("Total",
					$"The value '{xmlData["total"]}' should numeric.");
			}

			// Calculate the gst and the total_without_gst
			var countryCode = xmlData.ContainsKey("country_code")
				? xmlData["country_code"]
				: CountryCodes.NewZealand;

			decimal totalWithoutGst = CalculateTotalWithoutGst(totalWithGst,
				GstRates.GetRate(countryCode));

			xmlData["total_without_gst"] = totalWithoutGst
				.ToString("F2", CultureInfo.InvariantCulture);

			xmlData["gst"] = (totalWithGst - totalWithoutGst)
				.ToString("F2", CultureInfo.InvariantCulture);

			// Set cost_centre to UNKNOWN if it is missing
			if (!xmlData.ContainsKey("cost_centre"))
			{
				xmlData["cost_centre"] = "UNKNOWN";
			}

			return xmlData;
		}

		private static decimal CalculateTotalWithoutGst(decimal totalWithGst, decimal gstRate)
        {
            decimal totalWithoutGst = totalWithGst / (gstRate + 1);
            return decimal.Round(totalWithoutGst, 2);
        }
    }
}
