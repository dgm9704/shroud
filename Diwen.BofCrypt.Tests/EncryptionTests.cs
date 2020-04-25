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
    using System.Linq;
    using System.Text;
    using Xunit;

    public class EncryptionTests
    {

        [Fact]
        public void EndToEndTest()
        {
            var inputFiles = new[] { "data/header.xml", "data/report.xbrl" };
            ReportPackage.Create("keys/public.xml", "output/reportpackage.zip", inputFiles.First(), inputFiles.Last());
            var outputFiles = ReportPackage.Unpack("keys/private.xml", "output/reportpackage.zip", "output");
            var expectedOutputFiles = inputFiles.Select(f => f.Replace("data/", "output/")).ToArray();
            Assert.Equal(expectedOutputFiles, outputFiles);
        }


        [Fact]
        public void CreateReportPackageTest()
        => ReportPackage.Create("keys/public.xml", "output/reportpackage.zip", "data/header.xml", "data/report.xbrl");

        [Fact]
        public void UnpackReportPackageTest()
        => ReportPackage.Unpack("keys/private.xml", "output/reportpackage.zip", "output");

        [Fact]
        public void CreateReportPackageSteps()
        {
            Packaging.ZipFiles("report.zip", "data/report.xbrl");
            Encryption.EncryptReportFile("data/header.xml", "header.encrypted.xml", "keys/fin-fsa-pub.xml");
            Encryption.EncryptReportFile("report.zip", "report.encrypted.xml", "keys/fin-fsa-pub.xml");
            Packaging.ZipFiles("reportpackage.zip", "header.encrypted.xml", "report.encrypted.xml");
        }

        [Fact]
        public void EncryptReportFile()
        => Encryption.EncryptReportFile("data/header.xml", "header.encrypted.xml", "keys/fin-fsa-pub.xml");

        [Fact]
        public void GenerateSessionKeyTest()
        {
            var (key, iv) = Encryption.GenerateSessionKey();
        }

        [Fact]
        public void EncryptWithXmlKeyTest()
        {
            var plaintextKey = new byte[32];
            var keydata = File.ReadAllText("keys/fin-fsa-pub.xml", Encoding.ASCII);
            var encryptedKey = Encryption.EncryptSessionKey(plaintextKey, keydata);
        }

        [Fact]
        public void EncryptReportTest()
        {
            var sessionKey = new byte[32];
            var iv = new byte[16];

            var reportData = File.ReadAllBytes("data/header.xml");
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
            var reportData = File.ReadAllBytes("data/header.xml");
            var publicKey = File.ReadAllText("keys/fin-fsa-pub.xml", Encoding.ASCII);
            var (sessionKey, iv) = Encryption.GenerateSessionKey();

            var encryptedKey = Encryption.EncryptSessionKey(sessionKey, publicKey);
            var encryptedData = Encryption.EncryptReport(sessionKey, iv, reportData);

            var encryptedReport = new EncryptedReport();
            encryptedReport.SessionKey = Convert.ToBase64String(encryptedKey);
            encryptedReport.OutBuffer = Convert.ToBase64String(encryptedData);
            encryptedReport.ToFile("header.encrypted.xml");
        }

        [Fact]
        public void DecryptSessionKey()
        {
            var sessionKey = new byte[32];
            var publicKeyXml = File.ReadAllText("keys/public.xml", Encoding.ASCII);
            var encryptedKey = Encryption.EncryptSessionKey(sessionKey, publicKeyXml);

            var privateKeyXml = File.ReadAllText("keys/private.xml", Encoding.ASCII);
            var decryptedKey = Encryption.DecryptSessionKey(encryptedKey, privateKeyXml);
        }
    }
}