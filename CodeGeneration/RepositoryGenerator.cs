using System;
using System.Collections.Generic;
using System.Linq;
using DapperEntityGenerator.IO;
using Microsoft.SqlServer.Management.Smo;
using static System.String;
using static DapperEntityGenerator.Extensions;

namespace DapperEntityGenerator.CodeGeneration
{
    static class  RepositoryGenerator
    {

        public static void ExportTable(Table table, Func<string, string> getVariableName, Func<Column, string> getDotNetTypeName)
        {
            var getMethods = Fun(() => GetRepositoryMethods(table, getVariableName, getDotNetTypeName));

            var content = GetRepositoryFileContent(table, getMethods, "AaA", "BbB");

            FileHelper.WriteToFile("d:\\A.cs",content);
        }

        public static IReadOnlyList<string> GetRepositoryMethods(Table table, Func<string, string> getVariableName, Func<Column, string> getDotNetTypeName)
        {
            var getColumnByName = Fun((string name) => table.Columns.ToEnumeration().First(x => x.Name == name));

            

            var getLinesOfIndexMethod = Fun((Index index) =>
            {
                var lines = new List<string>();

                var columnNames = index.IndexedColumns.ToEnumeration().Select(x => x.Name).ToList();
                var parameters  = index.IndexedColumns.ToEnumeration().Select(x => new {Name = getVariableName(x.Name), DotNetType = getDotNetTypeName(getColumnByName(x.Name))});

                var sqlPart       = $"SELECT * FROM {table.Name} WITH(NOLOCK) WHERE {Join(" AND ", columnNames.Select(x => x + " = @" + getVariableName(x)))}";
                var parameterPart = $"new {{ {Join(",", columnNames.Select(c => getVariableName(c)))} }}";

                if (index.IsUnique)
                {
                    lines.Add($"public {table.Name} Get{table.Name}By{Join("And", columnNames)}({Join(",", parameters.Select(p => p.DotNetType + " " + p.Name))})");
                    lines.Add("{");

                    lines.Add($"return connection.QueryFirstOrDefault<{table.Name}>(\"{sqlPart}\", {parameterPart});");

                    lines.Add("}");

                    return lines;
                }

                

                    lines.Add($"public IReadOnlyList<{table.Name}> Get{table.Name}By{Join("And", columnNames)}({Join(",", parameters.Select(p => p.DotNetType + " " + p.Name))})");
                    lines.Add("{");

                    lines.Add($"return connection.Query<{table.Name}>(\"{sqlPart}\", {parameterPart}).ToList();");

                    lines.Add("}");

                    return lines;
                

            });

            var returnList = new List<string>();

            var i = 0;
            foreach (Index index in table.Indexes)
            {
                if (i>0)
                {
                    returnList.Add(Empty);
                }

                returnList.AddRange(getLinesOfIndexMethod(index));
                i++;
            }

            return returnList;
        }
        
        public static string GetRepositoryFileContent(Table table, Func<IReadOnlyList<string>> getMethods, string namespacePattern,string repositoryClassNamePattern)
        {
            var lines = new List<string>();

            var className = repositoryClassNamePattern.Replace("{SchemaName}", table.Schema).Replace("{TableName}", table.Name);
            var namespaceName = namespacePattern.Replace("{SchemaName}", table.Schema).Replace("{TableName}", table.Name);

            lines.Add( "using System;");
            lines.Add("using Dapper.Contrib.Extensions;");
            
            lines.Add(Empty);
            lines.Add($"namespace {namespaceName}");
            lines.Add("{");
            lines.Add($"partial class {className}");
            lines.Add("{");

            lines.AddRange(getMethods());

            lines.Add("}");
            lines.Add("}");

            return FileContentWriter.GetFileContent(lines);
        }
    }
}