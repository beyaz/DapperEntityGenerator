using System;
using System.Collections.Generic;
using System.IO;
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




        public static void ExportTable(Table table,  
                                       Func<Column, string> getDotNetTypeName,
                                       Func<Table,string> getEntityNamespaceName,
                                       Func<Table,string> getRepositoryClassName,
                                       Func<Table,string> getRepositoryNamespaceName,
                                       Func<Table,string> getRepositoryOutputFilePath)
        {
            var getMethods = Fun(() => GetRepositoryMethods(table, getDotNetTypeName));
            
            var content = GetRepositoryFileContent(table, getMethods,getRepositoryClassName,getRepositoryNamespaceName,getEntityNamespaceName);

            FileHelper.WriteToFile(getRepositoryOutputFilePath(table),content);
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

                    lines.Add($"return dbConnection.QueryFirstOrDefault<{table.Name}>(\"{sqlPart}\", {parameterPart});");

                    lines.Add("}");

                    return lines;
                }

                

                    lines.Add($"public IReadOnlyList<{table.Name}> Get{table.Name}By{Join("And", columnNames)}({Join(",", parameters.Select(p => p.DotNetType + " " + p.Name))})");
                    lines.Add("{");

                    lines.Add($"return dbConnection.Query<{table.Name}>(\"{sqlPart}\", {parameterPart}).ToList();");

                    lines.Add("}");

                    return lines;
                

            });

            var getUpdateLines = Fun(() =>
            {
                var lines = new List<string>();

                lines.Add($"public {table.Name} Update({table.Name} {GetVariableName(table.Name)})");
                lines.Add("{");
                lines.Add($"dbConnection.Update({GetVariableName(table.Name)}, dbTransaction);");
                lines.Add($"return {GetVariableName(table.Name)};");
                lines.Add("}");

                return lines;
            });

            var getInsertLines = Fun(() =>
            {
                var lines = new List<string>();

                lines.Add($"public {table.Name} Insert({table.Name} {GetVariableName(table.Name)})");
                lines.Add("{");
                lines.Add($"dbConnection.Insert({GetVariableName(table.Name)}, dbTransaction);");
                lines.Add($"return {GetVariableName(table.Name)};");
                lines.Add("}");

                return lines;
            });

            var methods = table.Indexes.ToEnumeration().Select(getLinesOfIndexMethod).ToList();

            methods.Add(getInsertLines());

            if (table.HasPrimaryClusteredIndex)
            {
                methods.Add(getUpdateLines());   
            }

            return JoinMethodLines(methods);
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
        
        
        static string GetRepositoryFileContent(Table table, Func<IReadOnlyList<string>> getMethods, Func<Table,string> getClassName, Func<Table,string> getNamespaceName,Func<Table,string> getEntityNamespaceName)
        {
            var lines = new List<string>();

            lines.Add("using System.Linq;");
            lines.Add("using System.Collections.Generic;");
            lines.Add("using Dapper;");
            lines.Add("using Dapper.Contrib.Extensions;");
            if (getEntityNamespaceName(table) != getNamespaceName(table))
            {
                lines.Add($"using {getEntityNamespaceName(table)};");    
            }
            
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

        public static void ExportRepositoryMainClassDecleration(string namespaceName,string className,string outputFilePath)
        {
            outputFilePath = outputFilePath.Replace("{TableName}", "");
            
            outputFilePath = Path.Combine(Path.GetDirectoryName(outputFilePath), className + ".cs");

            var lines = new List<string>
            {
                "using System;", 
                "using System.Data;",
                string.Empty,
                $"namespace {namespaceName}",
                "{",
                $"public partial class {className}",
                "{", 
                "readonly IDbConnection dbConnection;",
                "readonly IDbTransaction dbTransaction;",
                string.Empty, 
                $"public {className}(IDbConnection dbConnection, IDbTransaction dbTransaction)", 
                "{",
                "if (dbConnection == null)",
                "{",
                "throw new ArgumentNullException(nameof(dbConnection));", 
                "}",
                string.Empty,
                "this.dbConnection  = dbConnection;",
                "this.dbTransaction = dbTransaction;", "}",
                "}",
                "}"
            };

            var content = FileContentWriter.GetFileContent(lines);

            FileHelper.WriteToFile(outputFilePath,content);
        }
    }
}