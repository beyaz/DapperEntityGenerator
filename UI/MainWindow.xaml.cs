using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using DapperEntityGenerator.CodeGeneration;
using DapperEntityGenerator.UiHelpers;
using static DapperEntityGenerator.Extensions;
using static DapperEntityGenerator.UiHelpers.Layout;
using static DapperEntityGenerator.UiHelpers.WpfExtensions;

namespace DapperEntityGenerator.UI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        /// <summary>
        ///     The progress bar
        /// </summary>
        readonly ProgressBar progressBar = new ProgressBar
        {
            Value  = 0,
            Height = 10
        };

        /// <summary>
        ///     The trace text
        /// </summary>
        readonly TextBlock traceText = new TextBlock
        {
            Text = "Ready"
        };
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
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
                                              Children = new[]
                                              {
                                                  NewStackPanel(HorizontalWithDefaultIndent, NewActionButton("Generate", "Generating...", GenerateClicked))
                                              }
                                          },
                                          new Card
                                          {
                                              Header = "Config For Database",
                                              Children = new[]
                                              {
                                                  createInput(nameof(EntityGeneratorInput.ConnectionString)),
                                                  createInput(nameof(EntityGeneratorInput.DatabaseName)),
                                                  createInput(nameof(EntityGeneratorInput.SchemaName)),
                                                  createInput(nameof(EntityGeneratorInput.ExportTableNames))
                                              }
                                          },
                                          new Card
                                          {
                                              Header = "Config For Entity",
                                              Children = new[]
                                              {
                                                  createInput(nameof(EntityGeneratorInput.NamespacePatternForEntity)),
                                                  createInput(nameof(EntityGeneratorInput.CSharpOutputFilePathForEntity))
                                              }
                                          },
                                          new Card
                                          {
                                              Header = "Trace",
                                              Children = new[]
                                              {
                                                  NewStackPanel(VerticalWithMiniIndent, traceText, progressBar)
                                              }
                                          });

            Closing += (s, e) => { CacheHelper.SaveMainWindowModelToCache(Model); };
        }
        #endregion

        #region Public Properties
        /// <summary>
        ///     Gets or sets the model.
        /// </summary>
        public EntityGeneratorInput Model { get; set; }

        /// <summary>
        ///     Gets or sets the process information.
        /// </summary>
        public ProcessInfo ProcessInfo { get; set; } = new ProcessInfo {Trace = "Ready", Percent = 0};
        #endregion

        #region Methods
        /// <summary>
        ///     Generates the clicked.
        /// </summary>
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