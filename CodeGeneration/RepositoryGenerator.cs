using System;
using System.Collections.Generic;
using System.Linq;
using DapperEntityGenerator.IO;
using Microsoft.SqlServer.Management.Smo;
using static System.String;
using static DapperEntityGenerator.Extensions;
using  static  DapperEntityGenerator.NamingPattern;

namespace DapperEntityGenerator.CodeGeneration
{
    static class  RepositoryGenerator
    {

        public static void ExportTable(Table table,  Func<Column, string> getDotNetTypeName,Func<Table,string> getClassName,Func<Table,string> getNamespaceName,Func<Table,string> getOutputFilePath)
        {
            var getMethods = Fun(() => GetRepositoryMethods(table, getDotNetTypeName));
            
            var content = GetRepositoryFileContent(table, getMethods,getClassName,getNamespaceName);

            FileHelper.WriteToFile(getOutputFilePath(table),content);
        }

        static IReadOnlyList<string> GetRepositoryMethods(Table table,  Func<Column, string> getDotNetTypeName)
        {
            var getColumnByName = Fun((string name) => table.Columns.ToEnumeration().First(x => x.Name == name));

            var getLinesOfIndexMethod = Fun((Index index) =>
            {
                var lines = new List<string>();

                var columnNames = index.IndexedColumns.ToEnumeration().Select(x => x.Name).ToList();
                var parameters  = index.IndexedColumns.ToEnumeration().Select(x => new {Name = GetVariableName(x.Name), DotNetType = getDotNetTypeName(getColumnByName(x.Name))});

                var sqlPart       = $"SELECT * FROM {table.Name} WITH(NOLOCK) WHERE {Join(" AND ", columnNames.Select(x => x + " = @" + GetVariableName(x)))}";
                var parameterPart = $"new {{ {Join(",", columnNames.Select(c => GetVariableName(c)))} }}";

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



            return JoinMethodLines(table.Indexes.ToEnumeration().Select(getLinesOfIndexMethod));

          
        }

        static List<string> JoinMethodLines(IEnumerable<List<string>> parts)
        {
            var returnList = new List<string>();

            var i = 0;
            foreach (var items in parts)
            {
                if (i>0)
                {
                    returnList.Add(Empty);
                }

                returnList.AddRange(items);
                i++;
            }

            return returnList;
        }
        
        
        public static string GetRepositoryFileContent(Table table, Func<IReadOnlyList<string>> getMethods, Func<Table,string> getClassName,Func<Table,string> getNamespaceName)
        {
            var lines = new List<string>();

            lines.Add( "using System;");
            lines.Add("using Dapper.Contrib.Extensions;");
            
            lines.Add(Empty);
            lines.Add($"namespace {getNamespaceName(table)}");
            lines.Add("{");
            lines.Add($"partial class {getClassName(table)}");
            lines.Add("{");

            lines.AddRange(getMethods());

            lines.Add("}");
            lines.Add("}");

            return FileContentWriter.GetFileContent(lines);
        }
    }
}