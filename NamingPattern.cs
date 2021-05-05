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
        
        

        public static string ResolvePattern(Table table,string pattern)
        {
            return pattern.Replace("{SchemaName}", table.Schema).Replace("{TableName}", table.Name);
        }

        public static string ResolvePattern(string schemaName,string pattern)
        {
            return pattern.Replace("{SchemaName}", schemaName);
        }

        
    }
}