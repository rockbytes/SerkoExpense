using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Serko.Expense.ApplicationCore.Validators;
using Serko.Expense.TestHelper;
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
            // Add double quote ("") to indicate it is JSON string
            var requestContent = new StringContent(
                $"\"{CommonData.ValidInput}\"", 
                Encoding.UTF8, "application/json");

            await ActAndAssert(requestContent, CommonData.ValidExtractedOutput);
        }

        [Fact]
        public async Task ExtractExpenseClaim_ValidData_MediaTypePlainText()
        {
            var requestContent = new StringContent(CommonData.ValidInput,
                Encoding.UTF8, "text/plain");

            await ActAndAssert(requestContent, CommonData.ValidExtractedOutput);
        }

        [Fact]
        public async Task ExtractExpenseClaim_ValidData_BlankMediaType()
        {
            var requestContent = new StringContent(CommonData.ValidInput, Encoding.UTF8);
            await ActAndAssert(requestContent, CommonData.ValidExtractedOutput);
        }

        [Fact]
        public async Task ExtractExpenseClaim_InvalidData_MediaTypePlainText()
        {
            var requestContent = new StringContent(
                CommonData.InvalidInput_MissingClosingTag,
                Encoding.UTF8, "text/plain");

            await ActAndAssert(requestContent, CommonData.MissingClosingTagErrors);
        }

        [Fact]
        public async Task ExtractExpenseClaim_InvalidData_MediaTypeJson()
        {
            // Add double quote ("") to indicate it is JSON string
            var requestContent = new StringContent(
                $"\"{CommonData.InvalidInput_MissingClosingTag}\"",
                Encoding.UTF8, "application/json");

            await ActAndAssert(requestContent, CommonData.MissingClosingTagErrors);
        }

        private async Task ActAndAssert<T>(HttpContent requestContent, T expectedResult)
        {
            var response = await _client.PostAsync(ExpenseApiUri, requestContent);

            var rawResult = await response.Content.ReadAsStringAsync();
            var actualResult = JsonConvert.DeserializeObject<T>(rawResult);

            Assert.Equal(expectedResult, actualResult);
        }
    }
}
