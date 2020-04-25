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

        public static void EncryptReportFile(string reportFilePath, string encryptedReportPath, string publicKeyPath)
        {
            var reportData = File.ReadAllBytes(reportFilePath);
            var publicKey = File.ReadAllText(publicKeyPath);

            var (sessionKey, iv) = Encryption.GenerateSessionKey();

            var encryptedKey = Encryption.EncryptWithXmlKey(sessionKey, publicKey);
            var encryptedData = Encryption.EncryptReport(sessionKey, iv, reportData);

            var encryptedReport = new EncryptedReport();
            encryptedReport.SessionKey = Convert.ToBase64String(encryptedKey);
            encryptedReport.OutBuffer = Convert.ToBase64String(encryptedData);
            encryptedReport.ToFile(encryptedReportPath);
        }

        public static void DecryptReportFile(string encryptedReportfilePath, string privateKeyPath)
        {
            var encryptedReport = EncryptedReport.FromFile(encryptedReportfilePath);
            var encryptedSessionKey = Convert.FromBase64String(encryptedReport.SessionKey);
            var decryptedSessionKey = DecryptSessionKey(encryptedSessionKey,privateKeyPath);
            var outbuffer = Convert.FromBase64String(encryptedReport.OutBuffer);

        }

        private static byte[] DecryptSessionKey(byte[] encryptedSessionKey, string privateKeyPath)
        {
                       using (var rsa = RSA.Create())
            {
                rsa.FromXmlString(publicKeyXml);
                return rsa.Encrypt(sessionKey, RSAEncryptionPadding.Pkcs1);
            }
            var symmetricKey = rsa.Decrypt(encryptedSessionKey, false);
            //symmetricIV = rsa.Decrypt(encryptedSymmetricIV , false);
            return symmetricKey;
        }
    }
}
