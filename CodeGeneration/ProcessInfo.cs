using System;

namespace DapperEntityGenerator.CodeGeneration
{
    /// <summary>
    ///     The process information
    /// </summary>
    [Serializable]
    public sealed class ProcessInfo
    {
        #region Public Properties
        /// <summary>
        ///     Gets or sets the percent.
        /// </summary>
        public int Percent { get; set; }
        /// <summary>
        ///     Gets or sets the trace.
        /// </summary>
        public string Trace { get; set; }
        #endregion
    }
}