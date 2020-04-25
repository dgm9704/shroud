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
    using System.IO.Compression;
    using System.Linq;

    public static class Packaging
    {
        public static void ZipFiles(string archivePath, params string[] files)
        {
            var directory = Path.GetDirectoryName(archivePath);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            using (var stream = File.Create(archivePath))
            using (var zip = new ZipArchive(stream, ZipArchiveMode.Create))
                foreach (var file in files)
                    zip.CreateEntryFromFile(file, Path.GetFileName(file));
        }

        internal static string[] UnzipFiles(string archive, string outputPath)
        {
            var files = new HashSet<string>();
            using (var stream = File.OpenRead(archive))
            using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
                foreach (var entry in zip.Entries)
                {
                    var extractedPath = Path.Combine(outputPath, entry.Name);
                    entry.ExtractToFile(extractedPath, true);
                    files.Add(extractedPath);
                }
            return files.ToArray();
        }
    }
}