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
    using System.Text;

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

        public static byte[] EncryptWithXmlKey(byte[] sessionKey, string publicKeyXml)
        {
            using (var rsa = RSA.Create())
            {
                rsa.FromXmlString(publicKeyXml);
                return rsa.Encrypt(sessionKey, RSAEncryptionPadding.Pkcs1);
            }
        }

        public static byte[] EncryptReport(byte[] key, byte[] iv, byte[] report)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Key = key;
                aes.IV = iv;
                using (var encryptor = aes.CreateEncryptor(key, iv))
                using (var resultStream = new MemoryStream())
                {
                    resultStream.Write(iv, 0, iv.Length);
                    using (var aesStream = new CryptoStream(resultStream, encryptor, CryptoStreamMode.Write))
                    using (var plainStream = new MemoryStream(report))
                        plainStream.CopyTo(aesStream);

                    return resultStream.ToArray();
                }
            }
        }

        public static void EncryptReportFile(string reportFilePath, string encryptedReportPath, string publicKeyXmlPath)
        {
            var reportData = File.ReadAllBytes(reportFilePath);
            var publicKeyXml = File.ReadAllText(publicKeyXmlPath);

            var (sessionKey, iv) = Encryption.GenerateSessionKey();

            var encryptedKey = Encryption.EncryptWithXmlKey(sessionKey, publicKeyXml);
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

            var decryptedSessionKey = DecryptWithXmlKey(encryptedSessionKey, privateKeyXml);
            var encryptedContent = Convert.FromBase64String(encryptedReport.OutBuffer);
            var decryptedContent = Encryption.DecryptReport(decryptedSessionKey, encryptedContent);
            return decryptedContent;
        }

        private static byte[] DecryptReport(byte[] key, byte[] data)
        {
            int ivLength = 16;
            //byte[] result;
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                var iv = new byte[ivLength];
                Array.Copy(data, iv, ivLength);
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(key, iv))
                using (MemoryStream encryptedStream = new MemoryStream(data, ivLength, data.Length - ivLength))
                using (CryptoStream decryptCryptoStream = new CryptoStream(encryptedStream, decryptor, CryptoStreamMode.Read))
                using (MemoryStream decryptedStream = new MemoryStream())
                {
                    decryptCryptoStream.CopyTo(decryptedStream);
                    return decryptedStream.ToArray();
                }
                //using (StreamReader decryptReader = new StreamReader(decryptCryptoStream))
                //    result = decryptReader.ReadToEnd(); // returns a string
            }
            //            return result; // the content might zipped so return it as bytes
        }

        public static byte[] DecryptWithXmlKey(byte[] encryptedSessionKey, string privateKeyXml)
        {
            using (var rsa = RSA.Create())
            {
                rsa.FromXmlString(privateKeyXml);
                return rsa.Decrypt(encryptedSessionKey, RSAEncryptionPadding.Pkcs1);
            }
        }
    }
}
