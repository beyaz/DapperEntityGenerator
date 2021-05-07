using System.Collections.Generic;
using System.Linq;

namespace DapperEntityGenerator.CodeGeneration
{
    /// <summary>
    ///     The argument helper
    /// </summary>
    static class ArgumentHelper
    {
        /// <summary>
        ///     Gets the exportable table names.
        /// </summary>
        public static IList<string> GetExportableTableNames(string exportTableNames)
        {
            if (exportTableNames == null || exportTableNames == "*")
            {
                return new List<string>();
            }

            return exportTableNames.Split(',').ToList();
        }
    }
}