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
    }
}