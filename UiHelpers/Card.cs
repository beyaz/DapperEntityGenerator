using System.Collections.Generic;
using System.Windows;

namespace DapperEntityGenerator.UI
{
    public sealed class Card
    {
        #region Public Properties
        public IReadOnlyList<FrameworkElement> Childs { get; set; }
        public string Header { get; set; }
        public bool IsHorizontal { get; set; }
        #endregion
    }
}