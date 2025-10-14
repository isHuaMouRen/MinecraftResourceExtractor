using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;

namespace MinecraftResourceExtractor.Utils
{
    public static class ErrorReportBox
    {
        public async static void Show(string title, string message, Exception ex)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = new System.Windows.Controls.StackPanel
                {
                    Children =
                    {
                        new System.Windows.Controls.ProgressBar
                        {
                            Foreground = new SolidColorBrush(Color.FromRgb(255,100,100)),
                            Value = 100,
                            Margin = new System.Windows.Thickness(0,0,0,10)
                        },
                        new Label
                        {
                            Content = message,
                            Margin = new Thickness(0,0,0,10)
                        },
                        new TextBox
                        {
                            Text = ex.ToString()
                        }
                    }
                },
                PrimaryButtonText = "继续",
                SecondaryButtonText = "退出",
                DefaultButton = ContentDialogButton.Primary
            };
            if (await dialog.ShowAsync() == ContentDialogResult.Secondary)
                Environment.Exit(1);
        }
    }
}
