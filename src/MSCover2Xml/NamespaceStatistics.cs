﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace MSCover2Xml
{
    /// <summary>
    /// Represents code coverage statistics for a namespace within assembly module.
    /// </summary>
    [Serializable]
    public sealed class NamespaceStatistics : CoverageStatistics
    {
        private readonly List<ClassStatistics> _classes = new List<ClassStatistics>();

        /// <summary>
        /// Gets the namespace name.
        /// </summary>
        public string NamespaceName { get; private set; }
        /// <summary>
        /// Gets the unique namespace name.
        /// </summary>
        public string UniqueName { get { return string.Format("{0}{1}{2}", Module.Name, Module.Signature, NamespaceName); } }
        /// <summary>
        /// Gets the <see cref="ModuleStatistics"/> containing the current namespace.
        /// </summary>
        public ModuleStatistics Module { get; private set; }
        /// <summary>
        /// Gets a list of statistics for classes included in the namespace.
        /// </summary>
        public IEnumerable<ClassStatistics> Classes { get { return _classes; } }
        /// <summary>
        /// Gets a number of blocks covered by tests.
        /// </summary>
        public override long BlocksCovered { get { return _classes.Sum(x => x.BlocksCovered); } }
        /// <summary>
        /// Gets a number of blocks not covered by tests.
        /// </summary>
        public override long BlocksNotCovered { get { return _classes.Sum(x => x.BlocksNotCovered); } }
        /// <summary>
        /// Gets a number of lines fully covered by tests.
        /// </summary>
        public override long LinesCovered { get { return _classes.Sum(x => x.LinesCovered); } }
        /// <summary>
        /// Gets a number of lines not covered by tests.
        /// </summary>
        public override long LinesNotCovered { get { return _classes.Sum(x => x.LinesNotCovered); } }
        /// <summary>
        /// Gets a number of lines partially covered by tests.
        /// </summary>
        public override long LinesPartiallyCovered { get { return _classes.Sum(x => x.LinesPartiallyCovered); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceStatistics"/> class.
        /// </summary>
        /// <param name="module">Module containing the namespace.</param>
        /// <param name="name">Namespace name.</param>
        internal NamespaceStatistics(ModuleStatistics module, string name)
        {
            if (module == null) throw new ArgumentNullException("module");
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            Module = module;
            NamespaceName = name;
        }

        /// <summary>
        /// Gets class statistics information or adds new information to the namespace.
        /// </summary>
        /// <param name="className">Class name</param>
        /// <returns><see cref="ClassStatistics"/> for the specified class name.</returns>
        internal ClassStatistics GetOrAddClass(string className)
        {
            var classStats = _classes.Find(x => x.ClassName == className);
            if (classStats == null)
            {
                classStats = new ClassStatistics(this, className);
                _classes.Add(classStats);
            }
            return classStats;
        }

        /// <summary>
        /// Writes the <see cref="NamespaceStatistics"/> content to the specified xml writer.
        /// </summary>
        /// <param name="xmlWriter"><see cref="XmlWriter"/> to which to write the content.</param>
        public void WriteXml(XmlWriter xmlWriter)
        {
            if (xmlWriter == null) throw new ArgumentNullException("xmlWriter");
            
            xmlWriter.WriteElementString("ModuleName", Module.Name);
            xmlWriter.WriteElementString("NamespaceName", NamespaceName);
            xmlWriter.WriteElementString("NamespaceKeyName", UniqueName);
            WriteCoverageToXml(xmlWriter);

            foreach (var cls in Classes)
            {
                xmlWriter.WriteStartElement("Class");
                cls.WriteXml(xmlWriter);
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
            return NamespaceName;
        }
    }
}