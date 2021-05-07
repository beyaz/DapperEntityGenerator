using System.Collections.Generic;
using Microsoft.SqlServer.Management.Smo;

namespace DapperEntityGenerator.CodeGeneration
{
    static class RelationHelper
    {
        public static IReadOnlyList<string> GetRelatedDataContract(Table table, IReadOnlyList<Table> searchTables)
        {
            var lines = new List<string>();

            if (!table.HasPrimaryClusteredIndex)
            {
                return lines;
            }

            var foreignKeyTables = ForeignKeyHelper.TryGetForeignKeyInfo(table, searchTables);
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
    }
}