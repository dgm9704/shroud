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
    }
}
