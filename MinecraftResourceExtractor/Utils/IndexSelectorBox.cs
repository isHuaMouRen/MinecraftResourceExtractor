using ModernWpf.Controls;
using ModernWpf.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MinecraftResourceExtractor.Utils
{
    public static class IndexSelectorBox
    {
        public async static Task<string> Show(string[] fileNames)
        {
            while (true)
            {
                var listBox = new ListBox
                {
                    MaxHeight = 200
                };
                ScrollViewer.SetVerticalScrollBarVisibility(listBox, ScrollBarVisibility.Auto);
                
                for (int i = 0; i < fileNames.Length; i++)
                {
                    listBox.Items.Add(fileNames[i]);
                }

                listBox.SelectedItem = null;

                var dialog = new ContentDialog
                {
                    Title = "选择一个索引文件",
                    Content = new StackPanel
                    {
                        Children = {
                        new System.Windows.Controls.ProgressBar
                        {
                            Value=100,
                            Margin=new System.Windows.Thickness(0,0,0,10)
                        },
                        new HyperlinkButton
                        {
                            Content="如何选择索引文件",
                            NavigateUri=new Uri("https://zh.minecraft.wiki/w/%E6%95%A3%E5%88%97%E8%B5%84%E6%BA%90%E6%96%87%E4%BB%B6#%E7%B4%A2%E5%BC%95%E5%90%8D%E7%A7%B0"),
                            Margin=new System.Windows.Thickness(0,0,0,10)
                        },
                        listBox
                        }
                    },
                    PrimaryButtonText = "确定"
                };
                if (await dialog.ShowAsync() != ContentDialogResult.Primary || listBox.SelectedItem == null)
                {
                    var dialog2 = await new ContentDialog
                    {
                        Title = "提示",
                        Content = "必须选择一项",
                        PrimaryButtonText = "确定",
                        DefaultButton = ContentDialogButton.Primary
                    }.ShowAsync();
                }
                else
                {
                    if (listBox.SelectedItem is string item)
                    {
                        return item;
                    }
                }
            }
        }
    }
}
