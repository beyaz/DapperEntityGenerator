using System.Collections.Generic;
using System.Windows;

namespace DapperEntityGenerator.UiHelpers
{
    /// <summary>
    ///     The card
    /// </summary>
    public sealed class Card
    {
        #region Public Properties
        /// <summary>
        ///     Gets or sets the childs.
        /// </summary>
        public IReadOnlyList<FrameworkElement> Childs { get; set; }
        /// <summary>
        ///     Gets or sets the header.
        /// </summary>
        public string Header { get; set; }
        /// <summary>
        ///     Gets or sets a value indicating whether this instance is horizontal.
        /// </summary>
        public bool IsHorizontal { get; set; }
        #endregion
    }
}