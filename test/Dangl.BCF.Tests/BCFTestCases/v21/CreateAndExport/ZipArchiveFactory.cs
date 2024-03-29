using System.IO;
using System.IO.Compression;
using Dangl.BCF.BCFv21;
using System;

namespace Dangl.BCF.Tests.BCFTestCases.v21.CreateAndExport
{
    public static class ZipArchiveFactory
    {
        public const string FOLDERNAME = @"";

        public static ZipArchive ReturnAndWriteIfRequired(BCFv21Container container, string testCaseName, string readmeText)
        {
            var memStream = new MemoryStream();
            container.WriteStream(memStream);
            memStream.Position = 0;
            var createdArchive = new ZipArchive(memStream, ZipArchiveMode.Read);
            if (string.IsNullOrWhiteSpace(FOLDERNAME))
            {
                return createdArchive;
            }
            if (!Directory.Exists(FOLDERNAME + @"\" + testCaseName))
            {
                Directory.CreateDirectory(FOLDERNAME + @"\" + testCaseName);
            }
            var filePath = FOLDERNAME + @"\" + testCaseName + @"\" + testCaseName + ".bcf";
            using (var fileStream = File.Create(filePath))
            {
                container.WriteStream(fileStream);
            }
            filePath = FOLDERNAME + @"\" + testCaseName + @"\Readme.md";
            using (var streamWriter = new StreamWriter(File.Create(filePath)))
            {
                readmeText = readmeText.TrimEnd()
                                 + Environment.NewLine
                                 + Environment.NewLine
                                 + "---"
                                 + Environment.NewLine
                                 + Environment.NewLine
                                 +$"Created by iabi at {DateTime.UtcNow:dd.MM.yyyy HH:mm} (UTC)";
                streamWriter.Write(readmeText);
            }
            return createdArchive;
        }
    }
}