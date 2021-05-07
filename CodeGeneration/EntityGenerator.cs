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
                var filePath = ResolvePattern(table, input.CSharpOutputFilePathForEntity);

                var fileContent = FileContentWriter.GetFileContent(ConvertToFileContentLines(table));

                FileHelper.WriteToFile(filePath, fileContent);
            }

            void ExportRepository(Table table)
            {
                RepositoryGenerator.ExportTable(table, SqlTypeToDotNetTypeMap.GetDotNetDataType,
                                                getEntityNamespaceName: t => ResolvePattern(t, input.NamespacePatternForEntity),
                                                getRepositoryClassName: t => ResolvePattern(t, input.ClassNamePatternForRepository),
                                                getRepositoryNamespaceName: t => ResolvePattern(t, input.NamespacePatternForRepository),
                                                getRepositoryOutputFilePath: t => ResolvePattern(t, input.CSharpOutputFilePathForRepository));
            }

            void GenerateTable(Table table)
            {
                trace($"Exporting table entity for {table.Name}");
                ExportEntity(table);

                trace($"Exporting table repository for {table.Name}");
                ExportRepository(table);
            }

            var processedTables = Loop(GetTablesInSchema, GenerateTable, updatePercent);

            RepositoryGenerator.ExportRepositoryMainClassDecleration(ResolvePattern(schemaName, input.NamespacePatternForRepository),
                                                                     ResolvePattern(schemaName, input.ClassNamePatternForRepository),
                                                                     ResolvePattern(schemaName, input.CSharpOutputFilePathForRepository));

            trace($"{processedTables.Count} table successfully exported.");
            updatePercent(100);
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
            var propertyType = SqlTypeToDotNetTypeMap.GetDotNetDataType(column.DataType.Name);

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
        #endregion
    }
}