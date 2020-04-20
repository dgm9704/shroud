namespace Diwen.BofCrypt
{
    using System.IO;
    using System.IO.Compression;

    public static class Helper
    {
        public static void ZipFiles(string archive, params string[] files)
        {
            using (var stream = File.Create(archive))
            using (var zip = new ZipArchive(stream, ZipArchiveMode.Create))
                foreach (var file in files)
                    zip.CreateEntryFromFile(file, Path.GetFileName(file));
        }
    }
}