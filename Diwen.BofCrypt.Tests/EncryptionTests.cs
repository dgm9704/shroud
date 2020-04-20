namespace Diwen.BofCrypt.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using Xunit;

    public class EncryptionTests
    {

        [Fact]
        public void EncryptReportFile()
        => Encryption.EncryptReportFile("report.zip", "report.encrypted.xml", "fin-fsa-pub.xml");

        [Fact]
        public void GenerateSessionKeyTest()
        {
            var (key, iv) = Encryption.GenerateSessionKey();
        }

        [Fact]
        public void EncryptWithXmlKeyTest()
        {
            var plaintextKey = new byte[32];
            var keydata = File.ReadAllText("fin-fsa-pub.xml", Encoding.ASCII);
            var encryptedKey = Encryption.EncryptWithXmlKey(plaintextKey, keydata);
        }

        [Fact]
        public void EncryptReportTest()
        {
            var sessionKey = new byte[32];
            var iv = new byte[16];

            var reportData = File.ReadAllBytes("report.zip");
            var encryptedReport = Encryption.EncryptReport(sessionKey, iv, reportData);
        }

        [Fact]
        public void EncryptedReportTest()
        {
            var encryptedKey = new byte[256];
            var encryptedData = new byte[2048];
            var encryptedReport = new EncryptedReport();
            encryptedReport.SessionKey = Convert.ToBase64String(encryptedKey);
            encryptedReport.OutBuffer = Convert.ToBase64String(encryptedData);
            var result = encryptedReport.ToXml();
        }

        [Fact]
        public void EncryptReportFileSteps()
        {
            var reportData = File.ReadAllBytes("report.zip");
            var publicKey = File.ReadAllText("fin-fsa-pub.xml", Encoding.ASCII);
            var (sessionKey, iv) = Encryption.GenerateSessionKey();

            var encryptedKey = Encryption.EncryptWithXmlKey(sessionKey, publicKey);
            var encryptedData = Encryption.EncryptReport(sessionKey, iv, reportData);

            var encryptedReport = new EncryptedReport();
            encryptedReport.SessionKey = Convert.ToBase64String(encryptedKey);
            encryptedReport.OutBuffer = Convert.ToBase64String(encryptedData);
            encryptedReport.ToFile("report.encrypted.xml");
        }

    }
}
