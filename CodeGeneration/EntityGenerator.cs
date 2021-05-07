using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DapperEntityGenerator.IO;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using static System.String;
using static DapperEntityGenerator.Extensions;
using static DapperEntityGenerator.NamingPattern;

namespace DapperEntityGenerator.CodeGeneration
{
    class ForeignKeyInfo
    {
        #region Public Properties
        public bool IsUnique { get; set; }
        public Table Source { get; set; }
        public Table Target { get; set; }
        #endregion
    }

    static class EntityGenerator
    {
        #region Public Methods
        public static void GenerateSchema(EntityGeneratorInput input, ProcessInfo processInfo)
        {
            void trace(string traceMessage)
            {
                processInfo.Trace = traceMessage;
            }

            void updatePercent(int percent)
            {
                processInfo.Percent = percent;
            }

            IList<string> getExportableTableNames()
            {
                if (input.ExportTableNames == null || input.ExportTableNames == "*")
                {
                    return new List<string>();
                }

                return input.ExportTableNames.Split(',').ToList();
            }

            trace("Started.");

            updatePercent(3);

            var connectionString    = input.ConnectionString;
            var databaseName        = input.DatabaseName;
            var schemaName          = input.SchemaName;
            var exportTableNameList = getExportableTableNames();

            var cSharpOutputFilePath = input.CSharpOutputFilePathForEntity;

            IReadOnlyList<string> GetUsingList()
            {
                return new[]
                {
                    "using System;",
                    "using Dapper.Contrib.Extensions;"
                };
            }

            IReadOnlyList<Table> GetTablesInSchema()
            {
                var sqlConnection    = new SqlConnection(connectionString);
                var serverConnection = new ServerConnection(sqlConnection);
                var server           = new Server(serverConnection);

                var tables = new List<Table>();

                foreach (Table table in server.Databases[databaseName].Tables)
                {
                    if (table.Name.StartsWith("sys"))
                    {
                        continue;
                    }

                    if (exportTableNameList.Count > 0 && !exportTableNameList.Contains(table.Name))
                    {
                        continue;
                    }

                    if (table.Schema == schemaName)
                    {
                        tables.Add(table);
                    }
                }

                return tables;
            }

            IReadOnlyList<string> ConvertToFileContentLines(Table table)
            {
                var usingLines = new List<string>(GetUsingList());

                var lines = new List<string>();

                lines.Add(Empty);
                lines.Add($"namespace {ResolvePattern(table, input.NamespacePatternForEntity)}");
                lines.Add("{");
                lines.AddRange(ConvertToClassDefinition(table));

                //var relatedDataContract = GetRelatedDataContract(table, GetTablesInSchema());
                //if (relatedDataContract.Count > 0)
                //{
                //    usingLines.Add("using System.Collections.Generic;");
                //    lines.AddRange(relatedDataContract);
                //}

                lines.Add("}");

                lines.InsertRange(0, usingLines);

                return lines;
            }

            void ExportEntity(Table table)
            {
                trace($"Exporting table entity for {table.Name}");

                var filePath = cSharpOutputFilePath.Replace("{SchemaName}", schemaName).Replace("{TableName}", table.Name);

                var fileContent = FileContentWriter.GetFileContent(ConvertToFileContentLines(table));

                FileHelper.WriteToFile(filePath, fileContent);
            }

            void ExportRepository(Table table)
            {
                trace($"Exporting table repository for {table.Name}");

                RepositoryGenerator.ExportTable(table,
                                                GetDotNetDataType,
                                                getEntityNamespaceName: t => ResolvePattern(t, input.NamespacePatternForEntity),
                                                getRepositoryClassName: t => ResolvePattern(t, input.ClassNamePatternForRepository),
                                                getRepositoryNamespaceName: t => ResolvePattern(t, input.NamespacePatternForRepository),
                                                getRepositoryOutputFilePath: t => ResolvePattern(t, input.CSharpOutputFilePathForRepository));
            }

            void GenerateTable(Table table)
            {
                ExportEntity(table);
                ExportRepository(table);
            }

            var processedTables = Loop(GetTablesInSchema, GenerateTable, updatePercent);

            RepositoryGenerator.ExportRepositoryMainClassDecleration(ResolvePattern(schemaName, input.NamespacePatternForRepository),
                                                                     ResolvePattern(schemaName, input.ClassNamePatternForRepository),
                                                                     ResolvePattern(schemaName, input.CSharpOutputFilePathForRepository));

            processInfo.Trace   = $"{processedTables.Count} table successfully exported.";
            processInfo.Percent = 100;
        }

        public static IReadOnlyList<string> GetRelatedDataContract(Table table, IReadOnlyList<Table> searchTables)
        {
            var lines = new List<string>();

            if (!table.HasPrimaryClusteredIndex)
            {
                return lines;
            }

            var foreignKeyTables = TryGetForeignKeyInfo(table, searchTables);
            if (foreignKeyTables.Count == 0)
            {
                return lines;
            }

            lines.Add("[Serializable]");
            lines.Add($"public sealed class {table.Name}RelatedData");
            lines.Add("{");
            lines.Add($"public {table.Name} {table.Name} {{ get; set; }}");

            foreach (var foreignKeyInfo in foreignKeyTables)
            {
                var tableName = foreignKeyInfo.Target.Name;

                if (foreignKeyInfo.IsUnique)
                {
                    lines.Add($"public {tableName} {tableName} {{ get; set; }}");
                    continue;
                }

                lines.Add($"public IReadOnlyList<{tableName}> {tableName} {{ get; set; }}");
            }

            lines.Add("}");

            return lines;
        }
        #endregion

        #region Methods
        static IReadOnlyList<string> ConvertToClassDefinition(Table table)
        {
            var lines = new List<string>
            {
                "[Serializable]",
                $"[Table(\"{table.Schema}.{table.Name}\")]",
                $"public sealed class {table.Name}",
                "{"
            };

            foreach (Column column in table.Columns)
            {
                lines.AddRange(ConvertToPropertyDefinition(column));
                lines.Add(Empty);
            }

            lines.Add("}");

            return lines;
        }

        static IReadOnlyList<string> ConvertToPropertyDefinition(Column column)
        {
            var propertyType = GetDotNetDataType(column.DataType.Name);

            if (column.Nullable && !propertyType.Contains("string") && !propertyType.Contains("byte"))
            {
                propertyType += "?";
            }

            var lines = new List<string>();

            if (column.InPrimaryKey)
            {
                lines.Add("[Key]");
            }

            lines.Add($"public {propertyType} {column.Name} {{ get; set; }}");

            return lines;
        }

        static string GetDotNetDataType(Column column)
        {
            return GetDotNetDataType(column.DataType.Name);
        }

        static string GetDotNetDataType(string sqlDataTypeName)
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

        static ForeignKeyInfo TryGetForeignKeyInfo(Table source, Table target)
        {
            if (source.Schema == target.Schema && source.Name == target.Name)
            {
                return null;
            }

            foreach (ForeignKey foreignKey in target.ForeignKeys)
            {
                if (foreignKey.ReferencedTable == source.Name)
                {
                    if (foreignKey.ReferencedTableSchema == source.Schema)
                    {
                        foreach (Index index in target.Indexes)
                        {
                            if (index.IsUnique)
                            {
                                if (index.IndexedColumns.Count == 1 && index.IndexedColumns[0].Name == foreignKey.Columns[0].Name)
                                {
                                    return new ForeignKeyInfo {Source = source, Target = target, IsUnique = true};
                                }
                            }
                        }

                        return new ForeignKeyInfo {Source = source, Target = target, IsUnique = false};
                    }
                }
            }

            return null;
        }

        static IReadOnlyList<ForeignKeyInfo> TryGetForeignKeyInfo(Table source, IReadOnlyList<Table> searchTables)
        {
            var items = new List<ForeignKeyInfo>();

            foreach (var target in searchTables)
            {
                var foreignKeyInfo = TryGetForeignKeyInfo(source, target);
                if (foreignKeyInfo != null)
                {
                    items.Add(foreignKeyInfo);
                }
            }

            return items;
        }
        #endregion
    }
}