using System;
using System.IO;
using System.Linq;

namespace Tests.Helpers
{
    public class FileGenerator
    {
        private static Random random = new Random();
        private int fileCounter = 0;

        public void GenerateFiles(string dirPath, int length, int numberOfFiles)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            for (int i = 0; i < numberOfFiles; i++)
            {
                using (var sw = File.CreateText(string.Format(@"{0}\{1}.txt", dirPath, fileCounter.ToString())))
                {
                    sw.WriteLine(GenerateRandomString(length));
                    fileCounter++;
                }
            }
        }

        public void DeleteFiles(string dirPath, int numberOfFiles)
        {
            int i = 0;

            foreach (var file in new DirectoryInfo(dirPath).GetFiles())
            {
                file.Delete();

                if (i >= numberOfFiles - 1)
                {
                    break;
                }

                i++;
            }
        }

        public void DeleteDirectory(string dirPath)
        {
            Directory.Delete(dirPath, true);
        }

        public void ChangeFile(string path)
        {
            File.WriteAllText(path, GenerateRandomString(119));
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
