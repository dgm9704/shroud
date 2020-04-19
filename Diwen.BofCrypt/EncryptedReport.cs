
// <?xml version="1.0" encoding="utf-8"?>
// <ENCRYPTED_REPORT xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
//   <TITLE>BOFCRYPTNXT</TITLE>
//   <VERSION>2.0</VERSION>
//   <SESSION_KEY>Base64Enc(RSAEncrypt($FivaPublicKey, $SessionKey))</SESSION_KEY>
//   <OUT_BUFFER>Base64Enc((InitializationVector)(AESEncrypt($SessionKey, $PlainTextData)))</OUT_BUFFER>
// </ENCRYPTED_REPORT>

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