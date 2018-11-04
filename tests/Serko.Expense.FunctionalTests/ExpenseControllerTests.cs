using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Serko.Expense.ApplicationCore.Validators;
using Serko.Expense.WebApi;
using Xunit;

namespace Serko.Expense.FunctionalTests
{
    public class ExpenseControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private const string ExpenseApiUri = "/api/expense";

        public ExpenseControllerTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ExtractExpenseClaim_ValidData_MediaTypeJson()
        {
            // Arrange
            var expenseClaimText = @"...
<expense><cost_centre>DEV002</cost_centre>
<total>1024.01</total><payment_method>personal card</payment_method>
</expense>
...
Please create a reservation at the<vendor>Viaduct Steakhouse</vendor> our
<description> development team’s project end celebration dinner </description> on
<date> Tuesday 27 April 2017 </date>.We expect to arrive around 7.15pm.
    ...";

            var expectedResult = new Dictionary<string, string>
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

            // Action

            // Add double quote ("") to indicate it is JSON string
            var requestContent = new StringContent($"\"{expenseClaimText}\"", 
                Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(ExpenseApiUri, requestContent);

            response.EnsureSuccessStatusCode();

            // Assert
            var rawResult = await response.Content.ReadAsStringAsync();
            var actualResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(rawResult);

            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async Task ExtractExpenseClaim_ValidData_MediaTypePlainText()
        {
            // Arrange
            var expenseClaimText = @"...
<expense><cost_centre>DEV002</cost_centre>
<total>1024.01</total><payment_method>personal card</payment_method>
</expense>
...
Please create a reservation at the<vendor>Viaduct Steakhouse</vendor> our
<description> development team’s project end celebration dinner </description> on
<date> Tuesday 27 April 2017 </date>.We expect to arrive around 7.15pm.
    ...";

            var expectedResult = new Dictionary<string, string>
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

            // Action

            var requestContent = new StringContent(expenseClaimText,
                Encoding.UTF8, "text/plain");

            var response = await _client.PostAsync(ExpenseApiUri, requestContent);

            response.EnsureSuccessStatusCode();

            // Assert
            var rawResult = await response.Content.ReadAsStringAsync();
            var actualResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(rawResult);

            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async Task ExtractExpenseClaim_ValidData_BlankMediaType()
        {
            // Arrange
            var expenseClaimText = @"...
<expense><cost_centre>DEV002</cost_centre>
<total>1024.01</total><payment_method>personal card</payment_method>
</expense>
...
Please create a reservation at the<vendor>Viaduct Steakhouse</vendor> our
<description> development team’s project end celebration dinner </description> on
<date> Tuesday 27 April 2017 </date>.We expect to arrive around 7.15pm.
    ...";

            var expectedResult = new Dictionary<string, string>
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

            // Action

            var requestContent = new StringContent(expenseClaimText, Encoding.UTF8);

            var response = await _client.PostAsync(ExpenseApiUri, requestContent);

            response.EnsureSuccessStatusCode();

            // Assert
            var rawResult = await response.Content.ReadAsStringAsync();
            var actualResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(rawResult);

            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async Task ExtractExpenseClaim_InvalidData_MediaTypePlainText()
        {
            // Arrange
            var expenseClaimText = @"...
<expense><cost_centre>DEV002</cost_centre>
<total>1024.01</total><payment_method>personal card</payment_method>
</expense>
...
Please create a reservation at the<vendor>Viaduct Steakhouse</vendorDummySuffix> our
<description> development team’s project end celebration dinner </description> on
<date> Tuesday 27 April 2017 </date>.We expect to arrive around 7.15pm.
    ...";

            var expectedErrorMsgs = new List<string>
            {
                string.Format(ValidationMessages.OpeningTagXHasNoCorrespondingClosingTags, "vendor")
            };

            // Action

            var requestContent = new StringContent(expenseClaimText,
                Encoding.UTF8, "text/plain");

            var response = await _client.PostAsync(ExpenseApiUri, requestContent);

            // Assert
            var rawResult = await response.Content.ReadAsStringAsync();
            var actualResult = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(rawResult);

            Assert.Equal(expectedErrorMsgs, actualResult["ExpenseClaimText"]);
        }

        [Fact]
        public async Task ExtractExpenseClaim_InvalidData_MediaTypeJson()
        {
            // Arrange
            var expenseClaimText = @"...
<expense><cost_centre>DEV002</cost_centre>
<total>1024.01</total><payment_method>personal card</payment_method>
</expense>
...
Please create a reservation at the<vendor>Viaduct Steakhouse</vendorDummySuffix> our
<description> development team’s project end celebration dinner </description> on
<date> Tuesday 27 April 2017 </date>.We expect to arrive around 7.15pm.
    ...";

            var expectedErrorMsgs = new List<string>
            {
                string.Format(ValidationMessages.OpeningTagXHasNoCorrespondingClosingTags, "vendor")
            };

            // Action

            // Add double quote ("") to indicate it is JSON string
            var requestContent = new StringContent($"\"{expenseClaimText}\"",
                Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(ExpenseApiUri, requestContent);

            // Assert
            var rawResult = await response.Content.ReadAsStringAsync();
            var actualResult = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(rawResult);

            Assert.Equal(expectedErrorMsgs, actualResult["ExpenseClaimText"]);
        }
    }
}
