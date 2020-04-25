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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class ReportPackage
    {
        public static void Create(string publicKeyPath, string reportPackagePath, string headerFilePath, string reportfilePath)
        {
            var zippedReportfilePath = Path.ChangeExtension(reportfilePath, ".zip");
            Packaging.ZipFiles(zippedReportfilePath, reportfilePath);

            var encryptedReportfilePath = Path.ChangeExtension(reportfilePath, ".encrypted.xml");
            Encryption.EncryptReportFile(zippedReportfilePath, encryptedReportfilePath, publicKeyPath);

            var encryptedHeaderfilePath = Path.ChangeExtension(headerFilePath, ".encrypted.xml");
            Encryption.EncryptReportFile(headerFilePath, encryptedHeaderfilePath, publicKeyPath);

            Packaging.ZipFiles(reportPackagePath, encryptedReportfilePath, encryptedHeaderfilePath);

        }

        public static string[] Unpack(string privateKeyXmlPath, string zippedReportPackagePath, string outputPath)
        {
            var files = new HashSet<string>();
            var unzippedFiles = Packaging.UnzipFiles(zippedReportPackagePath, outputPath);

            // Header has contents as text
            var encryptedHeaderfilePath = unzippedFiles.Single(f => Path.GetFileName(f).StartsWith("header"));
            var decryptedHeaderContent = Encryption.DecryptReportFile(encryptedHeaderfilePath, privateKeyXmlPath);
            var decryptedHeaderFilePath = encryptedHeaderfilePath.Replace(".encrypted.xml", ".xml"); // ChangeExtension can't handle it
            File.WriteAllBytes(decryptedHeaderFilePath, decryptedHeaderContent);

            files.Add(decryptedHeaderFilePath);
            // Actual report is zipped before encryption
            var encryptedReportfilePath = unzippedFiles.Single(f => !Path.GetFileName(f).StartsWith("header"));
            var zippedReportContent = Encryption.DecryptReportFile(encryptedReportfilePath, privateKeyXmlPath);
            var zippedReportFilePath = encryptedReportfilePath.Replace(".encrypted.xml", ".zip"); // ChangeExtension can't handle it
            File.WriteAllBytes(zippedReportFilePath, zippedReportContent);

            var decryptedReportFilePath = Packaging.UnzipFiles(zippedReportFilePath, "output").Single(); // this should be xbrl
            files.Add(decryptedReportFilePath);
            return files.ToArray();
        }
    }
}