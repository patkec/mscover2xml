using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace MSCover2Xml
{
    /// <summary>
    /// Code coverage report.
    /// </summary>
    public sealed class CoverageReport
    {
        private readonly List<FileSpec> _files = new List<FileSpec>();
        private readonly IList<ModuleStatistics> _modules = new List<ModuleStatistics>();

        /// <summary>
        /// Gets a list of files included in code coverage inspection.
        /// </summary>
        public IEnumerable<FileSpec> Files { get { return _files; } }
        /// <summary>
        /// Gets a list of code coverage statistics for instrumented modules.
        /// </summary>
        public IEnumerable<ModuleStatistics> Modules { get { return _modules; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoverageReport"/> class.
        /// </summary>
        internal CoverageReport()
        {
        }

        /// <summary>
        /// Adds code coverage statistics for a module.
        /// </summary>
        /// <param name="moduleStats">Code coverage </param>
        internal void AddModule(ModuleStatistics moduleStats)
        {
            if (moduleStats == null) throw new ArgumentNullException("moduleStats");

            _modules.Add(moduleStats);
        }

        /// <summary>
        /// Gets file specification for a file or creates a new file specification if none exists.
        /// </summary>
        /// <param name="fileName">Name of the file for which to get the specification.</param>
        /// <returns><see cref="FileSpec"/> object for given file name.</returns>
        internal FileSpec GetOrAddFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException("fileName");

            var file = _files.Find(x => x.FileName == fileName);
            if (file == null)
            {
                file = new FileSpec(fileName, _files.Count + 1);
                _files.Add(file);
            }
            return file;
        }

        /// <summary>
        /// Writes the <see cref="CoverageReport"/> content to the specified xml writer.
        /// </summary>
        /// <param name="xmlWriter"><see cref="XmlWriter"/> to which to write the content.</param>
        public void WriteXml(XmlWriter xmlWriter)
        {
            if (xmlWriter == null) throw new ArgumentNullException("xmlWriter");

            xmlWriter.WriteStartElement("Modules");
            foreach (var module in Modules)
            {
                xmlWriter.WriteStartElement("Module");
                module.WriteXml(xmlWriter);
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("Files");
            foreach (var file in Files)
            {
                xmlWriter.WriteStartElement("File");
                file.WriteXml(xmlWriter);
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Saves current coverage report to the specified file.
        /// </summary>
        /// <param name="fileName">Full path to the file to which coverage report will be saved.</param>
        public void SaveToFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException("fileName");

            using (var outputStream = File.CreateText(fileName))
            using (var xmlWriter = XmlWriter.Create(outputStream, new XmlWriterSettings { OmitXmlDeclaration = false }))
            {
                xmlWriter.WriteStartElement("Coverage");
                WriteXml(xmlWriter);
                xmlWriter.WriteEndElement();
            }
        }
    }
}