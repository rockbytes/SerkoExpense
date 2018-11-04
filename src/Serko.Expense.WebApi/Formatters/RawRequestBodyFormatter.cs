using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace Serko.Expense.WebApi.Formatters
{
    public class RawRequestBodyFormatter : InputFormatter
    {
        public RawRequestBodyFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
        }

        public override bool CanRead(InputFormatterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var contentType = context.HttpContext.Request.ContentType;

            // Allow no content type or text/plain
            return string.IsNullOrWhiteSpace(contentType) ||
                   contentType.Contains("text/plain");
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(
            InputFormatterContext context)
        {
            var request = context.HttpContext.Request;

            using (var reader = new StreamReader(request.Body))
            {
                var content = await reader.ReadToEndAsync();
                return await InputFormatterResult.SuccessAsync(content);
            }
        }
    }
}
