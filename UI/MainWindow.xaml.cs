using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using DapperEntityGenerator.CodeGeneration;
using static DapperEntityGenerator.Extensions;
using static DapperEntityGenerator.UI.Layout;
using static DapperEntityGenerator.UI.WpfExtensions;

namespace DapperEntityGenerator.UI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        ProgressBar progressBar = new ProgressBar
        {
            Value  = 0,
            Height = 10
        };

        TextBlock traceText = new TextBlock
        {
            Text = "Ready"
        };
        #endregion

        #region Constructors
        public MainWindow()
        {
            Model = CacheHelper.GetMainWindowModelFromCache();

            InitializeComponent();

            FrameworkElement createInput(string bindingPath)
            {
                return NewSimpleInputEditor(bindingPath, Model, bindingPath);
            }

            Content = CreateCardContainer(new Card
                                          {
                                              Header = "Actions",
                                              Childs = new[]
                                              {
                                                  NewStackPanel(HorizontalWithDefaultIndent, NewActionButton("Generate", "Generating...", GenerateClicked))
                                              }
                                          },
                                          new Card
                                          {
                                              Header = "Config",
                                              Childs = new[]
                                              {
                                                  createInput(nameof(EntityGeneratorInput.ConnectionString)),
                                                  createInput(nameof(EntityGeneratorInput.DatabaseName)),
                                                  createInput(nameof(EntityGeneratorInput.SchemaName)),
                                                  createInput(nameof(EntityGeneratorInput.NamespacePattern)),
                                                  createInput(nameof(EntityGeneratorInput.CSharpOutputFilePath)),
                                                  createInput(nameof(EntityGeneratorInput.ExportTableNames))
                                              }
                                          },
                                          new Card
                                          {
                                              Header = "Trace",
                                              Childs = new[]
                                              {
                                                  NewStackPanel(VerticalWithMiniIndent, traceText, progressBar)
                                              }
                                          });

            Closing += (s, e) => { CacheHelper.SaveMainWindowModelToCache(Model); };
        }
        #endregion

        #region Public Properties
        public EntityGeneratorInput Model { get; set; }

        public ProcessInfo ProcessInfo { get; set; } = new ProcessInfo {Trace = "Ready", Percent = 0};
        #endregion

        #region Methods
        void GenerateClicked(Action onFinished)
        {
            void updateProcessIndicator()
            {
                traceText.Text    = ProcessInfo.Trace;
                progressBar.Value = ProcessInfo.Percent;

                Thread.Sleep(100);
            }

            var timer = CreateTimer(Dispatcher, updateProcessIndicator);

            void generate()
            {
                EntityGenerator.GenerateSchema(Model, ProcessInfo);

                UpdateUiAfterSleep(Dispatcher, 100, updateProcessIndicator);

                timer.Stop();

                onFinished();
            }

            timer.Start();

            StartThread(generate);
        }
        #endregion
    }
}