using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSCover2Xml.Tests
{
    [TestClass]
    public class NamespaceStatisticsTests
    {
        private const int ClassCount = 4;
        private const int MethodCount = 9;

        private static NamespaceStatistics CreateNamespaceStatistics()
        {
            var namespaceStatistics = new NamespaceStatistics("Clazz");
            var methodCoverageStats = new Microsoft.VisualStudio.Coverage.Analysis.CoverageStatistics
            {
                BlocksCovered = 1, BlocksNotCovered = 2, LinesCovered = 3, LinesNotCovered = 4, LinesPartiallyCovered = 5
            };

            for (int classIdx = 1; classIdx <= ClassCount; classIdx++)
            {
                var classStats = namespaceStatistics.GetOrAddClass("Class" + classIdx);
                for (var methodIdx = 1; methodIdx <= MethodCount; methodIdx++)
                    classStats.AddMethod(new MethodStatistics((uint)methodIdx, "Method" + methodIdx, "Method" + methodIdx, methodCoverageStats));
            }

            return namespaceStatistics;
        }

        [TestMethod]
        public void CoverageIsSumOfAllClassCoverages()
        {
            // Arrange
            var namespaceStats = CreateNamespaceStatistics();
            const int baseCoverage = ClassCount * MethodCount;

            // Act
            var blocksCovered = namespaceStats.BlocksCovered;
            var blocksNotCovered = namespaceStats.BlocksNotCovered;
            var linesCovered = namespaceStats.LinesCovered;
            var linesNotCovered = namespaceStats.LinesNotCovered;
            var linesPartiallyCovered = namespaceStats.LinesPartiallyCovered;

            // Assert
            blocksCovered.Should().Be(1 * baseCoverage);
            blocksNotCovered.Should().Be(2 * baseCoverage);
            linesCovered.Should().Be(3 * baseCoverage);
            linesNotCovered.Should().Be(4 * baseCoverage);
            linesPartiallyCovered.Should().Be(5 * baseCoverage);
        }

        [TestMethod]
        public void WritesDataToXml()
        {
            // Arrange
            var namespaceStats = CreateNamespaceStatistics();

            // Act
            var xml = TestHelper.GetXml(namespaceStats.WriteXml);

            // Assert
            xml.Should().StartWith(
                "<Root><NamespaceName>Clazz</NamespaceName><BlocksCovered>36</BlocksCovered><BlocksNotCovered>72</BlocksNotCovered><LinesCovered>108</LinesCovered>" +
                "<LinesNotCovered>144</LinesNotCovered><LinesPartiallyCovered>180</LinesPartiallyCovered><Classes>");
            xml.Should().EndWith("</Classes></Root>");
            xml.Should().HaveLength(12208);
        }
    }
}