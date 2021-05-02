using System.Windows;

namespace DapperEntityGenerator.UI
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Methods
        void OnStartup(object sender, StartupEventArgs e)
        {
            MainWindow = new MainWindow();

            MainWindow.Show();
        }
        #endregion
    }
}