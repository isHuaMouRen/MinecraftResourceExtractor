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
        public async static void Show(string title, string message, Exception ex, bool isFatalExcepiton = false)
        {
            var progressBarColor = new SolidColorBrush(Color.FromRgb(0, 0, 0));

            if (isFatalExcepiton)
                progressBarColor = new SolidColorBrush(Color.FromRgb(255, 10, 10));
            else
                progressBarColor = new SolidColorBrush(Color.FromRgb(255, 100, 100));                  


            var dialog = new ContentDialog
            {
                Title = title,
                Content = new System.Windows.Controls.StackPanel
                {
                    Children =
                    {
                        new System.Windows.Controls.ProgressBar
                        {
                            Foreground = progressBarColor,
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
                            Text = ex.ToString(),
                            IsReadOnly = true
                        }
                    }
                },
                PrimaryButtonText = "继续",
                SecondaryButtonText = "终止",
                DefaultButton = ContentDialogButton.Primary
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Secondary) 
                Environment.Exit(1);
            
        }
    }
}
