using System.Diagnostics;

namespace DapperEntityGenerator.BootstrapperInfrastructure
{
    /// <summary>
    ///     The process killer
    /// </summary>
    static class ProcessKiller
    {
        #region Public Methods
        /// <summary>
        ///     Kills all.
        /// </summary>
        public static void KillAll(string processName)
        {
            foreach (var process in Process.GetProcessesByName(processName))
            {
                process.Kill();
            }
        }
        #endregion
    }
}