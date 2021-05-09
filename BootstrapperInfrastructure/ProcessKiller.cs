using System.Diagnostics;
using System.Linq;

namespace DapperEntityGenerator.BootstrapperInfrastructure
{
    /// <summary>
    ///     The process killer
    /// </summary>
    static class ProcessKiller
    {
        #region Public Methods

        /// <summary>
        ///     Kills the same proceses except current.
        /// </summary>
        public static void KillSameProcesesExceptCurrent()
        {
            var self = Process.GetCurrentProcess();

            foreach (var p in Process.GetProcessesByName(self.ProcessName).Where(p => p.Id != self.Id))
            {
                p.Kill();
            }
        }
        #endregion
    }
}