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
    using System.IO;
    using System.Xml.Serialization;

    [XmlRoot("ENCRYPTED_REPORT")]
    public class EncryptedReport
    {
        [XmlElement("TITLE")]
        public string Title { get; set; } = "BOFCRYPTNXT";

        [XmlElement("VERSION")]
        public string Version { get; set; } = "2.0";

        [XmlElement("SESSION_KEY")]
        public string SessionKey { get; set; } = string.Empty;

        [XmlElement("OUT_BUFFER")]
        public string OutBuffer { get; set; } = string.Empty;

        public void ToFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                this.ToStream(stream);
        }

        public string ToXml()
        {
            using (var writer = new StringWriter())
            {
                var xml = new XmlSerializer(typeof(EncryptedReport));
                xml.Serialize(writer, this);
                return writer.ToString();
            }
        }

        public void ToStream(Stream stream)
        {
            var xml = new XmlSerializer(typeof(EncryptedReport));
            xml.Serialize(stream, this);
        }

    }

}