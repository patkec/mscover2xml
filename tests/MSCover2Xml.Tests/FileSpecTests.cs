using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSCover2Xml.Tests
{
    [TestClass]
    public class FileSpecTests
    {
        [TestMethod]
        public void WritesDataToXml()
        {
            // Arrange
            var fileSpec = new FileSpec("file.ext", 1);

            // Act
            var xml = TestHelper.GetXml(fileSpec.WriteXml);

            // Assert
            xml.Should().Be("<Root><FileID>1</FileID><FileName>file.ext</FileName></Root>");
        }

        /*[TestMethod]
        public void Test()
        {
            var input =
                @"d:\Projects\_MSCover2Xml\TestResults\06373912-c0f7-4bf4-97de-6f855012daf2\Simon_SIMON-PC 2012-10-21 22_20_26.coverage";
            var path = new[] {@"d:\Projects\_MSCover2Xml\TestResults\Simon_SIMON-PC 2012-06-27 23_23_06\Out\"};
            var report = CoverageReportGenerator.Create(input, path, path);

            report.Should().NotBeNull();
        }*/
    }
}