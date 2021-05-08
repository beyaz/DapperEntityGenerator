using System;

namespace DapperEntityGenerator.BootstrapperInfrastructure
{
    /// <summary>
    ///     The tracer
    /// </summary>
    class Tracer
    {
        #region Public Methods
        /// <summary>
        ///     Traces the specified message.
        /// </summary>
        public virtual void Trace(string message)
        {
            Console.WriteLine(message);
        }
        #endregion
    }
}