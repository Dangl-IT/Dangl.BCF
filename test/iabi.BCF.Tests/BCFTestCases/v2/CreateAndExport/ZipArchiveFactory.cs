using System.IO;
using System.IO.Compression;
using iabi.BCF.BCFv2;

namespace iabi.BCF.Tests.BCFTestCases.v2.CreateAndExport
{
    public static class ZipArchiveFactory
    {
        public const string FOLDERNAME = @"";

        public static ZipArchive ReturnAndWriteIfRequired(BCFv2Container container, string testCaseName, string readmeText)
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

            var filePath = FOLDERNAME + @"\" + testCaseName + @"\" + testCaseName + ".bcfzip";
            using (var fileStream = File.Create(filePath))
            {
                container.WriteStream(fileStream);
            }

            filePath = FOLDERNAME + @"\" + testCaseName + @"\Readme.md";
            using (var streamWriter = new StreamWriter(File.Create(filePath)))
            {
                streamWriter.Write(readmeText);
            }

            return createdArchive;
        }
    }
}