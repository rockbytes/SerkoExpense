using System.Collections.Generic;
using Serko.Expense.ApplicationCore.Services;
using Xunit;

namespace Serko.Expense.UnitTests
{
    public class XmlExpenseTextProcessorTests
    {
//        [Fact]
//        public void ExtractXmlContentFromText_ValidInput()
//        {
//            var textMixedWithXml = @"...
//<expense><cost_centre>DEV002</cost_centre>
//<total>1024.01</total><payment_method>personal card</payment_method>
//</expense>
//...
//Please create a reservation at the <vendor>Viaduct Steakhouse</vendor> our
//<description>development team’s project end celebration dinner</description> on
//<date>Tuesday 27 April 2017</date>. We expect to arrive around 7.15pm.
//...";

//            var expectedResult = new List<string>
//            {
//                @"<expense><cost_centre>DEV002</cost_centre>
//<total>1024.01</total><payment_method>personal card</payment_method>
//</expense>",
//                @"<vendor>Viaduct Steakhouse</vendor>",
//                @"<description>development team’s project end celebration dinner</description>",
//                @"<date>Tuesday 27 April 2017</date>"
//            };

//            var processor = new XmlExpenseTextProcessor();
//            var actualResult = processor.ExtractXmlContentFromText(textMixedWithXml);

//            Assert.Equal(expectedResult, actualResult);
//        }

        #region ValidatePresenceOfTotalTag

        [Fact]
        public void ValidatePresenceOfTotalTag_ValidData()
        {
            // Arrange
            var textMixedWithXml = @"...
<total>1024.01</total><payment_method>personal card</payment_method>";

            // Act
            var processor = new XmlExpenseTextValidator();
            var actualResult = processor.ValidatePresenceOfTotalTag(textMixedWithXml);

            // Assert
            Assert.True(actualResult);
        }

        [Fact]
        public void ValidatePresenceOfTotalTag_TotalTagMissing()
        {
            // Arrange
            var textMixedWithXml = @"...
<totalDummySuffix>1024.01</total><payment_method>personal card</payment_method>";

            // Act
            var processor = new XmlExpenseTextValidator();
            var actualResult = processor.ValidatePresenceOfTotalTag(textMixedWithXml);

            // Assert
            Assert.False(actualResult);
        }

        [Fact]
        public void ValidatePresenceOfTotalTag_TotalIsNotXmlTag()
        {
            // Arrange
            var textMixedWithXml = @"...
total>1024.01</total><payment_method>personal card</payment_method>";

            // Act
            var processor = new XmlExpenseTextValidator();
            var actualResult = processor.ValidatePresenceOfTotalTag(textMixedWithXml);

            // Assert
            Assert.False(actualResult);
        }

        [Fact]
        public void ValidatePresenceOfTotalTag_EmptyInput()
        {
            // Arrange
            var textMixedWithXml = "";

            // Act
            var processor = new XmlExpenseTextValidator();
            var actualResult = processor.ValidatePresenceOfTotalTag(textMixedWithXml);

            // Assert
            Assert.False(actualResult);
        }

        [Fact]
        public void ValidatePresenceOfTotalTag_NullInput()
        {
            // Arrange
            string textMixedWithXml = null;

            // Act
            var processor = new XmlExpenseTextValidator();
            var actualResult = processor.ValidatePresenceOfTotalTag(textMixedWithXml);

            // Assert
            Assert.False(actualResult);
        }

        #endregion

        #region ValidateMatchOfOpeningClosingTags

        [Fact]
        public void ValidateMatchOfOpeningClosingTags_OpeningClosingMatched()
        {
            // Arrange
            var textMixedWithXml = @"...
<expense><cost_centre>DEV002</cost_centre>
<total>1024.01</total><payment_method>personal card</payment_method>
</expense>
...
Please create a reservation at the<vendor>Viaduct Steakhouse</vendor> our
<description> development team’s project end celebration dinner </description> on
<date> Tuesday 27 April 2017 </date>.We expect to arrive around 7.15pm.
    ...";

            // Act
            var processor = new XmlExpenseTextValidator();
            var actualResult = processor.ValidateMatchOfOpeningClosingTags(textMixedWithXml);

            // Assert
            Assert.True(actualResult);
        }

        [Fact]
        public void ValidateMatchOfOpeningClosingTags_MatchedAsNoTags()
        {
            // Arrange
            var textMixedWithXml = @"...We expect to arrive around 7.15pm.";

            // Act
            var processor = new XmlExpenseTextValidator();
            var actualResult = processor.ValidateMatchOfOpeningClosingTags(textMixedWithXml);

            // Assert
            Assert.True(actualResult);
        }

        [Fact]
        public void ValidateMatchOfOpeningClosingTags_MatchedAsEmptyInput()
        {
            // Arrange
            var textMixedWithXml = "";

            // Act
            var processor = new XmlExpenseTextValidator();
            var actualResult = processor.ValidateMatchOfOpeningClosingTags(textMixedWithXml);

            // Assert
            Assert.True(actualResult);
        }

        [Fact]
        public void ValidateMatchOfOpeningClosingTags_MatchedAsNullInput()
        {
            // Arrange
            string textMixedWithXml = null;

            // Act
            var processor = new XmlExpenseTextValidator();
            var actualResult = processor.ValidateMatchOfOpeningClosingTags(textMixedWithXml);

            // Assert
            Assert.True(actualResult);
        }

        [Fact]
        public void ValidateMatchOfOpeningClosingTags_OpeningTagsMissing()
        {
            // Arrange
            var textMixedWithXml = @"...
<cost_centre>DEV002</cost_centre><total>1024.01</total></expense>";

            // Act
            var processor = new XmlExpenseTextValidator();
            var actualResult = processor.ValidateMatchOfOpeningClosingTags(textMixedWithXml);

            // Assert
            Assert.False(actualResult);
        }

        [Fact]
        public void ValidateMatchOfOpeningClosingTags_ClosingTagsMissing()
        {
            // Arrange
            var textMixedWithXml = @"...
<expense><cost_centre>DEV002</cost_centre><total>1024.01</expense>";

            // Act
            var processor = new XmlExpenseTextValidator();
            var actualResult = processor.ValidateMatchOfOpeningClosingTags(textMixedWithXml);

            // Assert
            Assert.False(actualResult);
        }

        [Fact]
        public void ValidateMatchOfOpeningClosingTags_BothOpeningAndClosingTagsMissing()
        {
            // Arrange
            var textMixedWithXml = @"...
<cost_centre>DEV002</cost_centre><total>1024.01</expense>";

            // Act
            var processor = new XmlExpenseTextValidator();
            var actualResult = processor.ValidateMatchOfOpeningClosingTags(textMixedWithXml);

            // Assert
            Assert.False(actualResult);
        }

        #endregion

    }
}
