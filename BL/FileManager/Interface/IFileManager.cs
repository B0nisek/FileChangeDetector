using BL.Models;

namespace BL.FileManager.Interface
{
    public interface IFileManager
    {
        DirectoryModel GetNewDirectoryModel(string path);

        DirectoryModel UpdateDirectoryModel(string path);
    }
} 
