using BL.Enums;
using System.Collections.Generic;
using System.Linq;

namespace BL.Models
{
    public class DirectoryModel
    {
        public string Path { get; set; }

        public List<FileModel> Files { get; set; }

        public List<DirectoryModel> Subdirectories { get; set; }

        public double Size => Files.Where(x => x.FileType != FileType.DELETED).Sum(x => x.Size)
            + Subdirectories.Where(x => x.FileType != FileType.DELETED).Sum(x => x.Size);

        public int NumberOfFiles => Files.Where(x => x.FileType != FileType.DELETED).Count() 
            + Subdirectories.Sum(x => x.Files.Where(y => y.FileType != FileType.DELETED).Count());

        public int SubdirectoriesCount => 
            Subdirectories.Where(x => x.FileType != FileType.DELETED).Count();

        public string ErrorMessage { get; set; }

        public bool IsValid => string.IsNullOrEmpty(ErrorMessage);

        public FileType FileType { get; set; }

        public DirectoryModel()
        {
            Subdirectories = new List<DirectoryModel>();
            Files = new List<FileModel>();
        }
    }
}