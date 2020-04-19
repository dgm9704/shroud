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

        public static byte[] EncryptSessionKey(byte[] sessionKey, string publicKeyXml)
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
    }
}
