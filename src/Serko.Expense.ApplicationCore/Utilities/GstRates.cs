using System.Collections.Generic;

namespace Serko.Expense.ApplicationCore.Utilities
{
	public class GstRates
	{
		private static readonly IDictionary<string, decimal> GstRatesByCountry =
			new Dictionary<string, decimal>
			{
				[CountryCodes.NewZealand] = 0.15m,
				[CountryCodes.Australia] = 0.1m
			};

		// TODO: inform users if gst rate does not exist, instead of returning NZ's

		public static decimal GetRate(string countryCode) =>
			countryCode != null && GstRatesByCountry.ContainsKey(countryCode)
				? GstRatesByCountry[countryCode]
				: GstRatesByCountry[CountryCodes.NewZealand];
	}
}
