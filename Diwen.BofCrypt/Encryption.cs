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
    using System.Security.Cryptography;

    public class Encryption
    {
        public static (byte[], byte[]) GenerateSessionKey()
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.GenerateKey();
                return (aes.Key, aes.IV);
            }
        }

        public static byte[] EncryptSessionKey(byte[] data, string publicKeyXml)
        {
            using (var rsa = RSA.Create())
            {
                rsa.FromXmlString(publicKeyXml);
                return rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
            }
        }

        public static byte[] DecryptSessionKey(byte[] data, string privateKeyXml)
        {
            using (var rsa = RSA.Create())
            {
                rsa.FromXmlString(privateKeyXml);
                return rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
            }
        }

        public static void EncryptReportFile(string reportFilePath, string encryptedReportPath, string publicKeyXmlPath)
        {
            var reportData = File.ReadAllBytes(reportFilePath);
            var publicKeyXml = File.ReadAllText(publicKeyXmlPath);

            var (sessionKey, iv) = Encryption.GenerateSessionKey();

            var encryptedKey = Encryption.EncryptSessionKey(sessionKey, publicKeyXml);
            var encryptedData = Encryption.EncryptReport(sessionKey, iv, reportData);

            var encryptedReport = new EncryptedReport();
            encryptedReport.SessionKey = Convert.ToBase64String(encryptedKey);
            encryptedReport.OutBuffer = Convert.ToBase64String(encryptedData);
            encryptedReport.ToFile(encryptedReportPath);
        }

        public static byte[] DecryptReportFile(string encryptedReportfilePath, string privateKeyXmlPath)
        {
            var encryptedReport = EncryptedReport.FromFile(encryptedReportfilePath);
            var encryptedSessionKey = Convert.FromBase64String(encryptedReport.SessionKey);
            var privateKeyXml = File.ReadAllText(privateKeyXmlPath);

            var decryptedSessionKey = DecryptSessionKey(encryptedSessionKey, privateKeyXml);
            var encryptedContent = Convert.FromBase64String(encryptedReport.OutBuffer);
            var decryptedContent = Encryption.DecryptReport(decryptedSessionKey, encryptedContent);
            return decryptedContent;
        }

        public static byte[] EncryptReport(byte[] key, byte[] iv, byte[] data)
        {
            byte[] result;
            byte[] encryptedData;
            var ivLength = iv.Length;

            using (var aes = Aes.Create())
            using (var encryptor = aes.CreateEncryptor(key, iv))
                encryptedData = encryptor.TransformFinalBlock(data, 0, data.Length);

            result = new byte[ivLength + encryptedData.Length];
            Array.Copy(iv, result, ivLength);
            Array.Copy(encryptedData, 0, result, ivLength, encryptedData.Length);

            return result;
        }

        private static byte[] DecryptReport(byte[] key, byte[] data)
        {
            int ivLength = 16;
            var iv = new byte[ivLength];
            Array.Copy(data, iv, ivLength);

            using (var aes = Aes.Create())
            using (var decryptor = aes.CreateDecryptor(key, iv))
                return decryptor.TransformFinalBlock(data, ivLength, data.Length - ivLength);
        }
    }
}