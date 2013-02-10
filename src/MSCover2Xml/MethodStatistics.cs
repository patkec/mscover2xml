using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.Coverage.Analysis;
using VSCoverageStatistics = Microsoft.VisualStudio.Coverage.Analysis.CoverageStatistics;

namespace MSCover2Xml
{
    /// <summary>
    /// Represents code coverage statistics for a single method.
    /// </summary>
    [Serializable]
    public sealed class MethodStatistics : CoverageStatistics
    {
        private readonly long _blocksCovered;
        private readonly long _blocksNotCovered;
        private readonly long _linesCovered;
        private readonly long _linesNotCovered;
        private readonly long _linesPartiallyCovered;
        private readonly IList<BlockCoverage> _blocks = new List<BlockCoverage>();

        /// <summary>
        /// Gets the method identification number.
        /// </summary>
        public long MethodId { get; private set; }
        /// <summary>
        /// Gets the method name.
        /// </summary>
        public string MethodName { get; private set; }
        /// <summary>
        /// Gets the method full name.
        /// </summary>
        public string MethodFullName { get; private set; }
        /// <summary>
        /// Gets a list of coverage information for separate blocks in the method.
        /// </summary>
        public IEnumerable<BlockCoverage> Blocks { get { return _blocks; } }
        /// <summary>
        /// Gets or sets a number of blocks covered by tests.
        /// </summary>
        public override long BlocksCovered { get { return _blocksCovered; } }
        /// <summary>
        /// Gets or sets a number of blocks not covered by tests.
        /// </summary>
        public override long BlocksNotCovered { get { return _blocksNotCovered; } }
        /// <summary>
        /// Gets or sets a number of lines fully covered by tests.
        /// </summary>
        public override long LinesCovered { get { return _linesCovered; } }
        /// <summary>
        /// Gets or sets a number of lines not covered by tests.
        /// </summary>
        public override long LinesNotCovered { get { return _linesNotCovered; } }
        /// <summary>
        /// Gets or sets a number of lines partially covered by tests.
        /// </summary>
        public override long LinesPartiallyCovered { get { return _linesPartiallyCovered; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodStatistics"/> class.
        /// </summary>
        /// <param name="methodId">Method identifier.</param>
        /// <param name="name">Method name.</param>
        /// <param name="fullName">Method full name.</param>
        /// <param name="statistics"><see cref="VSCoverageStatistics"/> information that will be used to update method statistics.</param>
        internal MethodStatistics(uint methodId, string name, string fullName, VSCoverageStatistics statistics)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(fullName)) throw new ArgumentNullException("fullName");
            if (statistics == null) throw new ArgumentNullException("statistics");

            MethodId = methodId;
            MethodName = name;
            MethodFullName = fullName;
            _blocksCovered = statistics.BlocksCovered;
            _blocksNotCovered = statistics.BlocksNotCovered;
            _linesCovered = statistics.LinesCovered;
            _linesNotCovered = statistics.LinesNotCovered;
            _linesPartiallyCovered = statistics.LinesPartiallyCovered;
        }

        /// <summary>
        /// Creates a method coverage statistics based on given coverage buffer.
        /// </summary>
        /// <param name="id">Identification assigned to the method.</param>
        /// <param name="name">Method name.</param>
        /// <param name="fullName">Method full name.</param>
        /// <param name="coverageBuffer">Coverage buffer.</param>
        /// <param name="lines">Lines representing the method.</param>
        /// <param name="files">List to which instrumented files are added.</param>
        /// <returns>New <see cref="MethodStatistics"/> instance.</returns>
        internal static MethodStatistics Create(uint id, string name, string fullName, byte[] coverageBuffer, IList<BlockLineRange> lines, FileSpecList files)
        {
            var coverageStats = CoverageInfo.GetMethodStatistics(coverageBuffer, lines);
            var methodStats = new MethodStatistics(id, name, fullName, coverageStats);

            foreach (var block in BlockCoverage.CreateForMethod(coverageBuffer, lines, files))
                methodStats.AddBlock(block);

            return methodStats;
        }

        /// <summary>
        /// Adds block coverage information for the method.
        /// </summary>
        /// <param name="block"><see cref="BlockCoverage"/> representing one or more block coverage information for the method.</param>
        internal void AddBlock(BlockCoverage block)
        {
            if (block == null) throw new ArgumentNullException("block");

            if (!_blocks.Contains(block))
                _blocks.Add(block);
        }

        /// <summary>
        /// Writes the <see cref="MethodStatistics"/> content to the specified xml writer.
        /// </summary>
        /// <param name="xmlWriter"><see cref="XmlWriter"/> to which to write the content.</param>
        public void WriteXml(XmlWriter xmlWriter)
        {
            if (xmlWriter == null) throw new ArgumentNullException("xmlWriter");

            xmlWriter.WriteElementString("MethodID", MethodId.ToString());
            xmlWriter.WriteElementString("MethodName", MethodName);
            xmlWriter.WriteElementString("MethodFullName", MethodFullName);
            WriteCoverageToXml(xmlWriter);

            xmlWriter.WriteStartElement("Lines");
            foreach (var line in Blocks)
            {
                xmlWriter.WriteStartElement("Line");
                line.WriteXml(xmlWriter);
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} ({1})", MethodFullName, MethodId);
        }
    }
}