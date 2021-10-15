using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DapperEntityGenerator.IO;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using static System.String;
using static DapperEntityGenerator.Extensions;
using static DapperEntityGenerator.NamingPatternResolver;

namespace DapperEntityGenerator.CodeGeneration
{
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

            trace("Started.");

            updatePercent(3);

            var connectionString = input.ConnectionString;
            var databaseName     = input.DatabaseName;
            var schemaName       = input.SchemaName;

            IReadOnlyList<Table> GetTablesInSchema()
            {
                var sqlConnection    = new SqlConnection(connectionString);
                var serverConnection = new ServerConnection(sqlConnection);
                var server           = new Server(serverConnection);

                var exportTableNameList = ArgumentHelper.GetExportableTableNames(input.ExportTableNames);

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
                var usingLines = new List<string>
                {
                    "using System;",
                    "using Dapper.Contrib.Extensions;"
                };

                var lines = new List<string>
                {
                    Empty,
                    $"namespace {ResolvePattern(table, input.NamespacePatternForEntity)}",
                    "{"
                };
                lines.AddRange(ConvertToClassDefinition(table));

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
            
            void loopTrace(Table table, int count, int index, int completePercent)
            {
                trace($"Exporting table ({index} of {count}) entity for '{table.Name}'.");
                updatePercent(completePercent);
            }

            var processedTables = Loop(GetTablesInSchema, ExportEntity, loopTrace);

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


            var hasMultiplePrimaryKey = table.Columns.ToEnumerable().Count(x => x.InPrimaryKey) > 1;

            foreach (Column column in table.Columns)
            {
                lines.AddRange(ConvertToPropertyDefinition(column, hasMultiplePrimaryKey));
                lines.Add(Empty);
            }

            lines.Add("}");

            return lines;
        }

        static IReadOnlyList<string> ConvertToPropertyDefinition(Column column, bool hasMultiplePrimaryKey)
        {
            var propertyType = SqlTypeToDotNetTypeMap.GetDotNetDataType(column.DataType.Name);

            if (column.Nullable && !propertyType.Contains("string") && !propertyType.Contains("byte"))
            {
                propertyType += "?";
            }

            var lines = new List<string>();

            if (column.InPrimaryKey)
            {
                lines.Add(hasMultiplePrimaryKey ? "[ExplicitKey]" : "[Key]");
            }

            lines.Add($"public {propertyType} {column.Name} {{ get; set; }}");

            return lines;
        }
        #endregion
    }
}