namespace DapperEntityGenerator.BootstrapperInfrastructure
{
    /// <summary>
    ///     The module loader
    /// </summary>
    static class ModuleLoader
    {
        #region Public Methods
        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public static void Load()
        {
            ProcessKiller.KillSameProcesesExceptCurrent();

            EmbeddedCompressedAssemblyReferencesResolver.Resolve("EmbeddedReferences.zip");
        }
        #endregion
    }
}