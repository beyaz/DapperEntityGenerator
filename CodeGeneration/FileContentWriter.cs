using System.Collections.Generic;
using System.Text;

namespace DapperEntityGenerator.CodeGeneration
{
    /// <summary>
    ///     The file content writer
    /// </summary>
    static class FileContentWriter
    {
        #region Public Methods
        /// <summary>
        ///     Gets the content of the file.
        /// </summary>
        public static string GetFileContent(IReadOnlyList<string> lines)
        {
            var sb = new StringBuilder();

            var paddingCount  = 0;
            var paddingLength = 4;

            foreach (var line in lines)
            {
                void writeLine()
                {
                    sb.AppendLine(string.Empty.PadLeft(paddingCount, ' ') + line);
                }

                if (line.Trim() == "{")
                {
                    writeLine();
                    paddingCount += paddingLength;
                    continue;
                }

                if (line.Trim() == "}")
                {
                    paddingCount -= paddingLength;
                    if (paddingCount < 0)
                    {
                        paddingCount = 0;
                    }

                    writeLine();

                    continue;
                }

                writeLine();
            }

            return sb.ToString();
        }
        #endregion
    }
}