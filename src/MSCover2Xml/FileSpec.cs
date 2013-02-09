using System;
using System.Globalization;
using System.Xml;

namespace MSCover2Xml
{
    /// <summary>
    /// Represents a file instrumented for code coverage.
    /// </summary>
    [Serializable]
    public sealed class FileSpec
    {
        /// <summary>
        /// Gets the file identification within a <see cref="CoverageReport"/>.
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// Gets name of the file that was instrumented.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSpec"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="id">File identification within a <see cref="CoverageReport"/>.</param>
        internal FileSpec(string fileName, int id)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException("fileName");

            Id = id;
            FileName = fileName;
        }

        /// <summary>
        /// Writes the <see cref="FileSpec"/> content to the specified xml writer.
        /// </summary>
        /// <param name="xmlWriter"><see cref="XmlWriter"/> to which to write the content.</param>
        public void WriteXml(XmlWriter xmlWriter)
        {
            if (xmlWriter == null) throw new ArgumentNullException("xmlWriter");

            xmlWriter.WriteElementString("FileID", Id.ToString(CultureInfo.InvariantCulture));
            xmlWriter.WriteElementString("FileName", FileName);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="FileSpec"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// true if the specified object is equal to the current object; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as FileSpec;
            return (other != null) && Id.Equals(other.Id);
        }

        /// <summary>
        /// Serves as a hash function for the <see cref="FileSpec"/> type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="FileSpec"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} ({1})", FileName, Id);
        }
    }
}