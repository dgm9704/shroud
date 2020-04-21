//
//  This file is part of Diwen.BofCrypt.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2020 John Nordberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace Diwen.BofCrypt
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    [XmlRoot("ENCRYPTED_REPORT")]
    public class EncryptedReport
    {
        static AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();
        static Version version = assembly.Version;
        static string id = assembly.Name;

        static XmlSerializer Serializer = new XmlSerializer(typeof(EncryptedReport));

        static XmlReaderSettings XmlReaderSettings = new XmlReaderSettings
        {
            IgnoreWhitespace = true,
            IgnoreProcessingInstructions = false,
            IgnoreComments = false,
            XmlResolver = null,
            ValidationType = ValidationType.None
        };

        static XmlWriterSettings XmlWriterSettings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "\t",
            NamespaceHandling = NamespaceHandling.OmitDuplicates,
            Encoding = Encoding.UTF8
        };

        [XmlElement("TITLE")]
        public string Title { get; set; } = "BOFCRYPTNXT";

        [XmlElement("VERSION")]
        public string Version { get; set; } = "2.0";

        [XmlElement("SESSION_KEY")]
        public string SessionKey { get; set; } = string.Empty;

        [XmlElement("OUT_BUFFER")]
        public string OutBuffer { get; set; } = string.Empty;

        public void ToStream(Stream stream)
        {
            using (var writer = XmlWriter.Create(stream, XmlWriterSettings))
                ToXmlWriter(writer);
        }

        public static EncryptedReport FromFile(string path)
        {
            EncryptedReport report;

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                report = FromStream(stream);

            return report;
        }

        public void ToFile(string path)
        {
            using (var writer = XmlWriter.Create(path, XmlWriterSettings))
                ToXmlWriter(writer);
        }

        void ToXmlWriter(XmlWriter writer)
        {
            var ns = new XmlSerializerNamespaces();
            var info = $" {id} {version} ";
            Serializer.Serialize(writer, this, ns);
            writer.WriteComment(info);
        }

        public XmlDocument ToXmlDocument()
        {
            var document = new XmlDocument();
            var declaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.AppendChild(declaration);
            var nav = document.CreateNavigator();

            using (var writer = nav.AppendChild())
                ToXmlWriter(writer);

            return document;
        }

        public static EncryptedReport FromXml(string content)
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(content);
                writer.Flush();

                stream.Position = 0;

                return FromStream(stream);
            }
        }

        public static EncryptedReport FromStream(Stream stream)
        {
            EncryptedReport report;
            using (var reader = XmlReader.Create(stream, XmlReaderSettings))
                report = (EncryptedReport)Serializer.Deserialize(reader);

            return report;
        }

        public string ToXml()
        => ToXmlDocument().OuterXml;

    }

}