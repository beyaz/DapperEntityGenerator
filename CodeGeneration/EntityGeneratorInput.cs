using System;

namespace DapperEntityGenerator.CodeGeneration
{
    /// <summary>
    ///     The entity generator input
    /// </summary>
    [Serializable]
    public sealed class EntityGeneratorInput
    {
        #region Public Properties
        /// <summary>
        ///     Gets or sets the connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        ///     Gets or sets the c sharp output file path for entity.
        /// </summary>
        public string CSharpOutputFilePathForEntity { get; set; }

        /// <summary>
        ///     Gets or sets the name of the database.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        ///     Gets or sets the export table names.
        /// </summary>
        public string ExportTableNames { get; set; }

        /// <summary>
        ///     Gets or sets the namespace pattern for entity.
        /// </summary>
        public string NamespacePatternForEntity { get; set; }

        /// <summary>
        ///     Gets or sets the name of the schema.
        /// </summary>
        public string SchemaName { get; set; }
        #endregion
    }
}