using BL.Enums;

namespace BL.Models
{
    public class FileModel
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public byte[] Hash { get; set; }

        public double Size { get; set; }

        public int Version { get; set; }

        public FileType FileType { get; set; }

        public FileModel()
        {
            FileType = FileType.NEW;
            Version = 1;
        }
    }
}