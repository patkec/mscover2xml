using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VSCoverageStatistics = Microsoft.VisualStudio.Coverage.Analysis.CoverageStatistics;

namespace MSCover2Xml.Tests
{
    [TestClass]
    public class MethodStatisticsTests
    {
        [TestMethod]
        public void CoverageEqualsToStats()
        {
            // Arrange
            var methodStats = new MethodStatistics(1, "Method", "Method", new VSCoverageStatistics
            {
                BlocksCovered = 1, BlocksNotCovered = 2, LinesCovered = 3, LinesNotCovered = 4, LinesPartiallyCovered = 5
            });

            // Act
            var blocksCovered = methodStats.BlocksCovered;
            var blocksNotCovered = methodStats.BlocksNotCovered;
            var linesCovered = methodStats.LinesCovered;
            var linesNotCovered = methodStats.LinesNotCovered;
            var linesPartiallyCovered = methodStats.LinesPartiallyCovered;

            // Assert
            blocksCovered.Should().Be(1);
            blocksNotCovered.Should().Be(2);
            linesCovered.Should().Be(3);
            linesNotCovered.Should().Be(4);
            linesPartiallyCovered.Should().Be(5);
        }

        [TestMethod]
        public void WritesDataToXml()
        {
            // Arrange
            var methodStats = new MethodStatistics(1, "Method", "Method", new VSCoverageStatistics
            {
                BlocksCovered = 1, BlocksNotCovered = 2, LinesCovered = 3, LinesNotCovered = 4, LinesPartiallyCovered = 5
            });

            // Act
            var xml = TestHelper.GetXml(methodStats.WriteXml);

            // Assert
            xml.Should().Be(
                "<Root><MethodID>1</MethodID><MethodName>Method</MethodName><MethodFullName>Method</MethodFullName><BlocksCovered>1</BlocksCovered>" +
                "<BlocksNotCovered>2</BlocksNotCovered><LinesCovered>3</LinesCovered><LinesNotCovered>4</LinesNotCovered><LinesPartiallyCovered>5</LinesPartiallyCovered><Lines /></Root>");
        }
    }
}