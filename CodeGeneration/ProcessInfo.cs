using System;

namespace DapperEntityGenerator.CodeGeneration
{
    [Serializable]
    public sealed class ProcessInfo
    {
        #region Public Properties
        public int Percent { get; set; }
        public string Trace { get; set; }
        #endregion
    }
}