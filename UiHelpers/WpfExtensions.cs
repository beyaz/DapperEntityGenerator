using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace DapperEntityGenerator.UI
{
    static class WpfExtensions
    {
        #region Constants
        const int DefaultMargin = 10;
        const int MiniIndent = 5;
        #endregion

        #region Properties
        static Brush LabelBrush => ToBrush("#596B75");
        #endregion

        #region Public Methods
        public static void Bind(DependencyObject target, DependencyProperty dependencyProperty, object source, string propertyPath)
        {
            BindingOperations.SetBinding(target, dependencyProperty, new Binding
            {
                Source              = source,
                Path                = new PropertyPath(propertyPath),
                Mode                = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
        }

        public static Border Create(Card data)
        {
            var stackPanel = new StackPanel
            {
                Background  = ToBrush("White"),
                Margin      = new Thickness(10),
                Orientation = data.IsHorizontal ? Orientation.Horizontal : Orientation.Vertical
            };

            var border = new Border
            {
                CornerRadius = new CornerRadius(10),
                Background   = ToBrush("White"),

                Effect = new DropShadowEffect
                {
                    BlurRadius  = 20,
                    Color       = Colors.DarkGray,
                    Opacity     = 0.4,
                    Direction   = 280,
                    ShadowDepth = 0
                },
                Child = stackPanel
            };

            stackPanel.Children.Add(new TextBlock {FontSize = 16, Foreground = LabelBrush, Text = data.Header});

            foreach (UIElement element in data.Childs ?? new FrameworkElement[0])
            {
                stackPanel.Children.Add(element);
            }

            WithIndent(stackPanel, 15);

            return border;
        }

        public static Panel CreateCardContainer(params Card[] cardPanels)
        {
            var cards = cardPanels.ToList().ConvertAll(Create);

            var container = new StackPanel
            {
                Background = ToBrush("#F5F6FA")
            };

            cards.ForEach(p => container.Children.Add(p));

            var margin = 10;

            foreach (FrameworkElement element in container.Children)
            {
                element.Margin = new Thickness(margin, 10, margin, element.Margin.Bottom);
            }

            var last = cards.Last();

            last.Margin = new Thickness(last.Margin.Left, last.Margin.Top, last.Margin.Right, margin);

            return container;
        }

        public static Timer CreateTimer(Dispatcher dispatcher, Action onTimeElapsed)
        {
            var timer = new Timer(200);

            timer.Elapsed += (s, e) => { dispatcher.BeginInvoke(onTimeElapsed); };

            return timer;
        }

        public static FrameworkElement NewActionButton(string label, string clickedLabel, Action<Action> onClick)
        {
            var textBlock = new TextBlock
            {
                Text   = label, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10)
            };

            var border = new Border
            {
                CornerRadius    = new CornerRadius(10),
                BorderThickness = new Thickness(1),
                BorderBrush     = ToBrush("#A4ADB2"),
                Child           = textBlock
            };

            void onFinish()
            {
                UpdateUiAfterSleep(textBlock.Dispatcher, 10, () => textBlock.Text = label);
            }

            ;

            border.MouseLeftButtonDown += (s, e) =>
            {
                textBlock.Text = clickedLabel;

                onClick(onFinish);
            };

            return border;
        }

        public static FrameworkElement NewSimpleInputEditor(string label, object model, string bindingPath)
        {
            TextBlock NewLabel(string text)
            {
                return new TextBlock {FontWeight = FontWeights.Bold, Text = text, Foreground = LabelBrush};
            }

            var textBox = new TextBox
            {
                BorderThickness = new Thickness(0)
            };

            Bind(textBox, TextBox.TextProperty, model, bindingPath);

            var border = new Border
            {
                Margin          = new Thickness(0, 5, 0, 0),
                BorderThickness = new Thickness(1),
                BorderBrush     = ToBrush("Black"),
                CornerRadius    = new CornerRadius(5),
                Padding         = new Thickness(2),
                Child           = textBox
            };

            return NewStackPanel(Layout.VerticalWithMiniIndent, NewLabel(label), border);
        }

        public static StackPanel NewStackPanel(Layout layout, params FrameworkElement[] childElements)
        {
            var sp = new StackPanel();

            if (layout == Layout.Horizontal || layout == Layout.HorizontalWithDefaultIndent)
            {
                sp.Orientation = Orientation.Horizontal;
            }

            foreach (var element in childElements)
            {
                sp.Children.Add(element);
            }

            if (layout == Layout.HorizontalWithDefaultIndent)
            {
                WithIndent(sp, DefaultMargin);
            }

            if (layout == Layout.VerticalWithMiniIndent)
            {
                WithIndent(sp, MiniIndent);
            }

            return sp;
        }

        public static void UpdateUiAfterSleep(Dispatcher dispatcher, int sleepMilliseconds, Action action)
        {
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                await dispatcher.BeginInvoke(action);
            });
        }
        #endregion

        #region Methods
        static Brush ToBrush(string hexaDecimalColorValue)
        {
            return new SolidColorBrush((Color) ColorConverter.ConvertFromString(hexaDecimalColorValue));
        }

        static StackPanel WithIndent(this StackPanel stackPanel, int indent)
        {
            var i = 0;
            if (stackPanel.Orientation == Orientation.Vertical)
            {
                foreach (FrameworkElement element in stackPanel.Children)
                {
                    if (i > 0)
                    {
                        element.Margin = new Thickness(element.Margin.Left, indent, element.Margin.Right, element.Margin.Bottom);
                    }

                    i++;
                }
            }
            else
            {
                foreach (FrameworkElement element in stackPanel.Children)
                {
                    if (i > 0)
                    {
                        element.Margin = new Thickness(indent, element.Margin.Top, element.Margin.Right, element.Margin.Bottom);
                    }

                    i++;
                }
            }

            return stackPanel;
        }
        #endregion
    }
}