using System;
using System.Globalization;
using System.Xml;
using Microsoft.VisualStudio.Coverage.Analysis;

namespace MSCover2Xml
{
    [Serializable]
    public sealed class BlockCoverage
    {
        /// <summary>
        /// Gets or sets the starting line of code coverage block.
        /// </summary>
        public long LineStart { get; private set; }
        /// <summary>
        /// Gets or sets the ending line of code coverage block.
        /// </summary>
        public long LineEnd { get; private set; }
        /// <summary>
        /// Gets or sets the starting column of code coverage block.
        /// </summary>
        public long ColumnStart { get; private set; }
        /// <summary>
        /// Gets or sets the ending column of code coverage block.
        /// </summary>
        public long ColumnEnd { get; private set; }
        /// <summary>
        /// Gets or sets the file containing the code coverage block.
        /// </summary>
        public FileSpec File { get; private set; }
        /// <summary>
        /// Gets or sets the coverage status for the block.
        /// </summary>
        public CoverageStatus Coverage { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockCoverage"/> class.
        /// </summary>
        /// <param name="file"><see cref="FileSpec"/> containing the instrumented line.</param>
        /// <param name="lineStart">Starting line of code coverage block.</param>
        /// <param name="lineEnd">Ending line of code coverage block.</param>
        /// <param name="columnStart">Starting column of code coverage block.</param>
        /// <param name="columnEnd">Ending column of code coverage block.</param>
        /// <param name="coverage">Block coverage status.</param>
        internal BlockCoverage(FileSpec file, long lineStart, long lineEnd, long columnStart, long columnEnd, CoverageStatus coverage)
        {
            File = file;
            LineStart = lineStart;
            LineEnd = lineEnd;
            ColumnStart = columnStart;
            ColumnEnd = columnEnd;
            Coverage = coverage;
        }

        /// <summary>
        /// Writes the <see cref="BlockCoverage"/> content to the specified xml writer.
        /// </summary>
        /// <param name="xmlWriter"><see cref="XmlWriter"/> to which to write the content.</param>
        public void WriteXml(XmlWriter xmlWriter)
        {
            if (xmlWriter == null) throw new ArgumentNullException("xmlWriter");

            xmlWriter.WriteElementString("LineStart", LineStart.ToString(CultureInfo.InvariantCulture));
            xmlWriter.WriteElementString("LineEnd", LineEnd.ToString(CultureInfo.InvariantCulture));
            xmlWriter.WriteElementString("ColumnStart", ColumnStart.ToString(CultureInfo.InvariantCulture));
            xmlWriter.WriteElementString("ColumnEnd", ColumnEnd.ToString(CultureInfo.InvariantCulture));
            xmlWriter.WriteElementString("Coverage", ((int)Coverage).ToString(CultureInfo.InvariantCulture));
            xmlWriter.WriteElementString("FileID", File.Id.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="BlockCoverage"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// true if the specified object is equal to the current object; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as BlockCoverage;
            return (other != null) &&
                   Equals(File, other.File) &&
                   LineStart.Equals(other.LineStart) &&
                   LineEnd.Equals(other.LineEnd) &&
                   ColumnStart.Equals(other.ColumnStart) &&
                   ColumnEnd.Equals(other.ColumnEnd);
        }

        /// <summary>
        /// Serves as a hash function for the <see cref="BlockCoverage"/> type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="BlockCoverage"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return File.GetHashCode() ^
                   LineStart.GetHashCode() ^ LineEnd.GetHashCode() ^
                   ColumnStart.GetHashCode() ^ ColumnEnd.GetHashCode();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} ({1}:{2} - {3}:{4})", File.FileName, LineStart, ColumnStart, LineEnd, ColumnEnd);
        }
    }
}