using Microsoft.VisualStudio.TestTools.UnitTesting;
using BL.FileManager;
using Tests.Helpers;
using System.IO;
using System.Linq;
using BL.Enums;

namespace Tests.BL
{
    [TestClass]
    public class FileManagerTests
    {
        private static FileGenerator FileGenerator = new FileGenerator();

        private FileManager FileManager;

        private const string dir1 = @"c:\files";
        private const string dir2 = @"c:\files\files2";

        [TestInitialize]
        public void Initialize()
        {
            CleanDirectory();
            FileManager = new FileManager();
        }

        [TestCleanup()]
        public void Cleanup()
        {
            CleanDirectory();
        }

        [TestMethod]
        public void GetFilesCorrectResult()
        {
            FileGenerator.GenerateFiles(dir1, 10, 2);

            var result = FileManager.GetNewDirectoryModel(dir1);
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(2, result.NumberOfFiles);
            Assert.AreEqual(0, result.SubdirectoriesCount);
        }

        [TestMethod]
        public void GetFilesWithNoChangesCorrectResult()
        {
            FileGenerator.GenerateFiles(dir1, 10, 2);

            var result = FileManager.GetNewDirectoryModel(dir1);
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(2, result.NumberOfFiles);
            Assert.AreEqual(0, result.SubdirectoriesCount);

            var updatedResult = FileManager.UpdateDirectoryModel(dir1);
            Assert.IsTrue(updatedResult.IsValid);
            Assert.AreEqual(2, updatedResult.NumberOfFiles);
            Assert.AreEqual(0, updatedResult.SubdirectoriesCount);
        }

        [TestMethod]
        public void GetModifiedFilesCorrect()
        {
            FileGenerator.GenerateFiles(dir1, 10, 1);

            var result = FileManager.GetNewDirectoryModel(dir1);
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(1, result.NumberOfFiles);
            Assert.AreEqual(0, result.SubdirectoriesCount);

            var newFile = result.Files.FirstOrDefault();
            Assert.IsNotNull(newFile);

            FileGenerator.ChangeFile(newFile.Path);

            var updateResult = FileManager.UpdateDirectoryModel(dir1);
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(1, result.NumberOfFiles);
            Assert.AreEqual(0, result.SubdirectoriesCount);

            var modifiedFile = updateResult.Files.FirstOrDefault();
            Assert.IsNotNull(modifiedFile);
            Assert.IsTrue(modifiedFile.FileType == FileType.MODIFIED);
            Assert.IsTrue(modifiedFile.Version != 1);
        }

        [TestMethod]
        public void FilesSizeOver50MB()
        {
            FileGenerator.GenerateFiles(dir1, 10000000, 6);

            var result = FileManager.GetNewDirectoryModel(dir1);
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void FilesNumberOver100()
        {
            FileGenerator.GenerateFiles(dir1, 5, 102);

            var result = FileManager.GetNewDirectoryModel(dir1);
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void  NewFilesMergedCorrectly()
        {
            FileGenerator.GenerateFiles(dir1, 5, 10);

            var result = FileManager.GetNewDirectoryModel(dir1);
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(10, result.NumberOfFiles);

            FileGenerator.GenerateFiles(dir1, 5, 5);

            var newResult = FileManager.UpdateDirectoryModel(dir1);
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(15, newResult.NumberOfFiles);
        }

        [TestMethod]
        public void FilesDeletedCorrectly()
        {
            FileGenerator.GenerateFiles(dir1, 5, 10);

            var result = FileManager.GetNewDirectoryModel(dir1);
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(10, result.NumberOfFiles);

            FileGenerator.DeleteFiles(dir1, 3);

            var updatedResult = FileManager.UpdateDirectoryModel(dir1);
            Assert.IsTrue(updatedResult.IsValid);
            Assert.AreEqual(7, updatedResult.NumberOfFiles);
        }

        [TestMethod]
        public void SubdirectoryCorrectlyAdded()
        {
            FileGenerator.GenerateFiles(dir1, 5, 10);

            var result = FileManager.GetNewDirectoryModel(dir1);
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(10, result.NumberOfFiles);

            FileGenerator.GenerateFiles(dir2, 5, 5);

            var updatedResult = FileManager.UpdateDirectoryModel(dir1);
            Assert.IsTrue(updatedResult.IsValid);
            Assert.AreEqual(15, updatedResult.NumberOfFiles);
            Assert.AreEqual(1, updatedResult.SubdirectoriesCount);
        }

        [TestMethod]
        public void SubdirectoryCorrectlyDeleted()
        {
            FileGenerator.GenerateFiles(dir1, 5, 10);
            FileGenerator.GenerateFiles(dir2, 5, 5);

            var result = FileManager.GetNewDirectoryModel(dir1);
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(15, result.NumberOfFiles);
            Assert.AreEqual(1, result.SubdirectoriesCount);

            FileGenerator.DeleteDirectory(dir2);

            var updatedResult = FileManager.UpdateDirectoryModel(dir1);
            Assert.IsTrue(updatedResult.IsValid);
            Assert.AreEqual(10, updatedResult.NumberOfFiles);
            Assert.AreEqual(0, updatedResult.SubdirectoriesCount);
        }

        [TestMethod]
        public void AddedTooManyFilesInDirectory()
        {
            FileGenerator.GenerateFiles(dir1, 5, 50);

            var result = FileManager.GetNewDirectoryModel(dir1);
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(50, result.NumberOfFiles);
            Assert.AreEqual(0, result.SubdirectoriesCount);

            FileGenerator.GenerateFiles(dir2, 5, 60);

            var updatedResult = FileManager.UpdateDirectoryModel(dir1);
            Assert.IsFalse(updatedResult.IsValid);
            Assert.AreEqual(110, updatedResult.NumberOfFiles);
            Assert.AreEqual(1, updatedResult.SubdirectoriesCount);
        }

        [TestMethod]
        public void AddAndDeleteCorrectly()
        {
            FileGenerator.GenerateFiles(dir1, 5, 50);

            var result = FileManager.GetNewDirectoryModel(dir1);
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(50, result.NumberOfFiles);
            Assert.AreEqual(0, result.SubdirectoriesCount);

            FileGenerator.GenerateFiles(dir2, 5, 5);

            var updatedResult = FileManager.UpdateDirectoryModel(dir1);
            Assert.IsTrue(updatedResult.IsValid);
            Assert.AreEqual(55, updatedResult.NumberOfFiles);
            Assert.AreEqual(1, updatedResult.SubdirectoriesCount);

            FileGenerator.DeleteFiles(dir1, 30);

            var updatedResult_1 = FileManager.UpdateDirectoryModel(dir1);
            Assert.IsTrue(updatedResult_1.IsValid);
            Assert.AreEqual(25, updatedResult_1.NumberOfFiles);
            Assert.AreEqual(1, updatedResult_1.SubdirectoriesCount);
            Assert.AreEqual(50, updatedResult_1.Files.Count);
        }
        
        private void CleanDirectory()
        {
            if (Directory.Exists(dir1))
            {
                Directory.Delete(dir1, true);
            }
        }
    }
}
