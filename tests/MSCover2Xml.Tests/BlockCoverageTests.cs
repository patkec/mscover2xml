using Microsoft.VisualStudio.Coverage.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace MSCover2Xml.Tests
{
    [TestClass]
    public class BlockCoverageTests
    {
        [TestMethod]
        public void WritesDataToXml()
        {
            // Arrange
            var blockCoverage = new BlockCoverage(new FileSpec("dummy", 1), 1, 1, 1, 255, CoverageStatus.Covered);

            // Act
            string xml = TestHelper.GetXml(blockCoverage.WriteXml);

            // Assert
            xml.Should().Be("<Root><LineStart>1</LineStart><LineEnd>1</LineEnd><ColumnStart>1</ColumnStart><ColumnEnd>255</ColumnEnd><Coverage>0</Coverage><FileID>1</FileID></Root>");
        }
    }
}
