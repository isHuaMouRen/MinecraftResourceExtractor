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

using System;
using System.Collections.Generic;
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
using MinecraftResourceExtractor.Utils;
using System.IO;
using Microsoft.Win32;
using MinecraftResourceExtractor.Class;

namespace MinecraftResourceExtractor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Func
        public void Initialize()
        {
            try
            {
                //初始化配置文件
                if (!File.Exists(ConfigPath))
                {
                    GlobalConfig = new JsonConfig.Config.Root
                    {
                        MinecraftPath = null
                    };
                    Json.WriteJson(ConfigPath, GlobalConfig);
                }

                //读配置文件
                GlobalConfig = Json.ReadJson<JsonConfig.Config.Root>(ConfigPath);

                if (GlobalConfig.MinecraftPath != null)
                {
                    minecraftPath = GlobalConfig.MinecraftPath;
                    textBox_minecraftPath.Text = minecraftPath;
                    LoadMinecraftInfo();
                }
            }
            catch (Exception ex)
            {
                ErrorReportBox.Show("发生错误", "在初始化程序时发生错误", ex);
            }
        }

        public void LoadMinecraftInfo()
        {

        }
        #endregion

        #region Class
        #endregion

        #region Obj
        #endregion
        
        #region Var
        public static string RunPath = Directory.GetCurrentDirectory();
        public static string ConfigPath = $"{RunPath}\\config.json";
        public static string OutputPath = $"{RunPath}\\Output";
        public static string minecraftPath = null!;

        public static JsonConfig.Config.Root GlobalConfig = null!;
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
            Dispatcher.BeginInvoke(() =>
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
                        GlobalConfig.MinecraftPath = minecraftPath;
                        Json.WriteJson(ConfigPath, GlobalConfig);
                        LoadMinecraftInfo();
                    }
                }
                catch (Exception ex)
                {
                    ErrorReportBox.Show("发生错误", "在应用Minecraft路径时发生错误", ex);
                }
                
            });
        }
    }
}
