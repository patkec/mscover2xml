using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace MSCover2Xml
{
    /// <summary>
    /// Represents code coverage statistics for an assembly module.
    /// </summary>
    [Serializable]
    public sealed class ModuleStatistics : CoverageStatistics
    {
        // Epoch is defined as December 31, 1969, at 4:00 P.M, see
        // http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.coverage.analysis.icoveragemodule.imagelinktime(v=vs.110)
        private static readonly DateTime Epoch = new DateTime(1969, 12, 31, 16, 0, 0, 0);

        private readonly List<NamespaceStatistics> _namespaces = new List<NamespaceStatistics>();

        /// <summary>
        /// Gets the name of the module.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Gets the size, in bytes, of the compiled assembly for the module.
        /// </summary>
        public long ImageSize { get; private set; }
        /// <summary>
        /// Gets the time stamp from the compiled assembly for the module.
        /// </summary>
        public DateTime ImageLinkTime { get; internal set; }
        /// <summary>
        /// Gets a list of statistics for namespaces included in the module.
        /// </summary>
        public IEnumerable<NamespaceStatistics> Namespaces { get { return _namespaces; } }
        /// <summary>
        /// Gets a number of blocks covered by tests.
        /// </summary>
        public override long BlocksCovered { get { return _namespaces.Sum(x => x.BlocksCovered); } }
        /// <summary>
        /// Gets a number of blocks not covered by tests.
        /// </summary>
        public override long BlocksNotCovered { get { return _namespaces.Sum(x => x.BlocksNotCovered); } }
        /// <summary>
        /// Gets a number of lines fully covered by tests.
        /// </summary>
        public override long LinesCovered { get { return _namespaces.Sum(x => x.LinesCovered); } }
        /// <summary>
        /// Gets a number of lines not covered by tests.
        /// </summary>
        public override long LinesNotCovered { get { return _namespaces.Sum(x => x.LinesNotCovered); } }
        /// <summary>
        /// Gets a number of lines partially covered by tests.
        /// </summary>
        public override long LinesPartiallyCovered { get { return _namespaces.Sum(x => x.LinesPartiallyCovered); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleStatistics"/> class.
        /// </summary>
        /// <param name="name">Module name.</param>
        /// <param name="imageSize">Size of the compiled module, in bytes.</param>
        /// <param name="imageLinkTime">Time stamp of the compiled module, in epoch time.</param>
        internal ModuleStatistics(string name, long imageSize, long imageLinkTime)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            Name = name;
            ImageSize = imageSize;
            SetImageLinkTime(imageLinkTime);
        }

        private void SetImageLinkTime(long secondsFromEpoch)
        {
            if (secondsFromEpoch < 0) throw new ArgumentOutOfRangeException("secondsFromEpoch");

            ImageLinkTime = Epoch.AddSeconds(secondsFromEpoch);
        }

        /// <summary>
        /// Gets namespace statistics information or adds new information to the module.
        /// </summary>
        /// <param name="namespaceName">Namespace name.</param>
        /// <returns><see cref="NamespaceStatistics"/> for the specified namespace name.</returns>
        internal NamespaceStatistics GetOrAddNamespace(string namespaceName)
        {
            if (string.IsNullOrEmpty(namespaceName)) throw new ArgumentNullException("namespaceName");

            var namespaceStats = _namespaces.Find(x => x.NamespaceName == namespaceName);
            if (namespaceStats == null)
            {
                namespaceStats = new NamespaceStatistics(namespaceName);
                _namespaces.Add(namespaceStats);
            }
            return namespaceStats;
        }

        /// <summary>
        /// Writes the <see cref="ModuleStatistics"/> content to the specified xml writer.
        /// </summary>
        /// <param name="xmlWriter"><see cref="XmlWriter"/> to which to write the content.</param>
        public void WriteXml(XmlWriter xmlWriter)
        {
            if (xmlWriter == null) throw new ArgumentNullException("xmlWriter");

            xmlWriter.WriteElementString("ModuleName", Name);
            xmlWriter.WriteElementString("ImageSize", ImageSize.ToString());
            xmlWriter.WriteElementString("ImageLinkTime", ImageLinkTime.ToString("o"));
            WriteCoverageToXml(xmlWriter);

            xmlWriter.WriteStartElement("Namespaces");
            foreach (var ns in Namespaces)
            {
                xmlWriter.WriteStartElement("Namespace");
                ns.WriteXml(xmlWriter);
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
            return Name;
        }
    }
}