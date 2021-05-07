using System.Collections.Generic;
using Microsoft.SqlServer.Management.Smo;

namespace DapperEntityGenerator.CodeGeneration
{
    static class ForeignKeyHelper
    {
        public static IReadOnlyList<ForeignKeyInfo> TryGetForeignKeyInfo(Table source, IReadOnlyList<Table> searchTables)
        {
            var items = new List<ForeignKeyInfo>();

            foreach (var target in searchTables)
            {
                var foreignKeyInfo = ForeignKeyHelper.TryGetForeignKeyInfo(source, target);
                if (foreignKeyInfo != null)
                {
                    items.Add(foreignKeyInfo);
                }
            }

            return items;
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
    }
}