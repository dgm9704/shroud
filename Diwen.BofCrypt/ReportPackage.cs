namespace Diwen.BofCrypt
{
    using System.IO;

    public static class ReportPackage
    {
        public static void Create(string publicKeyPath, string reportPackagePath, string reportfilePath, string headerFilePath)
        {
            var zippedReportfilePath = Path.ChangeExtension(reportfilePath, ".zip");
            Helper.ZipFiles(zippedReportfilePath, reportfilePath);

            var encryptedReportfilePath = Path.ChangeExtension(reportfilePath, ".encrypted.xml");
            Encryption.EncryptReportFile(zippedReportfilePath, encryptedReportfilePath, publicKeyPath);

            var encryptedHeaderfilePath = Path.ChangeExtension(headerFilePath, ".encrypted.xml");
            Encryption.EncryptReportFile(headerFilePath, encryptedHeaderfilePath, publicKeyPath);

            Helper.ZipFiles(reportPackagePath, encryptedReportfilePath, encryptedHeaderfilePath);

        }
    }
}