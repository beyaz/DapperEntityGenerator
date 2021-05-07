using System.Collections.Generic;
using System.Linq;

namespace DapperEntityGenerator.CodeGeneration
{
    static class ArgumentHelper
    {
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