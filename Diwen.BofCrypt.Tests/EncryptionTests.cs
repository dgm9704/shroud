namespace Diwen.BofCrypt.Tests
{
    using System.Text;
    using Xunit;

    public class EncryptionTests
    {
        [Fact]
        public void GenerateSessionKeyTest()
        {
            var (key, iv) = Encryption.GenerateSessionKey();
        }

        [Fact]
        public void EncryptSessionKeyTest()
        {
            var plaintextKey = new byte[32];
            string keydata = "<RSAKeyValue><Modulus>sAXZXDDI8PNwy6ZGDkis/McSbWFdi+B+UV/lyHJRtwwnLHkKvgbms2o6OG2lDqvdU92bmOBrVPn162murwqIyZk/j8rZcOI0wgIUV431prkj39pGBWK4pA2YUDSCvfImlXJXu8tMAwoATJAA5ntJ4lQxA9ODeDhwlKI/yPxl+xUTrloLjsRn5KOrVCiweZO1eZq6ViVKmHCgH8XUSZYtbzmes9UBZgHGLj+L+pLJhF0aFzfc3+Ay1E/iEpus9y4e6rdf8C94wLxBIrE9hOSvm2V/WVZ5Ngg2OsUZO9K9aLl8lLMkVTw+QNDlru3z5xII/IWjjdoxZUEKQPMpYt41qw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            var encryptedKey = Encryption.EncryptSessionKey(plaintextKey, keydata);
        }

        [Fact]
        public void EncryptReportTest()
        {
            var sessionKey = new byte[32];
            var iv = new byte[16];

            var report = "<RSAKeyValue><Modulus>sAXZXDDI8PNwy6ZGDkis/McSbWFdi+B+UV/lyHJRtwwnLHkKvgbms2o6OG2lDqvdU92bmOBrVPn162murwqIyZk/j8rZcOI0wgIUV431prkj39pGBWK4pA2YUDSCvfImlXJXu8tMAwoATJAA5ntJ4lQxA9ODeDhwlKI/yPxl+xUTrloLjsRn5KOrVCiweZO1eZq6ViVKmHCgH8XUSZYtbzmes9UBZgHGLj+L+pLJhF0aFzfc3+Ay1E/iEpus9y4e6rdf8C94wLxBIrE9hOSvm2V/WVZ5Ngg2OsUZO9K9aLl8lLMkVTw+QNDlru3z5xII/IWjjdoxZUEKQPMpYt41qw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            var reportData = Encoding.UTF8.GetBytes(report);
            var encryptedReport = Encryption.EncryptReport(sessionKey, iv, reportData);
        }


    }
}
