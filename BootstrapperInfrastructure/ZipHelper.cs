using System;
using System.IO;
using System.IO.Compression;

namespace DapperEntityGenerator.BootstrapperInfrastructure
{
    /// <summary>
    ///     The zip helper
    /// </summary>
    static class ZipHelper
    {
        #region Public Methods
        /// <summary>
        ///     Extracts the zip file contents to directory.
        /// </summary>
        public static void ExtractZipFileContentsToDirectory(byte[] zipFileData, string targetDirectoryPath, Action<string> trace)
        {
            var temporaryZipFilePath = Path.Combine(targetDirectoryPath, Guid.NewGuid() + ".zip");

            if (File.Exists(temporaryZipFilePath))
            {
                trace($"Deleting file {temporaryZipFilePath}");
                File.Delete(temporaryZipFilePath);
            }

            var targetDirectory = Path.GetDirectoryName(temporaryZipFilePath);
            if (targetDirectory == null)
            {
                throw new ArgumentNullException(nameof(targetDirectory));
            }

            if (File.Exists(targetDirectory))
            {
                trace($"Deleting file {targetDirectory}");
                File.Delete(targetDirectory);
            }

            if (Directory.Exists(targetDirectory))
            {
                trace($"Deleting directory {targetDirectory}");
                Directory.Delete(targetDirectory, true);
            }

            trace($"Creating directory {targetDirectory}");
            Directory.CreateDirectory(targetDirectory);

            File.WriteAllBytes(temporaryZipFilePath, zipFileData);

            ZipFile.ExtractToDirectory(temporaryZipFilePath, Path.GetDirectoryName(temporaryZipFilePath));

            File.Delete(temporaryZipFilePath);
        }
        #endregion
    }
}