using System;
using System.Globalization;
using System.Xml;

namespace MSCover2Xml
{
    /// <summary>
    /// Represents code coverage statistics information for an instrumented object.
    /// </summary>
    public abstract class CoverageStatistics
    {
        /// <summary>
        /// Gets a number of blocks covered by tests.
        /// </summary>
        public abstract long BlocksCovered { get; }
        /// <summary>
        /// Gets a number of blocks not covered by tests.
        /// </summary>
        public abstract long BlocksNotCovered { get; }
        /// <summary>
        /// Gets a number of lines fully covered by tests.
        /// </summary>
        public abstract long LinesCovered { get; }
        /// <summary>
        /// Gets a number of lines not covered by tests.
        /// </summary>
        public abstract long LinesNotCovered { get; }
        /// <summary>
        /// Gets a number of lines partially covered by tests.
        /// </summary>
        public abstract long LinesPartiallyCovered { get; }

        /// <summary>
        /// Writes the <see cref="CoverageStatistics"/> content to the specified xml writer.
        /// </summary>
        /// <param name="xmlWriter"><see cref="XmlWriter"/> to which to write the content.</param>
        protected void WriteCoverageToXml(XmlWriter xmlWriter)
        {
            if (xmlWriter == null) throw new ArgumentNullException("xmlWriter");

            xmlWriter.WriteElementString("BlocksCovered", BlocksCovered.ToString(CultureInfo.InvariantCulture));
            xmlWriter.WriteElementString("BlocksNotCovered", BlocksNotCovered.ToString(CultureInfo.InvariantCulture));
            xmlWriter.WriteElementString("LinesCovered", LinesCovered.ToString(CultureInfo.InvariantCulture));
            xmlWriter.WriteElementString("LinesNotCovered", LinesNotCovered.ToString(CultureInfo.InvariantCulture));
            xmlWriter.WriteElementString("LinesPartiallyCovered", LinesPartiallyCovered.ToString(CultureInfo.InvariantCulture));
        }
    }
}