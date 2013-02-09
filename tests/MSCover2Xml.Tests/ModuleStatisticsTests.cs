using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSCover2Xml.Tests
{
    [TestClass]
    public class ModuleStatisticsTests
    {
        private const int ClassCount = 4;
        private const int MethodCount = 9;
        private const int NamespaceCount = 2;

        private static ModuleStatistics CreateModuleStatistics()
        {
            var moduleStats = new ModuleStatistics("Module", 50 /* imageSize */, 4 /* linkTime */);
            var methodCoverageStats = new Microsoft.VisualStudio.Coverage.Analysis.CoverageStatistics
            {
                BlocksCovered = 1, BlocksNotCovered = 2, LinesCovered = 3, LinesNotCovered = 4, LinesPartiallyCovered = 5
            };

            for (int namespaceIdx = 1; namespaceIdx <= NamespaceCount; namespaceIdx++)
            {
                var nsStats = moduleStats.GetOrAddNamespace("Namespace" + namespaceIdx);
                for (int classIdx = 1; classIdx <= ClassCount; classIdx++)
                {
                    var classStats = nsStats.GetOrAddClass("Class" + classIdx);
                    for (var methodIdx = 1; methodIdx <= MethodCount; methodIdx++)
                        classStats.AddMethod(new MethodStatistics((uint)methodIdx, "Method" + methodIdx, "Method" + methodIdx, methodCoverageStats));
                }
            }
            return moduleStats;
        }

        [TestMethod]
        public void CoverageIsSumOfAllNamespaceCoverages()
        {
            // Arrange
            var moduleStats = CreateModuleStatistics();
            const int baseCoverage = ClassCount*MethodCount*NamespaceCount;

            // Act
            var blocksCovered = moduleStats.BlocksCovered;
            var blocksNotCovered = moduleStats.BlocksNotCovered;
            var linesCovered = moduleStats.LinesCovered;
            var linesNotCovered = moduleStats.LinesNotCovered;
            var linesPartiallyCovered = moduleStats.LinesPartiallyCovered;

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
            var moduleStats = CreateModuleStatistics();

            // Act
            var xml = TestHelper.GetXml(moduleStats.WriteXml);

            // Assert
            xml.Should().StartWith(
                "<Root><ModuleName>Module</ModuleName><ImageSize>50</ImageSize><ImageLinkTime>1969-12-31T16:00:04.0000000</ImageLinkTime><BlocksCovered>72</BlocksCovered>" +
                "<BlocksNotCovered>144</BlocksNotCovered><LinesCovered>216</LinesCovered><LinesNotCovered>288</LinesNotCovered><LinesPartiallyCovered>360</LinesPartiallyCovered><Namespaces>");
            xml.Should().EndWith("</Namespaces></Root>");
            xml.Should().HaveLength(24791);
        }
    }
}