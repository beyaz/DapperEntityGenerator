using System;
using Microsoft.SqlServer.Management.Smo;

namespace DapperEntityGenerator
{
    static class NamingPattern
    {
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

        public static string GetRepositoryClassName(Table table)
        {
            return $"{table.Schema}Repository";
        }

        public static string GetRepositoryNamespaceName(Table table)
        {
            return $"A.B.C{table.Schema}";
        }

        public static string GetRepositoryClassName(Table table,string repositoryClassNamePattern)
        {
            return repositoryClassNamePattern.Replace("{SchemaName}", table.Schema).Replace("{TableName}", table.Name);
        }

        public static string GetRepositoryNamespaceName(Table table,string namespacePattern)
        {
            return namespacePattern.Replace("{SchemaName}", table.Schema).Replace("{TableName}", table.Name);
        }

        public static string GetRepositoryClassOutputFilePath(Table table,string pattern)
        {
            return pattern.Replace("{SchemaName}", table.Schema).Replace("{TableName}", table.Name);
        }
    }
}