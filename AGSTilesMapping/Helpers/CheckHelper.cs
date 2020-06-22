using AGSTilesMapping.Core;
using ESRI.ArcGIS.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGSTilesMapping.Helpers
{
    class CheckHelper
    {
        /// <summary>
        /// Checks, if directory's path is not empty, creates directory if it doesn't exist
        /// and checks if it's empty or not.
        /// </summary>
        /// <param name="directoryInfo">Directory to check.</param>
        /// <param name="shouldBeEmpty">Should directory be empty?
        /// <para/>If set <see keyword="null"/>, emptyness doesn't check.</param>
        public static void CheckDirectory(DirectoryInfo directoryInfo, bool? shouldBeEmpty = null)
        {
            //Check directory's path.
            if (string.IsNullOrWhiteSpace(directoryInfo.FullName))
                throw new Exception(string.Format(Strings.StringIsEmpty, nameof(directoryInfo.FullName)));

            //Try to create directory.
            try
            {
                directoryInfo.Create();
                directoryInfo.Refresh();
            }
            catch (Exception exception)
            {
                throw new
                    Exception(string.Format(Strings.UnableToCreate, nameof(directoryInfo), directoryInfo.FullName),
                              exception);
            }

            if (shouldBeEmpty == true)
            {
                if (directoryInfo.EnumerateFileSystemInfos().Any())
                    throw new Exception(string.Format(Strings.DirectoryIsntEmpty, directoryInfo.FullName));
            }
            else if (shouldBeEmpty == false)
            {
                if (!directoryInfo.EnumerateFileSystemInfos().Any())
                    throw new Exception(string.Format(Strings.DirectoryIsEmpty, directoryInfo.FullName));
            }
        }

        public static async ValueTask<TileFileInfo> CheckInputFile(FileInfo inputFileInfo)
        {
            CheckFile(inputFileInfo, true);
            //Check if input image is ready for cropping.
            return await AGSTilesMapping.Core.AGSTile.GetTileFileInfo(inputFileInfo);
        }
        internal static void CheckFile(FileInfo fileInfo, bool shouldExist)
        {
            //Update file state.
            fileInfo.Refresh();

            //Check file's path.
            if (string.IsNullOrWhiteSpace(fileInfo.FullName))
                throw new Exception(string.Format(Strings.StringIsEmpty, nameof(fileInfo.FullName)));

            //Check file's extension.
            if (fileInfo.Extension != AGSTilesMapping.FileExtension.extimg& fileInfo.Extension != AGSTilesMapping.FileExtension.extshp & fileInfo.Extension != AGSTilesMapping.FileExtension.exttif)
                throw new Exception(string.Format(Strings.WrongExtension, nameof(fileInfo)));

            //Check file's existance.
            if (shouldExist)
            {
                if (!fileInfo.Exists)
                    throw new Exception(string.Format(Strings.DoesntExist, nameof(fileInfo), fileInfo.FullName));
            }
            else if (fileInfo.Exists)
            {
                throw new Exception(string.Format(Strings.AlreadyExist, nameof(fileInfo), fileInfo.FullName));
            }
        }
    }
}
