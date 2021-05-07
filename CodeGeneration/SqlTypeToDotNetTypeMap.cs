using System;
using Microsoft.SqlServer.Management.Smo;

namespace DapperEntityGenerator.CodeGeneration
{
    /// <summary>
    ///     The SQL type to dot net type map
    /// </summary>
    static class SqlTypeToDotNetTypeMap
    {
        #region Public Methods
        /// <summary>
        ///     Gets the type of the dot net data.
        /// </summary>
        public static string GetDotNetDataType(Column column)
        {
            return GetDotNetDataType(column.DataType.Name);
        }

        /// <summary>
        ///     Gets the type of the dot net data.
        /// </summary>
        public static string GetDotNetDataType(string sqlDataTypeName)
        {
            switch (sqlDataTypeName.ToLower())
            {
                case "bigint":
                    return "Int64";
                case "binary":
                case "image":
                case "varbinary":
                    return "byte[]";
                case "bit":
                    return "bool";
                case "char":
                    return "char";
                case "datetime":
                case "date":
                case "smalldatetime":
                    return "DateTime";
                case "decimal":
                case "money":
                case "numeric":
                    return "decimal";
                case "float":
                    return "double";
                case "int":
                    return "int";
                case "nchar":
                case "nvarchar":
                case "ntext":
                case "text":
                case "varchar":
                case "xml":
                    return "string";
                case "real":
                    return "single";
                case "smallint":
                    return "Int16";
                case "tinyint":
                    return "byte";
                case "uniqueidentifier":
                    return "Guid";

                default:
                    throw new Exception(sqlDataTypeName);
            }
        }
        #endregion
    }
}