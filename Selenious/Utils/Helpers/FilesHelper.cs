namespace Selenious.Utils.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using NLog;
    using Phoenix.Core.Utils.Helpers;
    using Selenious.Core;
    using Selenious.Utils.Helpers;

    public static class FilesHelper
    {
        public static readonly char Separator = Path.DirectorySeparatorChar;

        /// <summary>
        /// Returns the extension of the desired file type.
        /// </summary>
        public static string ReturnFileExtension(FileType type)
        {
            switch (type)
            {
                case FileType.Pdf:
                    return ".pdf";
                case FileType.Xls:
                    return ".xls";
                case FileType.Csv:
                    return ".csv";
                case FileType.Txt:
                    return ".txt";
                case FileType.Doc:
                    return ".doc";
                case FileType.Xlsx:
                    return ".xlsx";
                case FileType.Docx:
                    return ".docx";
                case FileType.Gif:
                    return ".gif";
                case FileType.Jpg:
                    return ".jpg";
                case FileType.Bmp:
                    return ".bmp";
                case FileType.Png:
                    return ".png";
                case FileType.Xml:
                    return ".xml";
                case FileType.Html:
                    return ".html";
                case FileType.Ppt:
                    return ".ppt";
                case FileType.Pptx:
                    return ".pptx";
                case FileType.Zip:
                    return ".zip";
                case FileType.Xlsm:
                    return ".xlsm";
                case FileType.Jpeg:
                    return ".jpeg";
                case FileType.Json:
                    return ".json";
                case FileType.Svg:
                    return ".svg";
                default:
                    return "none";
            }
        }

        /// <summary>
        /// Returns a list of files having the desired filetype present in the desired folder.
        /// </summary>
        public static ICollection<FileInfo> GetFilesOfGivenType(string folder, FileType type, string postfixFilesName = null)
        {
            ICollection<FileInfo> files =
                new DirectoryInfo(folder)
                    .GetFiles("*" + postfixFilesName + ReturnFileExtension(type)).OrderBy(f => f.Name).ToList();
            return files;
        }

        /// <summary>
        /// Returns a list of files having the desired filetype present in the desired folder, with no postfix for the desired file name.
        /// </summary>
        public static ICollection<FileInfo> GetFilesOfGivenTypeFromAllSubFolders(string folder, FileType type)
        {
            return GetFilesOfGivenTypeFromAllSubFolders(folder, type, string.Empty);
        }

        /// <summary>
        /// Returns the list of files that match the desired filetype, with specified postfix in all the directories,
        /// using a value to determine whether to search subdirectories.
        /// </summary>
        public static ICollection<FileInfo> GetFilesOfGivenTypeFromAllSubFolders(string folder, FileType type, string postfixFilesName)
        {
            List<FileInfo> files =
                new DirectoryInfo(folder)
                    .GetFiles("*" + postfixFilesName + ReturnFileExtension(type), SearchOption.AllDirectories).OrderBy(f => f.Name).ToList();

            return files;
        }

        /// <summary>
        /// Returns the list of all the files in the desired folder, with the desired file name postfix.
        /// </summary>
        public static ICollection<FileInfo> GetAllFiles(string folder, string postfixFilesName)
        {
            ICollection<FileInfo> files =
                new DirectoryInfo(folder)
                    .GetFiles("*" + postfixFilesName).OrderBy(f => f.Name).ToList();

            return files;
        }

        /// <summary>
        /// Returns the list of all the files, with the desired file name postifx in the desired folder, by searching in all directories.
        /// </summary>
        public static ICollection<FileInfo> GetAllFilesFromAllSubFolders(string folder, string postfixFilesName)
        {
            ICollection<FileInfo> files =
                new DirectoryInfo(folder)
                    .GetFiles("*" + postfixFilesName, SearchOption.AllDirectories).OrderBy(f => f.Name).ToList();

            return files;
        }

        /// <summary> 
        /// Returns the list of all the files, in the desired folder, by searching in all directories.
        /// </summary>
        public static ICollection<FileInfo> GetAllFilesFromAllSubFolders(string folder)
        {
            return GetAllFilesFromAllSubFolders(folder, string.Empty);
        }

        /// <summary>
        /// Returns the list of all the files, present in the desired folder.
        /// </summary>
        public static ICollection<FileInfo> GetAllFiles(string folder)
        {
            return GetAllFiles(folder, string.Empty);
        }

        /// <summary>
        /// Returns the desired file, present in the desired folder.
        /// </summary>
        public static FileInfo GetFileByName(string folder, string fileName)
        {
            FileInfo file =
                new DirectoryInfo(folder)
                    .GetFiles(fileName).First();

            return file;
        }

        /// <summary>
        /// Returns the count of the files of desired file type, in the desired folder.
        /// </summary>
        public static int CountFiles(string folder, FileType type)
        {
            var fileNumber = GetFilesOfGivenType(folder, type).Count;
            return fileNumber;
        }

        /// <summary>
        /// Returns the total count of the files, in the desired folder.
        /// </summary>
        public static int CountFiles(string folder)
        {
            var fileNumber = GetAllFiles(folder).Count;
            return fileNumber;
        }

        /// <summary>
        /// Returns the last file name of the desired file type, in the desired folder.
        /// </summary>
        public static string GetLastFile(string folder, FileType type)
        {
            FileInfo lastFile =
                new DirectoryInfo(folder).GetFiles()
                    .Where(f => f.Extension == ReturnFileExtension(type))
                    .OrderByDescending(f => f.LastWriteTime)
                    .First();
            return lastFile.FullName;
        }

        /// <summary>
        /// Returns the last file, in the deisred folder.
        /// </summary>
        public static FileInfo GetLastFile(string folder)
        {
            var lastFile = new DirectoryInfo(folder).GetFiles()
                .OrderByDescending(f => f.LastWriteTime)
                .First();
            return lastFile;
        }

        /// <summary>
        /// Waits until the desired timeout for the count of the desired file type to increase in the desired folder.
        /// if checksize passed as true, waits for the size of the last file added to be graeter than zero.
        /// </summary>
        public static void WaitForFileOfGivenType(FileType type, double waitTime, int filesNumber, string folder, bool checkSize)
        {
            var timeoutMessage = string.Format(CultureInfo.CurrentCulture, "Waiting for file number to increase in {0}", folder);
            WaitHelper.Wait(
                () => CountFiles(folder, type) > filesNumber, TimeSpan.FromSeconds(waitTime), TimeSpan.FromSeconds(1), timeoutMessage);

            if (checkSize)
            {
                timeoutMessage = string.Format(CultureInfo.CurrentCulture, "Checking if size of last file of given type {0} > 0 bytes", type);

                WaitHelper.Wait(
                    () => GetLastFile(folder, type).Length > 0, TimeSpan.FromSeconds(waitTime), TimeSpan.FromSeconds(1), timeoutMessage);
            }
        }

        /// <summary>
        /// Waits untile timeout for the count of the desired file type to increase in the desired folder.
        /// </summary>
        public static void WaitForFileOfGivenType(FileType type, int filesNumber, string folder)
        {
            WaitForFileOfGivenType(type, BaseConfiguration.LongTimeout, filesNumber, folder, true);
        }

        /// <summary>
        /// Waits until the desired time out for desired file to increase in the desired folder.
        /// if checksize passed as true, waits for the size of the file added to be graeter than zero.
        /// </summary>
        public static void WaitForFileOfGivenName(double waitTime, string filesName, string folder, bool checkSize)
        {
            var timeoutMessage = string.Format(CultureInfo.CurrentCulture, "Waiting for file {0} in folder {1}", filesName, folder);
            WaitHelper.Wait(
                () => File.Exists(folder + Separator + filesName), TimeSpan.FromSeconds(waitTime), TimeSpan.FromSeconds(1), timeoutMessage);

            if (checkSize)
            {
                timeoutMessage = string.Format(CultureInfo.CurrentCulture, "Checking if size of file {0} > 0 bytes", filesName);
                WaitHelper.Wait(
                    () => GetFileByName(folder, filesName).Length > 0, TimeSpan.FromSeconds(waitTime), TimeSpan.FromSeconds(1), timeoutMessage);
            }
        }

        /// <summary>
        /// Waits until timeout for the desired file to increase in the desired folder.
        /// </summary>
        public static void WaitForFileOfGivenName(double waitTime, string filesName, string folder)
        {
            WaitForFileOfGivenName(waitTime, filesName, folder, true);
        }

        /// <summary>
        /// Waits for the desired file to increase in the desired folder.
        /// </summary>
        public static void WaitForFileOfGivenName(string filesName, string folder)
        {
            WaitForFileOfGivenName(BaseConfiguration.LongTimeout, filesName, folder);
        }

        /// <summary>
        /// Waits until timeout for the desired file to increase in the desired folder.
        /// if checksize passed as true, waits for the size of the file added to be graeter than zero.
        /// </summary>
        public static void WaitForFileOfGivenName(string filesName, string folder, bool checkSize)
        {
            WaitForFileOfGivenName(BaseConfiguration.LongTimeout, filesName, folder, checkSize);
        }

        /// <summary>
        /// Waits until the desired timeout for the count of the desired file to increase in the desired folder.
        /// if checksize passed as true, waits for the size of the last file added to be greater than zero bytes.
        /// </summary>
        public static void WaitForFile(double waitTime, int filesNumber, string folder, bool checkSize)
        {
            var timeoutMessage = string.Format(CultureInfo.CurrentCulture, "Waiting for file number to increase in {0}", folder);
            WaitHelper.Wait(
                () => CountFiles(folder) > filesNumber, TimeSpan.FromSeconds(waitTime), TimeSpan.FromSeconds(1), timeoutMessage);

            if (checkSize)
            {
                timeoutMessage = "Checking if size of last file > 0 bytes";

                WaitHelper.Wait(
                    () => GetLastFile(folder).Length > 0, TimeSpan.FromSeconds(waitTime), TimeSpan.FromSeconds(1), timeoutMessage);
            }
        }

        /// <summary>
        /// Waits for the count of the files in the desired folder to be greater than the desired fileNumbder.
        /// waits for the size of the file added to be greater than zero bytes.
        /// </summary>
        public static void WaitForFile(int filesNumber, string folder)
        {
            WaitForFile(BaseConfiguration.LongTimeout, filesNumber, folder, true);
        }

        /// <summary>
        /// Renames the old file name by replacing it with new file name.
        /// </summary>
        public static string RenameFile(double waitTime, string oldName, string newName, string subFolder)
        {
            newName = NameHelper.ShortenFileName(subFolder, newName, "_", 255);

            string fullPath = Path.Combine(subFolder, newName);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            string command = "/c ren " + '\u0022' + oldName + '\u0022' + " " + '\u0022' + newName +
                             '\u0022';
            ProcessStartInfo cmdsi = new ProcessStartInfo("cmd.exe")
            {
                WorkingDirectory = subFolder,
                Arguments = command
            };
            Thread.Sleep(1000);

            var timeoutMessage = string.Format(CultureInfo.CurrentCulture, "Waiting till file will be renamed {0}", subFolder);
            Process.Start(cmdsi);
            WaitHelper.Wait(() => File.Exists(subFolder + Separator + newName), TimeSpan.FromSeconds(waitTime), TimeSpan.FromSeconds(1), timeoutMessage);
            return newName;
        }

        /// <summary>
        /// Copies the contents of the old file in the new file.
        /// </summary>
        public static string CopyFile(double waitTime, string oldName, string newName, string workingDirectory)
        {
            return CopyFile(waitTime, oldName, newName, workingDirectory, workingDirectory);
        }

        /// <summary>
        /// Deletes the new file if already present.
        /// Copies the contents of the old file in the new file
        /// </summary>
        public static string CopyFile(double waitTime, string oldName, string newName, string oldSubFolder, string newSubFolder)
        {
            newName = NameHelper.ShortenFileName(newSubFolder, newName, "_", 255);

            if (File.Exists(newSubFolder + Separator + newName))
            {
                File.Delete(newSubFolder + Separator + newName);
            }

            string command = "/c copy " + '\u0022' + oldName + '\u0022' + " " + '\u0022' + newSubFolder + Separator + newName +
                             '\u0022';
            ProcessStartInfo cmdsi = new ProcessStartInfo("cmd.exe")
            {
                WorkingDirectory = oldSubFolder,
                Arguments = command
            };
            Thread.Sleep(1000);

            var timeoutMessage = string.Format(CultureInfo.CurrentCulture, "Waiting till file will be copied {0}", newSubFolder);
            Process.Start(cmdsi);
            WaitHelper.Wait(() => File.Exists(newSubFolder + Separator + newName), TimeSpan.FromSeconds(waitTime), TimeSpan.FromSeconds(1), timeoutMessage);
            return newName;
        }

        /// <summary>
        /// Copies the content of the source file from the destination file.
        /// </summary>
        public static void CopyFile(string sourceFilePath, string destinationDir, string newFileName = null)
        {
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            string destFileName;
            if (newFileName != null)
            {
                destFileName = Path.Combine(destinationDir, newFileName);
            }
            else
            {
                destFileName = Path.Combine(destinationDir, Path.GetFileName(sourceFilePath));
            }

            FileInfo file = new FileInfo(destFileName);
            if (file.Exists)
            {
                file.Delete();
            }

            File.Copy(sourceFilePath, destFileName, true);
        }

        /// <summary>
        /// Deletes the desired file from the desired sub-folder.
        /// </summary>
        public static void DeleteFile(string name, string subFolder)
        {
            string fullPath = Path.Combine(subFolder, name);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        /// <summary>
        /// Delets all the files and folders present at the desired directory path
        /// </summary>
        public static void DeleteDirectoryContents(string dirPath)
        {
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (Directory.Exists(dirPath))
            {
                foreach (FileInfo fi in dir.GetFiles())
                {
                    fi.Delete();
                }

                foreach (DirectoryInfo di in dir.GetDirectories())
                {
                    di.Delete();
                }
            }
        }

        /// <summary>
        /// Deletes the directory present at the desired directory path
        /// </summary>
        public static void DeleteDirectory(string dirPath)
        {
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (Directory.Exists(dirPath))
            {
                dir.Delete(true);
            }
        }

        /// <summary>
        /// Renames the old file as, adding the desired file type to desired new name.
        /// </summary>
        public static string RenameFile(string oldName, string newName, string subFolder, FileType type)
        {
            newName = newName + ReturnFileExtension(type);
            return RenameFile(BaseConfiguration.ShortTimeout, oldName, newName, subFolder);
        }

        /// <summary>
        /// Returns the folder name.
        /// </summary>
        public static string GetFolder(string appConfigValue, string currentFolder)
        {
            string folder;

            if (string.IsNullOrEmpty(appConfigValue))
            {
                folder = currentFolder;
            }
            else
            {
                if (BaseConfiguration.UseCurrentDirectory)
                {
                    folder = Path.Combine(currentFolder, appConfigValue);
                }
                else
                {
                    folder = appConfigValue;
                }

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }

            return folder;
        }

        /// <summary>
        /// Creates a directory, if desired folder exists.
        /// Else deletes all the files and folders in the desired directory.
        /// </summary>
        public static void CreateFolder(string folderName)
        {
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }
            else
            {
                DeleteDirectoryContents(folderName);
            }
        }

        /// <summary>
        /// Returns the total count of files of the specified file type in the specified folder.
        /// </summary>
        public static int GetFileCount(string folder, FileType type)
        {
            var count = Directory.GetFiles(folder, "*" + type).Count();
            return count;
        }

        /// <summary>
        /// Returns latest file name after waiting (until timeout) for the counting of files(of desired type) in desired folder to be completed.
        /// </summary>
        public static string GetDownloadedFile(string downloadFolder, FileType type, int oldFileCount, double timeout = -1)
        {
            double timer = (timeout == -1) ? BaseConfiguration.LongTimeout : timeout;
            var timeoutMessage = string.Format(CultureInfo.CurrentCulture, "Waiting for file number to increase in {0}", downloadFolder);
            WaitHelper.Wait(
                () => CountFiles(downloadFolder, type) > oldFileCount, TimeSpan.FromSeconds(timer), TimeSpan.FromSeconds(1), timeoutMessage);
            string latestFile = GetLastFile(downloadFolder, type);
            return latestFile;
        }
    }
}