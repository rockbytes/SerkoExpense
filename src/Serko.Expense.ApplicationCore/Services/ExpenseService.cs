using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using FluentValidation;
using Serko.Expense.ApplicationCore.Dtos;
using Serko.Expense.ApplicationCore.Interfaces;
using Serko.Expense.ApplicationCore.Utilities;
using ValidationException = Serko.Expense.ApplicationCore.Exceptions.ValidationException;

namespace Serko.Expense.ApplicationCore.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IValidator<ExpenseClaimInput> _inputValidator;

        public ExpenseService(IValidator<ExpenseClaimInput> inputValidator)
        {
            _inputValidator = inputValidator;
        }

		public IDictionary<string, string> CreateExpenseClaimFromInput(ExpenseClaimInput input)
		{
			ValidateExpenseClaimInput(input);

			var xmlData = ExtractXmlDataFromText(input.ExpenseClaimText);

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

            // Assume tags does not duplicate in the text. If there are
		    // duplications, the first one will be used.
            var tagsAndValues = new Dictionary<string, string>();
		    foreach (Match match in matches)
		    {
                var tag = match.Groups["tag"].ToString();
		        if (!tagsAndValues.ContainsKey(tag))
		        {
		            tagsAndValues.Add(tag, match.Groups["value"].ToString().Trim());
                }    
		    }

		    return tagsAndValues;
		}

		private static IDictionary<string, string> GenerateOutputBasedOnXmlData(
			IDictionary<string, string> xmlData)
		{
		    var totalWithGst = decimal.Parse(xmlData["total"]);

			var countryCode = xmlData.ContainsKey("country_code")
				? xmlData["country_code"]
				: CountryCodes.NewZealand;

		    // Calculate the gst and the total_without_gst
            decimal totalWithoutGst = CalculateTotalWithoutGst(totalWithGst,
				GstRates.GetRate(countryCode));

			xmlData["total_without_gst"] = totalWithoutGst
				.ToString("F2", CultureInfo.InvariantCulture);

			xmlData["gst"] = (totalWithGst - totalWithoutGst)
				.ToString("F2", CultureInfo.InvariantCulture);

			// Set cost_centre to UNKNOWN if it is missing
			if (!xmlData.ContainsKey("cost_centre") ||
				string.IsNullOrEmpty(xmlData["cost_centre"]))
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
