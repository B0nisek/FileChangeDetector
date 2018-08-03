using BL.FileManager.Interface;
using System.IO;
using BL.Helpers;
using BL.Shared;
using BL.Enums;
using System.Linq;
using BL.Models;

namespace BL.FileManager
{
    public class FileManager : IFileManager
    {
        private DirectoryModel CurrentDirectoryModel;
        private DirectoryModel NewDirectoryModel;
        
        public DirectoryModel GetNewDirectoryModel(string path)
        {
            GetDirectoryModel(path);
            Callback();
            return CurrentDirectoryModel;
        }

        public DirectoryModel UpdateDirectoryModel(string path)
        {
            GetDirectoryModel(path);
            CheckForChanges();
            Callback();
            return CurrentDirectoryModel;
        }

        private DirectoryModel GetDirectoryModel(string path)
        {
            var directoryModel = new DirectoryModel();

            SetRoot(directoryModel);
            AddSubdirectories(directoryModel, path);
            AddFiles(directoryModel, path);
            ValidateDirectoryModel(directoryModel);

            return directoryModel;
        }

        private void CheckForChanges(DirectoryModel originalDirectoryModel = null, DirectoryModel newDirectoryModel = null)
        {
            var origDir = CurrentDirectoryModel;
            var newDir = NewDirectoryModel;

            if (originalDirectoryModel != null && newDirectoryModel != null)
            {
                origDir = originalDirectoryModel;
                newDir = newDirectoryModel;
            }

            CheckForDirectoryChanges(origDir, newDir);
            AddNewDirectories(origDir, newDir);
            CheckForFileChanges(origDir, newDir);
            AddNewFiles(origDir, newDir);
        }

        private void Callback()
        {
            NewDirectoryModel = null;
        }

        private void SetRoot(DirectoryModel directoryModel)
        {
            if (CurrentDirectoryModel == null)
            {
                CurrentDirectoryModel = directoryModel;
            }
            else if(NewDirectoryModel == null)
            {
                NewDirectoryModel = directoryModel;
            }
        }

        private void AddSubdirectories(DirectoryModel directoryModel, string path)
        {
            directoryModel.Path = Path.GetFullPath(path);
            var subdirectories = Directory.GetDirectories(path);

            if (subdirectories.Length != 0)
            {
                foreach (var directory in subdirectories)
                {
                    directoryModel.Subdirectories.Add(GetDirectoryModel(directory));
                }
            }
        }

        private void AddFiles(DirectoryModel directoryModel, string path)
        {
            foreach (var file in new DirectoryInfo(path).GetFiles())
            {
                directoryModel.Files.Add(CreateFileModel(file.FullName, file.Length));
            }
        }

        private FileModel CreateFileModel(string path, long size)
        {
            var model = new FileModel();

            model.Name = Path.GetFileName(path);
            model.Path = Path.GetFullPath(path);
            model.Hash = CryptographyProvider.Encode(File.ReadAllBytes(path));
            model.Size = SizeConverter.ConvertBytesToMegaBytes(size);

            return model;
        }

        private void ValidateDirectoryModel(DirectoryModel directoryModel)
        {
            if (directoryModel.NumberOfFiles > AppConstants.MAX_FILES_IN_DIR)
            {
                CurrentDirectoryModel.ErrorMessage = "files count > 100";
            }

            if (directoryModel.Size > AppConstants.MAX_FILES_SIZE_IN_DIR)
            {
                CurrentDirectoryModel.ErrorMessage = "files size > 50MB";
            }
        }

        private void DeleteDirectory(DirectoryModel directoryModel)
        {
            directoryModel.FileType = FileType.DELETED;

            foreach (var dir in directoryModel.Subdirectories)
            {
                DeleteDirectory(dir);
            }

            foreach (var file in directoryModel.Files)
            {
                file.FileType = FileType.DELETED;
            }
        }

        private void CheckForDirectoryChanges(DirectoryModel origDir, DirectoryModel newDir)
        {
            foreach (var dir in origDir.Subdirectories)
            {
                var nDir = newDir.Subdirectories.SingleOrDefault(x => x.Path == dir.Path);
                if (nDir != null)
                {
                    nDir.FileType = FileType.UNMNODIFIED;
                    CheckForChanges(dir, nDir);
                }
                else
                {
                    DeleteDirectory(dir);
                }
            }
        }

        private void CheckForFileChanges(DirectoryModel origDir, DirectoryModel newDir)
        {
            foreach (var file in origDir.Files)
            {
                var newFile = newDir.Files.SingleOrDefault(x => x.Name == file.Name);
                if (newFile != null)
                {
                    CheckFilesForChanges(file, newFile);
                }
                else
                {
                    file.FileType = FileType.DELETED;
                }
            }
        }

        private void CheckFilesForChanges(FileModel originalFile, FileModel newFile)
        {
            if(CryptographyProvider.HashesAreEqual(originalFile.Hash, newFile.Hash))
            {
                newFile.FileType = FileType.UNMNODIFIED;
            }
            else
            {
                originalFile.Version++;
                originalFile.FileType = FileType.MODIFIED;
                newFile.FileType = FileType.MODIFIED;
            }
        }

        private void AddNewDirectories(DirectoryModel origDir, DirectoryModel newDir)
        {
            var newDirs = newDir.Subdirectories.Where(x => x.FileType == FileType.NEW);
            origDir.Subdirectories.AddRange(newDirs);
        }

        private void AddNewFiles(DirectoryModel origDir, DirectoryModel newDir)
        {
            var newFiles = newDir.Files.Where(x => x.FileType == FileType.NEW);
            origDir.Files.AddRange(newFiles);
        }
    }
}