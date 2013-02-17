using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VSCoverageStatistics = Microsoft.VisualStudio.Coverage.Analysis.CoverageStatistics;

namespace MSCover2Xml.Tests
{
    [TestClass]
    public class ClassStatisticsTests
    {
        private static ClassStatistics CreateClassStatistics()
        {
            var classStats = new ClassStatistics(new NamespaceStatistics(new ModuleStatistics("module", Guid.Empty, 0, 0), "ns"), "Clazz");
            foreach (var methodStats in Enumerable.Range(1, 10)
                .Select(x => new MethodStatistics((uint)x, "Method " + x, "Method " + x, new VSCoverageStatistics
                {
                    BlocksCovered = (uint)x,
                    BlocksNotCovered = (uint)x - 1,
                    LinesCovered = (uint)x,
                    LinesNotCovered = (uint)x - 1,
                    LinesPartiallyCovered = (uint)x
                })))
                classStats.AddMethod(methodStats);

            return classStats;
        }

        [TestMethod]
        public void CoverageIsSumOfAllMethodCoverages()
        {
            // Arrange
            var classStats = CreateClassStatistics();

            // Act
            var blocksCovered = classStats.BlocksCovered;
            var blocksNotCovered = classStats.BlocksNotCovered;
            var linesCovered = classStats.LinesCovered;
            var linesNotCovered = classStats.LinesNotCovered;
            var linesPartiallyCovered = classStats.LinesPartiallyCovered;

            // Assert
            blocksCovered.Should().Be(55);
            blocksNotCovered.Should().Be(45);
            linesCovered.Should().Be(55);
            linesNotCovered.Should().Be(45);
            linesPartiallyCovered.Should().Be(55);
        }

        [TestMethod]
        public void WritesDataToXml()
         {
             // Arrange
             var classStats = CreateClassStatistics();

            // Act
            var xml = TestHelper.GetXml(classStats.WriteXml);

            // Assert
            xml.Should().StartWith(
                "<Root><ClassKeyName>module00000000-0000-0000-0000-000000000000nsClazz</ClassKeyName><ClassName>Clazz</ClassName>" +
                "<NamespaceKeyName>module00000000-0000-0000-0000-000000000000ns</NamespaceKeyName><LinesCovered>55</LinesCovered><LinesPartiallyCovered>55</LinesPartiallyCovered>");
            xml.Should().EndWith("</Method></Root>");
            xml.Should().HaveLength(4056);
         }
    }
}