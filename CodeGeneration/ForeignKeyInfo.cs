sing Microsoft.SqlServer.Management.Smo;

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
}