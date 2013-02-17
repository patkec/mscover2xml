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
            xml.Should().Be("<Root><LnStart>1</LnStart><ColStart>1</ColStart><LnEnd>1</LnEnd><ColEnd>255</ColEnd><Coverage>0</Coverage><SourceFileID>1</SourceFileID><LineID>0</LineID></Root>");
        }
    }
}
