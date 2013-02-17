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
            xml.Should().Be("<Root><SourceFileID>1</SourceFileID><SourceFileName>file.ext</SourceFileName></Root>");
        }
    }
}