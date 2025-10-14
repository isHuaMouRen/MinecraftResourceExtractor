using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static MinecraftResourceExtractor.Class.JsonConfig;

namespace MinecraftResourceExtractor.Utils
{
    public static class TreeViewControler
    {
        /// <summary>
        /// 把索引添加进treeView
        /// </summary>
        /// <param name="treeView">控件</param>
        /// <param name="indexConfig">数据</param>
        public static void LoadIndexIntoTree(TreeView treeView, IndexFile.Root indexConfig)
        {
            if (indexConfig.objects == null) return;

            foreach (var kvp in indexConfig.objects)
            {
                string path = kvp.Key;
                AddPathToTree(treeView, path);
            }
        }

        private static void AddPathToTree(TreeView treeView, string path)
        {
            string[] parts = path.Split('/');
            ItemCollection currentLevel = treeView.Items;

            foreach (string part in parts)
            {
                TreeViewItem? node = null;
                foreach (TreeViewItem item in currentLevel)
                {
                    if (item.Header?.ToString() == part)
                    {
                        node = item;
                        break;
                    }
                }

                if (node == null)
                {
                    node = new TreeViewItem { Header = part };
                    currentLevel.Add(node);
                }

                currentLevel = node.Items;
            }
        }

        /// <summary>
        /// 按文本过滤树
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="searchText"></param>
        public static void FilterTree(TreeView treeView, string searchText)
        {
            foreach (TreeViewItem item in treeView.Items)
            {
                FilterTreeItem(item, searchText);
            }
        }

        private static bool FilterTreeItem(TreeViewItem item, string searchText)
        {
            bool isMatch = item.Header?.ToString()?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false;

            bool hasVisibleChild = false;
            foreach (TreeViewItem child in item.Items)
            {
                if (FilterTreeItem(child, searchText))
                {
                    hasVisibleChild = true;
                }
            }

            // 如果自己匹配或者有子项匹配，就显示
            item.Visibility = (isMatch || hasVisibleChild) ? Visibility.Visible : Visibility.Collapsed;

            return item.Visibility == Visibility.Visible;
        }

        /// <summary>
        /// 获得treeitem的位置
        /// </summary>
        /// <param name="item">项</param>
        /// <returns></returns>
        public static string GetTreeItemPath(TreeViewItem item)
        {
            if (item == null) return string.Empty;

            var pathParts = new Stack<string>();
            pathParts.Push(item.Header?.ToString() ?? "");

            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (parent != null)
            {
                if (parent is TreeViewItem parentItem)
                {
                    pathParts.Push(parentItem.Header?.ToString() ?? "");
                }
                parent = VisualTreeHelper.GetParent(parent);
            }

            return string.Join("/", pathParts);
        }
    }
}
