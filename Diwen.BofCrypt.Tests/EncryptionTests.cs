//
//  EncryptionTests.cs
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2020 John Nordberg
//
//  Free Public License 1.0.0
//  Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted.
//  THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND FITNESS.IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES 
//  OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS 
//  ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

namespace Diwen.BofCrypt.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using Xunit;

    public class EncryptionTests
    {

        [Fact]
        public void CreateReportPackageTest()
        => ReportPackage.Create("fin-fsa-pub.xml", "reportpackage.zip", "report.xbrl", "header.xml");

        [Fact]
        public void CreateReportPackageSteps()
        {
            Helper.ZipFiles("report.zip", "report.xbrl");
            Encryption.EncryptReportFile("report.zip", "report.encrypted.xml", "fin-fsa-pub.xml");
            Encryption.EncryptReportFile("header.xml", "header.encrypted.xml", "fin-fsa-pub.xml");
            Helper.ZipFiles("reportpackage.zip", "report.encrypted.xml", "header.encrypted.xml");
        }

        [Fact]
        public void EncryptReportFile()
        => Encryption.EncryptReportFile("header.xml", "header.encrypted.xml", "fin-fsa-pub.xml");

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

            var reportData = File.ReadAllBytes("header.xml");
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
            var reportData = File.ReadAllBytes("header.xml");
            var publicKey = File.ReadAllText("fin-fsa-pub.xml", Encoding.ASCII);
            var (sessionKey, iv) = Encryption.GenerateSessionKey();

            var encryptedKey = Encryption.EncryptWithXmlKey(sessionKey, publicKey);
            var encryptedData = Encryption.EncryptReport(sessionKey, iv, reportData);

            var encryptedReport = new EncryptedReport();
            encryptedReport.SessionKey = Convert.ToBase64String(encryptedKey);
            encryptedReport.OutBuffer = Convert.ToBase64String(encryptedData);
            encryptedReport.ToFile("header.encrypted.xml");
        }

    }
}
