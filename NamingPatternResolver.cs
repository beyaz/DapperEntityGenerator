using System;
using Microsoft.SqlServer.Management.Smo;

namespace DapperEntityGenerator
{
    /// <summary>
    ///     The naming pattern resolver
    /// </summary>
    static class NamingPatternResolver
    {
        #region Public Methods
        /// <summary>
        ///     Gets the name of the variable.
        /// </summary>
        public static string GetVariableName(string columnName)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }

            var firstChar = columnName[0].ToString().ToLowerTR();
            if (firstChar == "ı")
            {
                firstChar = "i";
            }

            return firstChar + columnName.Substring(1);
        }

        /// <summary>
        ///     Resolves the pattern.
        /// </summary>
        public static string ResolvePattern(Table table, string pattern)
        {
            return pattern.Replace("{SchemaName}", table.Schema).Replace("{TableName}", table.Name);
        }

        /// <summary>
        ///     Resolves the pattern.
        /// </summary>
        public static string ResolvePattern(string schemaName, string pattern)
        {
            return pattern.Replace("{SchemaName}", schemaName);
        }
        #endregion
    }
}