using System;
using System.IO;

namespace DapperEntityGenerator.IO
{
    /// <summary>
    ///     The file helper
    /// </summary>
    static class FileHelper
    {
        #region Public Methods
        /// <summary>
        ///     Writes all text.
        /// </summary>
        public static void WriteToFile(string filePath, string content)
        {
            var directoryName = Path.GetDirectoryName(filePath);
            if (directoryName == null)
            {
                throw new ArgumentNullException(nameof(directoryName));
            }

            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            File.WriteAllText(filePath, content);
        }
        #endregion
    }
}