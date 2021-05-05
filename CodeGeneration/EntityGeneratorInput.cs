using System;

namespace DapperEntityGenerator.CodeGeneration
{
    [Serializable]
    public sealed class EntityGeneratorInput
    {
        #region Public Properties
        public string ConnectionString { get; set; }
        public string CSharpOutputFilePath { get; set; }
        public string DatabaseName { get; set; }
        public string ExportTableNames { get; set; }
        public string NamespacePattern { get; set; }
        public string SchemaName { get; set; }

        public string CSharpOutputFilePathForRepository { get; set; }
        public string NamespacePatternForRepository { get; set; }
        public string ClassNamePatternForRepository { get; set; }
        #endregion
    }
}