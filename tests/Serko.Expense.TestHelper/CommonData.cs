using System.Collections.Generic;
using Serko.Expense.ApplicationCore.Validators;

namespace Serko.Expense.TestHelper
{
    public class CommonData
    {
        #region ValidInput
        public static string ValidInput =>
            @"...
<expense><cost_centre>DEV002</cost_centre>
<total>1024.01</total><payment_method>personal card</payment_method>
</expense>
...
Please create a reservation at the<vendor>Viaduct Steakhouse</vendor> our
<description> development team’s project end celebration dinner </description> on
<date> Tuesday 27 April 2017 </date>.We expect to arrive around 7.15pm.
    ...";

        public static Dictionary<string, string> ValidExtractedOutput =>
            new Dictionary<string, string>
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
        #endregion

        #region ValidInput_CostCentreMissing
        public static string ValidInput_CostCentreMissing =>
            @"...
<expense>
<total>1024.01</total><payment_method>personal card</payment_method>
</expense>
...
Please create a reservation at the<vendor>Viaduct Steakhouse</vendor> our
<description> development team’s project end celebration dinner </description> on
<date> Tuesday 27 April 2017 </date>.We expect to arrive around 7.15pm.
    ...";

        public static Dictionary<string, string> ValidExtractedOutput_Unknown =>
            new Dictionary<string, string>
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
        #endregion

        #region MissingClosingTag
        public static string InvalidInput_MissingClosingTag =>
            @"...
<expense><cost_centre>DEV002</cost_centre>
<total>1024.01</total><payment_method>personal card</payment_method>
</expense>
...
Please create a reservation at the<vendor>Viaduct Steakhouse</vendorDummySuffix> our
<description> development team’s project end celebration dinner </description> on
<date> Tuesday 27 April 2017 </date>.We expect to arrive around 7.15pm.
    ...";

        public static Dictionary<string, string[]> MissingClosingTagErrors =>
            new Dictionary<string, string[]>
            {
                ["ExpenseClaimText"] = new string[]
                {
                    TestStringLocalizerFactory<ExpenseClaimInputValidator>
                    .Localizer["OpeningTagXHasNoCorrespondingClosingTags", "vendor"]
                }
            };
        #endregion
    }
}
