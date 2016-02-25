using System.IO;
using System.IO.Compression;
using iabi.BCF.BCFv2;

namespace iabi.BCF.Tests.BCFTestCases.CreateAndExport
{
    public static class ZipArchiveFactory
    {
        public const bool CreatePhysicalFiles = false;

        public const string FolderName = @"C:\Users\Dangl\Downloads\_BCFTestCases";

        public static ZipArchive ReturnAndWriteIfRequired(BCFv2Container Container, string TestCaseName, string ReadmeText)
        {
            var MemStream = new MemoryStream();
            Container.WriteStream(MemStream);
            MemStream.Position = 0;
            var CreatedArchive = new ZipArchive(MemStream, ZipArchiveMode.Read);

            if (!CreatePhysicalFiles)
            {
                return CreatedArchive;
            }

            if (!Directory.Exists(FolderName + @"\" + TestCaseName))
            {
                Directory.CreateDirectory(FolderName + @"\" + TestCaseName);
            }

            var FilePath = FolderName + @"\" + TestCaseName + @"\" + TestCaseName + ".bcfzip";
            using (var FileStream = File.Create(FilePath))
            {
                Container.WriteStream(FileStream);
            }

            FilePath = FolderName + @"\" + TestCaseName + @"\Readme.md";
            using (var StreamWriter = new StreamWriter(File.Create(FilePath)))
            {
                StreamWriter.Write(ReadmeText);
            }

            return CreatedArchive;
        }
    }
}