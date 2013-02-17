using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.Coverage.Analysis;

namespace MSCover2Xml
{
    /// <summary>
    /// Code coverage report.
    /// </summary>
    // Based on http://blogs.msdn.com/b/phuene/archive/2009/12/01/programmatic-coverage-analysis-in-visual-studio-2010.aspx
    public sealed class CoverageReport
    {
        /// <summary>
        /// Gets a list of files included in code coverage inspection.
        /// </summary>
        public IEnumerable<FileSpec> Files { get; private set; }
        /// <summary>
        /// Gets a list of code coverage statistics for instrumented modules.
        /// </summary>
        public IEnumerable<ModuleStatistics> Modules { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoverageReport"/> class.
        /// </summary>
        internal CoverageReport(IEnumerable<ModuleStatistics> modules, IEnumerable<FileSpec> files)
        {
            if (modules == null) throw new ArgumentNullException("modules");
            if (files == null) throw new ArgumentNullException("files");

            Modules = modules;
            Files = files;
        }

        private static CoverageInfo CreateCoverageInfo(string coverageFile, IEnumerable<string> executablePaths, IEnumerable<string> symbolPaths)
        {
            if (string.IsNullOrEmpty(coverageFile)) throw new ArgumentNullException("coverageFile");
            
            // Ensure that we have correct paths for binaries and symbols.

            var executableDirs = executablePaths;
            if ((executableDirs == null) || !executableDirs.Any())
                executableDirs = new[] {Path.GetDirectoryName(coverageFile)};

            var symbolDirs = symbolPaths;
            if ((symbolDirs == null) || !symbolDirs.Any())
                symbolDirs = new[] {Path.GetDirectoryName(coverageFile)};

            return CoverageInfo.CreateFromFile(coverageFile, executableDirs, symbolDirs);
        }

        /// <summary>
        /// Creates a new code coverage report from a specified code coverage binary file.
        /// </summary>
        /// <param name="coverageFile">Path to the code coverage binary file.</param>
        /// <param name="executablePaths">List of executable paths.</param>
        /// <param name="symbolPaths">List of symbol paths.</param>
        /// <returns>A new instance of <see cref="CoverageReport"/> class.</returns>
        public static CoverageReport Create(string coverageFile, IEnumerable<string> executablePaths = null, IEnumerable<string> symbolPaths = null)
        {
            if (string.IsNullOrEmpty(coverageFile)) throw new ArgumentNullException("coverageFile");

            using (var coverageInfo = CreateCoverageInfo(coverageFile, executablePaths, symbolPaths))
                return Create(coverageInfo);
        }

        /// <summary>
        /// Creates a new code coverage report from a specified code coverage information.
        /// </summary>
        /// <param name="coverageInfo">Code coverage information for which to create a report.</param>
        /// <returns>A new instance of <see cref="CoverageReport"/> class.</returns>
        public static CoverageReport Create(CoverageInfo coverageInfo)
        {
            if (coverageInfo == null) throw new ArgumentNullException("coverageInfo");

            var files = new FileSpecList();
            var modules = GetModules(coverageInfo, files).ToList();

            var report = new CoverageReport(modules, files);
            return report;
        }

        /// <summary>
        /// Gets a list of module statistics from fiven coverage information.
        /// </summary>
        /// <param name="coverageInfo">Code coverage information for which to generate module list.</param>
        /// <param name="files">List that will receive the files that were instrumented.</param>
        /// <returns>An enumerable list of module statistics for coverage information.</returns>
        public static IEnumerable<ModuleStatistics> GetModules(CoverageInfo coverageInfo, FileSpecList files = null)
        {
            if (coverageInfo == null) throw new ArgumentNullException("coverageInfo");

            long lineStartId = 0;
            files = files ?? new FileSpecList();
            return coverageInfo.Modules.Select(x => ModuleStatistics.Create(files, x, ref lineStartId));
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

        private static void WriteListToXml<T>(XmlWriter xmlWriter, string elementName, IEnumerable<T> items, Action<XmlWriter, T> writeAction)
        {
            foreach (var item in items)
            {
                xmlWriter.WriteStartElement(elementName);
                writeAction(xmlWriter, item);
                xmlWriter.WriteEndElement();
            }
        }

        /// <summary>
        /// Writes coverage information from specified coverage file as XML to the specified output file.
        /// </summary>
        /// <remarks>Using this method is recommended if coverage information contains lots of data.</remarks>
        /// <param name="coverageFile">Path to the code coverage binary file.</param>
        /// <param name="outputFile">Output file to which the XML will be saved.</param>
        /// <param name="executablePaths">List of executable paths.</param>
        /// <param name="symbolPaths">List of symbol paths.</param>
        /// <param name="options">Options for the coverage report.</param>
        public static void WriteXml(string coverageFile, string outputFile, 
            IEnumerable<string> executablePaths = null, IEnumerable<string> symbolPaths = null, CoverageReportOptions options = null)
        {
            if (string.IsNullOrEmpty(coverageFile)) throw new ArgumentNullException("coverageFile");
            if (String.IsNullOrEmpty(outputFile)) throw new ArgumentNullException("outputFile");

            options = options ?? new CoverageReportOptions();

            using (var coverageInfo = CreateCoverageInfo(coverageFile, executablePaths, symbolPaths))
            using (var outputStream = File.CreateText(outputFile))
            using (var xmlWriter = XmlWriter.Create(outputStream, new XmlWriterSettings { OmitXmlDeclaration = false, Indent = true}))
            {
                var files = new FileSpecList();

                var rootName = string.IsNullOrEmpty(options.RootName) ? "CoverageDSPriv" : options.RootName;
                xmlWriter.WriteStartElement(rootName);

                long lineStartId = 0;
                WriteListToXml(xmlWriter, "Module", coverageInfo.Modules, (writer, item) =>
                {
                    var moduleStats = ModuleStatistics.Create(files, item, ref lineStartId);
                    moduleStats.WriteXml(writer);
                });
                WriteListToXml(xmlWriter, "SourceFileNames", files, (writer, item) => item.WriteXml(writer));

                xmlWriter.WriteEndElement();
            }
        }

        /// <summary>
        /// Writes the <see cref="CoverageReport"/> content to the specified xml writer.
        /// </summary>
        /// <param name="xmlWriter"><see cref="XmlWriter"/> to which to write the content.</param>
        public void WriteXml(XmlWriter xmlWriter)
        {
            if (xmlWriter == null) throw new ArgumentNullException("xmlWriter");

            WriteListToXml(xmlWriter, "Module", Modules, (writer, item) => item.WriteXml(writer));
            WriteListToXml(xmlWriter, "File", Files, (writer, item) => item.WriteXml(writer));
        }
    }
}