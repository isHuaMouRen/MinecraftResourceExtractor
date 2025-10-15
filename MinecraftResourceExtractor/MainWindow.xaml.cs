using Microsoft.Win32;
using MinecraftResourceExtractor.Class;
using MinecraftResourceExtractor.Utils;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ToolLib.Library.AutoStartLib;
using ToolLib.Library.CmdLib;
using ToolLib.Library.DownloaderLib;
using ToolLib.Library.GdiToolLib;
using ToolLib.Library.HashLib;
using ToolLib.Library.HexLib;
using ToolLib.Library.HotkeyManagerLib;
using ToolLib.Library.IniLib;
using ToolLib.Library.InputLib;
using ToolLib.Library.JsonLib;
using ToolLib.Library.KeyboardHookLib;
using ToolLib.Library.LogLib;
using ToolLib.Library.MemoryLib;
using static MinecraftResourceExtractor.Class.JsonConfig;
using NAudio.Vorbis;
using NAudio.Wave;

namespace MinecraftResourceExtractor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Func
        public async void Initialize()
        {
            try
            {
                //初始化配置文件
                if (!File.Exists(ConfigPath))
                {
                    GlobalConfig = new JsonConfig.Config.Root
                    {
                        isFirstUse = true
                    };
                    Json.WriteJson(ConfigPath, GlobalConfig);
                }

                //读配置文件
                GlobalConfig = Json.ReadJson<JsonConfig.Config.Root>(ConfigPath);

                if (GlobalConfig.isFirstUse)
                {
                    GlobalConfig.isFirstUse = false;
                    Json.WriteJson(ConfigPath, GlobalConfig);

                    var dialog = await new ContentDialog
                    {
                        Title = "欢迎",
                        Content = "欢迎使用 Minecraft散列资源提取器",
                        PrimaryButtonText = "继续",
                        DefaultButton = ContentDialogButton.Primary
                    }.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                ErrorReportBox.Show("发生错误", "在初始化程序时发生错误", ex);
            }
        }

        public async Task LoadMinecraftInfo()
        {
            try
            {
                //加载索引文件
                string[] indexFiles = Directory.GetFiles($"{minecraftPath}\\assets\\indexes");                
                string[] indexFileNames = new string[indexFiles.Length];
                if (indexFiles.Length == 0)
                    throw new Exception("未检测到任何索引文件，请至少下载一个Minecraft实例并启动一次");

                for (int i = 0; i < indexFiles.Length; i++)                
                    indexFileNames[i] = System.IO.Path.GetFileName(indexFiles[i]);

                IndexConfig = Json.ReadJson<JsonConfig.IndexFile.Root>($"{minecraftPath}\\assets\\indexes\\{await IndexSelectorBox.Show(indexFileNames)}");

                //添加进tree
                treeView_Main.Items.Clear();
                TreeViewControler.LoadIndexIntoTree(treeView_Main, IndexConfig);
                

                border_Main.IsEnabled = true;

            }
            catch (Exception ex)
            {
                border_Main.IsEnabled = false;
                ErrorReportBox.Show("致命错误", $"在加载索引列表时发生错误，请选择正确的 .minecraft 文件夹\n\n{ex.Message}", ex, true);
            }
        }

        public async Task LoadFileInfoAsync(TreeViewItem item)
        {
            try
            {
                // 停止之前的音频
                if (_audioOutput != null)
                {
                    _audioOutput.Stop();
                    _audioOutput.Dispose();
                    _audioOutput = null;
                }
                if (_audioReader != null)
                {
                    _audioReader.Dispose();
                    _audioReader = null;
                }

                label_FileTitle.Content = item.Header;
                label_FilePath.Content = TreeViewControler.GetTreeItemPath(item);

                if (item.Items.Count == 0)
                {
                    label_FileSize.Content = $"文件大小: {Math.Round(IndexConfig.objects![(string)label_FilePath.Content].size / (1024.0 * 1024.0), 3)}MB";
                    button_Extract.IsEnabled = true;

                    bool isLoadDone = false;
                    string filePath = label_FilePath.Content.ToString()!;
                    string hashFilePath = $"{minecraftPath}\\assets\\objects\\{IndexConfig.objects[(string)label_FilePath.Content].hash!.Substring(0, 2)}\\{IndexConfig.objects[(string)label_FilePath.Content].hash}";

                    if (filePath.EndsWith(".json") || filePath.EndsWith(".lang") || filePath.EndsWith(".mcmeta"))
                    {
                        isLoadDone = true;
                        label_PreviewError.Visibility = Visibility.Hidden;
                        textBox_Preview.Visibility = Visibility.Visible;
                        image_Preview.Visibility = Visibility.Hidden;

                        // 异步读取文本文件
                        textBox_Preview.Text = await Task.Run(() => File.ReadAllText(hashFilePath));
                    }
                    else if (filePath.EndsWith(".png"))
                    {
                        isLoadDone = true;
                        label_PreviewError.Visibility = Visibility.Hidden;
                        textBox_Preview.Visibility = Visibility.Hidden;
                        image_Preview.Visibility = Visibility.Visible;

                        // 异步加载图片
                        var bitmapSource = await Task.Run(() =>
                        {
                            var bmp = new BitmapImage();
                            bmp.BeginInit();
                            bmp.UriSource = new Uri(hashFilePath, UriKind.Absolute);
                            bmp.CacheOption = BitmapCacheOption.OnLoad; // 防止文件被占用
                            bmp.EndInit();
                            bmp.Freeze(); // 跨线程访问
                            return bmp;
                        });

                        image_Preview.Source = bitmapSource;
                    }
                    else if (filePath.EndsWith(".ogg"))
                    {
                        isLoadDone = true;
                        label_PreviewError.Visibility = Visibility.Visible;
                        textBox_Preview.Visibility = Visibility.Hidden;
                        image_Preview.Visibility = Visibility.Hidden;
                        label_PreviewError.Content = $"正在播放\n{label_FilePath.Content}";

                        // 异步创建音频播放实例
                        await Task.Run(() =>
                        {
                            _audioReader = new VorbisWaveReader(hashFilePath);
                            _audioOutput = new WaveOutEvent();
                            _audioOutput.Init(_audioReader);
                            _audioOutput.Play();
                        });
                    }

                    if (!isLoadDone)
                    {
                        label_PreviewError.Visibility = Visibility.Visible;
                        textBox_Preview.Visibility = Visibility.Hidden;
                        image_Preview.Visibility = Visibility.Hidden;
                    }
                }
                else
                {
                    label_FileSize.Content = $"文件大小: 无";
                    label_PreviewError.Visibility = Visibility.Visible;
                    textBox_Preview.Visibility = Visibility.Hidden;
                    image_Preview.Visibility = Visibility.Hidden;
                    button_Extract.IsEnabled = false;
                    label_PreviewError.Content = $"不支持的预览格式";
                }
            }
            catch (Exception ex)
            {
                ErrorReportBox.Show("发生错误", "加载文件信息时发生错误", ex);
            }
        }

        #endregion

        #region Class
        #endregion

        #region Obj
        private WaveOutEvent? _audioOutput;
        private VorbisWaveReader? _audioReader;
        #endregion

        #region Var
        public static string RunPath = Directory.GetCurrentDirectory();
        public static string ConfigPath = $"{RunPath}\\config.json";
        public static string OutputPath = $"{RunPath}\\Output";
        public static string minecraftPath = null!;

        public static JsonConfig.Config.Root GlobalConfig = null!;
        public static JsonConfig.IndexFile.Root IndexConfig = null!;
        #endregion


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize();
        }

        private void button_minecraftPath_Broswer_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(async () =>
            {
                try
                {
                    var dialog = new OpenFolderDialog();
                    dialog.Title = "请选择.minecraft文件夹";
                    dialog.Multiselect = false;
                    if (dialog.ShowDialog() == true)
                    {
                        minecraftPath = dialog.FolderName;
                        textBox_minecraftPath.Text = minecraftPath;
                        await LoadMinecraftInfo();
                    }
                }
                catch (Exception ex)
                {
                    ErrorReportBox.Show("发生错误", "在应用Minecraft路径时发生错误", ex);
                }
                
            });
        }

        private void treeView_Main_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Dispatcher.BeginInvoke(async () =>
            {
                if (treeView_Main.SelectedItem is TreeViewItem item)
                {
                    await LoadFileInfoAsync(item);
                }
                
            });
        }

        private void button_Search_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    TreeViewControler.FilterTree(treeView_Main, (string)textBox_Search.Text);
                }
                catch (Exception ex)
                {
                    ErrorReportBox.Show("发生错误", $"在过滤 treeView 项时发生错误\n\n{ex.Message}", ex);
                }
            });
        }

        private void button_Extractor_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(async () =>
            {
                try
                {
                    var dialog = new OpenFolderDialog
                    {
                        Title = "选择要保存的位置"
                    };
                    if (dialog.ShowDialog() == true)
                    {
                        string savePath = $"{dialog.FolderName}\\{System.IO.Path.GetFileName(label_FilePath.Content.ToString())}";
                        string hashFilePath = $"{minecraftPath}\\assets\\objects\\{IndexConfig.objects![(string)label_FilePath.Content].hash!.Substring(0, 2)}\\{IndexConfig.objects[(string)label_FilePath.Content].hash}";

                        File.Copy(hashFilePath, savePath, true);

                        var dialog2 = new ContentDialog
                        {
                            Title = "导出成功",
                            Content = new StackPanel
                            {
                                Children =
                                {
                                    new System.Windows.Controls.ProgressBar
                                    {
                                        Foreground=new SolidColorBrush(Color.FromRgb(100,255,100)),
                                        Value=100,
                                        Margin=new Thickness(0,0,0,10)
                                    },
                                    new Label
                                    {
                                        Content=$"文件已保存至 {savePath}",
                                        Margin=new Thickness(0,0,0,10)
                                    }
                                }
                            },
                            PrimaryButtonText = "确定",
                            SecondaryButtonText = "定位",
                            DefaultButton = ContentDialogButton.Primary
                        };
                        if (await dialog2.ShowAsync() == ContentDialogResult.Secondary)
                            Process.Start("explorer.exe", $"/select,\"{savePath}\"");
                    }
                }
                catch (Exception ex)
                {
                    ErrorReportBox.Show("发生错误", "在尝试导出资源时发生错误", ex);
                }
            });
        }

        private void button_ExtractALL_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(async () =>
            {
                try
                {
                    // 确认提示
                    var isSaveDialog = new ContentDialog
                    {
                        Title = "提示",
                        Content = "是否导出所有文件\n\n(注意: 此操作可能会导致程序看起来未响应，实际上程序在努力导出文件)",
                        PrimaryButtonText = "是",
                        CloseButtonText = "否",
                        DefaultButton = ContentDialogButton.Primary
                    };
                    if (await isSaveDialog.ShowAsync() != ContentDialogResult.Primary)
                        return;

                    // 选择输出目录
                    var dialog = new OpenFolderDialog { Title = "选择导出位置" };
                    if (dialog.ShowDialog() != true) return;
                    string outputFolder = dialog.FolderName;

                    // 用于记录导出失败的文件
                    List<string> failedFiles = new List<string>();

                    // 递归函数：传入当前路径前缀
                    void ExportItems(ItemCollection items, string parentPath)
                    {
                        foreach (TreeViewItem item in items)
                        {
                            string currentPath = string.IsNullOrEmpty(parentPath) ? item.Header.ToString()!: $"{parentPath}/{item.Header}";

                            if (item.Items.Count == 0) // 文件节点
                            {
                                try
                                {
                                    if (IndexConfig.objects != null && IndexConfig.objects.TryGetValue(currentPath, out var fileInfo))
                                    {
                                        string hash = fileInfo.hash!;
                                        string sourcePath = $"{minecraftPath}\\assets\\objects\\{hash.Substring(0, 2)}\\{hash}";
                                        string targetPath = System.IO.Path.Combine(outputFolder, currentPath.Replace('/', '\\'));

                                        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(targetPath)!);
                                        File.Copy(sourcePath, targetPath, true);
                                    }
                                }
                                catch
                                {
                                    failedFiles.Add(currentPath);
                                }
                            }
                            else
                            {
                                ExportItems(item.Items, currentPath);
                            }
                        }
                    }

                    ExportItems(treeView_Main.Items, "");

                    var doneDialog = new ContentDialog
                    {
                        Title = "导出完成",
                        Content = $"操作完成，所有可导出的文件已导出至 {outputFolder}\n\n其中有 {failedFiles.Count} 个文件导出失败",
                        PrimaryButtonText = "确定",
                        SecondaryButtonText = "定位",
                        DefaultButton = ContentDialogButton.Primary
                    };

                    if (await doneDialog.ShowAsync() == ContentDialogResult.Secondary)
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = outputFolder,
                            UseShellExecute = true
                        });
                    }
                }
                catch (Exception ex)
                {
                    ErrorReportBox.Show("发生错误", "导出所有文件时发生错误", ex);
                }
            });
        }
    }
}
