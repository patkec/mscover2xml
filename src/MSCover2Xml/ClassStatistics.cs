﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace MSCover2Xml
{
    /// <summary>
    /// Represents code coverage statistics for a class within a namespace.
    /// </summary>
    [Serializable]
    public sealed class ClassStatistics : CoverageStatistics
    {
        private readonly IList<MethodStatistics> _methods = new List<MethodStatistics>();

        /// <summary>
        /// Gets the name od the class that was instrumented.
        /// </summary>
        public string ClassName { get; private set; }
        /// <summary>
        /// Gets the unique class name.
        /// </summary>
        public string UniqueName { get { return Namespace.UniqueName + ClassName; }}
        /// <summary>
        /// Gets the <see cref="NamespaceStatistics"/> containing the current class.
        /// </summary>
        public NamespaceStatistics Namespace { get; private set; }
        /// <summary>
        /// Gets a list of coverage information for methods contained in the class.
        /// </summary>
        public IEnumerable<MethodStatistics> Methods { get { return _methods; } }
        /// <summary>
        /// Gets a number of blocks covered by tests.
        /// </summary>
        public override long BlocksCovered { get { return _methods.Sum(x => x.BlocksCovered); } }
        /// <summary>
        /// Gets a number of blocks not covered by tests.
        /// </summary>
        public override long BlocksNotCovered { get { return _methods.Sum(x => x.BlocksNotCovered); } }
        /// <summary>
        /// Gets a number of lines fully covered by tests.
        /// </summary>
        public override long LinesCovered { get { return _methods.Sum(x => x.LinesCovered); } }
        /// <summary>
        /// Gets a number of lines not covered by tests.
        /// </summary>
        public override long LinesNotCovered { get { return _methods.Sum(x => x.LinesNotCovered); } }
        /// <summary>
        /// Gets a number of lines partially covered by tests.
        /// </summary>
        public override long LinesPartiallyCovered { get { return _methods.Sum(x => x.LinesPartiallyCovered); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassStatistics"/> class.
        /// </summary>
        /// <param name="ns">Namespace containing the class.</param>
        /// <param name="name">Class name.</param>
        internal ClassStatistics(NamespaceStatistics ns, string name)
        {
            if (ns == null) throw new ArgumentNullException("ns");
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            Namespace = ns;
            ClassName = name;
        }

        /// <summary>
        /// Adds method statistics to the class.
        /// </summary>
        /// <param name="method">Method statistics.</param>
        internal void AddMethod(MethodStatistics method)
        {
            if (method == null) throw new ArgumentNullException("method");

            method.Class = this;
            _methods.Add(method);
        }

        /// <summary>
        /// Writes the <see cref="ClassStatistics"/> content to the specified xml writer.
        /// </summary>
        /// <param name="xmlWriter"><see cref="XmlWriter"/> to which to write the content.</param>
        public void WriteXml(XmlWriter xmlWriter)
        {
            if (xmlWriter == null) throw new ArgumentNullException("xmlWriter");
            
            xmlWriter.WriteElementString("ClassKeyName", UniqueName);
            xmlWriter.WriteElementString("ClassName", ClassName);
            xmlWriter.WriteElementString("NamespaceKeyName", Namespace.UniqueName);
            WriteCoverageToXml(xmlWriter);

            foreach (var method in Methods)
            {
                xmlWriter.WriteStartElement("Method");
                method.WriteXml(xmlWriter);
                xmlWriter.WriteEndElement();
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return ClassName;
        }
    }
}