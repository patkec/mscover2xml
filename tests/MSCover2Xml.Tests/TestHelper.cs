using System;
using System.IO;
using System.Xml;

namespace MSCover2Xml.Tests
{
    internal static class TestHelper
    {
        public static string GetXml(Action<XmlWriter> writeAction)
        {
            using (var writer = new StringWriter())
            using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = false, OmitXmlDeclaration = true }))
            {
                xmlWriter.WriteStartElement("Root");
                writeAction(xmlWriter);
                xmlWriter.WriteEndElement();

                xmlWriter.Flush();
                return writer.ToString();
            }
        }
    }
}