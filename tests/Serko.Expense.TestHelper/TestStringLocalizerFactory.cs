using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace Serko.Expense.TestHelper
{
    public class TestStringLocalizerFactory<T>
    {
        private static IStringLocalizer<T> _localizer;

        private TestStringLocalizerFactory()
        {
        }

        public static IStringLocalizer<T> Localizer => 
            _localizer ?? (_localizer = CreateStringLocalizer());

        public static IStringLocalizer<T> CreateStringLocalizer()
        {
            var locOptions = new LocalizationOptions
            {
                ResourcesPath = "Resources"
            };
            var options = new Mock<IOptions<LocalizationOptions>>();
            options.Setup(o => o.Value).Returns(locOptions);

            var loggerFactory = NullLoggerFactory.Instance;

            var factory = new ResourceManagerStringLocalizerFactory(options.Object, loggerFactory);

            return new StringLocalizer<T>(factory);
        }
    }
}
