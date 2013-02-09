using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Coverage.Analysis;

namespace MSCover2Xml
{
    /// <summary>
    /// Provides methods for generating a <see cref="CoverageReport"/> from a Microsoft coverage information.
    /// </summary>
    public static class CoverageReportGenerator
    {
        /// <summary>
        /// Creates a new code coverage report from a specified code coverage binary file.
        /// </summary>
        /// <param name="coverageFile">Path to the code coverage binary file.</param>
        /// <param name="executablePaths">List of executable paths.</param>
        /// <param name="symbolPaths">List of symbol paths.</param>
        /// <returns>A new instance of <see cref="CoverageReport"/> class.</returns>
        public static CoverageReport Create(string coverageFile, IEnumerable<string> executablePaths, IEnumerable<string> symbolPaths)
        {
            if (string.IsNullOrEmpty(coverageFile)) throw new ArgumentNullException("coverageFile");

            using (var coverageInfo = CoverageInfo.CreateFromFile(coverageFile, executablePaths, symbolPaths))
            {
                //coverageInfo.BuildDataSet().WriteXml(@"d:\Projects\mscover2xml\MSCover2Xml\TestResults\06373912-c0f7-4bf4-97de-6f855012daf2\ds.xml");
                return Create(coverageInfo);
            }
        }

        /// <summary>
        /// Creates a new code coverage report from a specified code coverage information.
        /// </summary>
        /// <param name="coverageInfo">Code coverage information for which to create a report.</param>
        /// <returns>A new instance of <see cref="CoverageReport"/> class.</returns>
        public static CoverageReport Create(CoverageInfo coverageInfo)
        {
            if (coverageInfo == null) throw new ArgumentNullException("coverageInfo");

            var report = new CoverageReport();
            foreach (var module in coverageInfo.Modules)
                AddModuleStatistics(report, module);

            return report;
        }

        private static void AddModuleStatistics(CoverageReport report, ICoverageModule module)
        {
            var moduleStats = new ModuleStatistics(module.Name, module.ImageSize, module.ImageLinkTime);
            report.AddModule(moduleStats);

            var lines = new List<BlockLineRange>();
            var coverageBuffer = module.GetCoverageBuffer(null /* tests */);
            using (var symbolReader = module.Symbols.CreateReader())
            {
                uint methodId;
                string methodName;
                string undecoratedMethodName;
                string className;
                string namespaceName;

                while (symbolReader.GetNextMethod(out methodId, out methodName, out undecoratedMethodName, out className, out namespaceName, lines))
                {
                    var namespaceStats = moduleStats.GetOrAddNamespace(namespaceName);
                    var classStats = namespaceStats.GetOrAddClass(className);

                    var coverageStats = CoverageInfo.GetMethodStatistics(coverageBuffer, lines);
                    var methodStats = new MethodStatistics(methodId, methodName, undecoratedMethodName, coverageStats);
                    classStats.AddMethod(methodStats);

                    // First generate block coverage map based on the coverage buffer ...
                    var blockCoverageMap = GetBlockCoverageMap(coverageBuffer, lines);
                    // Then add coverage information for all lines to the method.
                    AddLinesAndCoverage(report, methodStats, blockCoverageMap, lines);
                    // Clear the method list to be ready for next method.
                    lines.Clear();
                }
            }
        }

        private static void AddLinesAndCoverage(CoverageReport report, MethodStatistics methodStats, IDictionary<BlockLineRange, CoverageStatus> lineCoverage, IEnumerable<BlockLineRange> lines)
        {
            foreach (var line in lines.Where(line => line.IsValid))
            {
                var file = report.GetOrAddFile(line.SourceFile);
                methodStats.AddBlock(new BlockCoverage(file, line.StartLine, line.EndLine, line.StartColumn, line.EndColumn, lineCoverage[line]));
            }
        }

        private static IDictionary<BlockLineRange, CoverageStatus> GetBlockCoverageMap(byte[] coverageBuffer, IReadOnlyCollection<BlockLineRange> lines)
        {
            var lineCoverage = new Dictionary<BlockLineRange, CoverageStatus>(lines.Count);
            foreach (var line in lines.Where(line => line.IsValid))
            {
                CoverageStatus currentLineStatus;
                // Each line is fully covered only if all the blocks in the line are covered, otherwise it is either partially covered or not covered.
                var blockStatus = (int)coverageBuffer[line.BlockIndex] == 0 ? CoverageStatus.NotCovered : CoverageStatus.Covered;
                lineCoverage[line] = !lineCoverage.TryGetValue(line, out currentLineStatus)
                                         ? blockStatus // If there is no info about coverage status for the line as of yet, then just use block status
                                         : (currentLineStatus == blockStatus)
                                               ? currentLineStatus // If previous blocks are all covered or not covered then status for the whole line is the same.
                                               : CoverageStatus.PartiallyCovered; // If some blocks are covered and some not then line is partially covered.
            }
            return lineCoverage;
        }
    }
}