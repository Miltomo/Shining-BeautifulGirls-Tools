using MHTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static MHTools.数据工具;

namespace Shining_BeautifulGirls
{
    /// <summary>
    /// 技能编辑.xaml 的交互逻辑
    /// </summary>
    public partial class 技能编辑 : Window
    {
        private static List<SkillItem> DefaultList { get; } = [];
        ListBox? DraggerSource { get; set; }
        List<ListBox> 技能组 { get; init; }
        SimpleFileManager FileManager { get; } =
            new(App.SkillStrategyDir, "技能组", "json");

        TextBlock? textBlockInEdit;
        TextBox? textBoxInEdit;
        class SkillItem
        {
            public string Name { get; set; }
            public string Path { get; set; }

            public override bool Equals(object? obj)
            {
                if (obj == null || GetType() != obj.GetType())
                    return false;

                var other = (SkillItem)obj;
                return Name == other.Name;
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode() ^ Path.GetHashCode();
            }
        }
        public static List<List<string>> GetPrioritySkillList(string name)
        {
            List<List<string>> pslist = [];
            var wd = new 技能编辑();
            if (wd.FileManager.Select(name))
            {
                wd.Load技能配置(wd.FileManager.SelectedFile!);
                wd.技能组.ForEach(lb =>
                pslist.Add(
                    App.Items2List<SkillItem>(lb.Items)
                    .Select(item => item.Name)
                    .ToList()
                )
                );
                wd.FileManager.CancelSelect();
            }
            wd.Close();
            return pslist;
        }
        public 技能编辑()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.CanResizeWithGrip;
            Height = 800;
            Width = 1200;
            技能组 = [级别1, 级别2, 级别3, 级别4];

            foreach (var skill in Directory.GetFiles(ShiningGirl.SkillDir))
                DefaultList.Add(new SkillItem { Name = Path.GetFileNameWithoutExtension(skill), Path = skill });

            文件列表.ItemsSource = FileManager.Names;
        }

        protected override void OnClosed(EventArgs e)
        {
            Save技能配置(FileManager.SelectedFile);
            if (App.UserWindow is not null)
            {
                App.UserWindow.IsEnabled = true;
                App.UserWindow.Show();
                (App.UserWindow as 用户界面)!.Refresh();
            }
            App.SkillWindow = null;
            base.OnClosed(e);
        }

        private void SwitchMode(bool edit = true)
        {
            if (textBlockInEdit != null && textBoxInEdit != null)
            {
                var dqFileName = edit ?
                    textBlockInEdit.Text :
                    textBoxInEdit.Text;

                textBoxInEdit.IsEnabled = edit;
                textBlockInEdit.Visibility = edit ? Visibility.Hidden : Visibility.Visible;
                textBoxInEdit.Visibility = edit ? Visibility.Visible : Visibility.Hidden;

                if (edit)
                {
                    textBoxInEdit.Text = dqFileName;
                    textBoxInEdit.Focus();
                    textBoxInEdit.CaretIndex = dqFileName.Length;
                }
                else
                {
                    if (FileManager.TryRename(dqFileName))
                        文件列表.ItemsSource = FileManager.Names;
                    textBlockInEdit = null;
                    textBoxInEdit = null;
                }
            }
        }

        private void RenameSkFile(object source)
        {
            // 获取目标项
            ListBoxItem? listBoxItem = App.FindVisualParent<ListBoxItem>(source as DependencyObject);

            if (listBoxItem != null)
            {
                // 获取项中的元素
                textBlockInEdit = App.FindVisualChild<TextBlock>(listBoxItem);
                textBoxInEdit = App.FindVisualChild<TextBox>(listBoxItem);

                SwitchMode();
            }
        }

        private void Save技能配置(string? configFile)
        {
            if (configFile != null)
            {
                Dictionary<string, object> dt = [];

                技能组.ForEach(x => dt[x.Name] = App.Items2List<SkillItem>(x.Items).Select(item => item.Name));

                SaveToJSON(dt, configFile);
            }
        }

        private void Load技能配置(string configFile)
        {
            var dt = LoadFromJSON<Dictionary<string, JsonElement>>(configFile);
            var complement = DefaultList;
            技能组.ForEach(x => x.Items.Clear());
            技能仓库.Items.Clear();

            if (dt != null)
            {
                技能组.ForEach(x =>
                {
                    var sks = TransJsonElement(dt[x.Name]) as List<object>;
                    sks?.ForEach(s =>
                    {
                        var name = s as string;
                        x.Items.Add(DefaultList.Where(x => x.Name == name).FirstOrDefault());
                    });
                });

                complement = DefaultList.Except(
                    技能组.Select(x => App.Items2List<SkillItem>(x.Items))
                    .SelectMany(list => list).Distinct()
                    ).ToList();
            }

            complement.ForEach(item => 技能仓库.Items.Add(item));
        }



        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 初始化拖动项
            ListBoxItem? draggedItem = App.FindVisualParent<ListBoxItem>((DependencyObject)e.OriginalSource);

            if (draggedItem != null)
            {
                DraggerSource = sender as ListBox;
                DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
            }
        }
        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            var draggerTarget = sender as ListBox;
            if (draggerTarget == DraggerSource)
                return;
            if (e.Data.GetDataPresent(typeof(SkillItem)))
            {
                var droppedData = (SkillItem)e.Data.GetData(typeof(SkillItem));

                DraggerSource?.Items.Remove(droppedData);
                draggerTarget?.Items.Add(droppedData);
                e.Handled = true;
            }
        }

        private void Button添加_Click(object sender, RoutedEventArgs e)
        {
            Button添加.IsEnabled = false;

            Thread thread = new(() =>
            {
                FileManager.Add();
                Dispatcher.Invoke(() =>
                {
                    文件列表.ItemsSource = FileManager.Names;
                    Button添加.IsEnabled = true;
                });
            });
            thread.Start();
        }

        private void Button删除_Click(object sender, RoutedEventArgs e)
        {
            Button删除.IsEnabled = false;

            Thread thread = new(() =>
            {
                try
                {
                    FileManager.Delete();
                    Dispatcher.Invoke(() =>
                    {
                        文件列表.ItemsSource = FileManager.Names;
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            });
            thread.Start();
        }

        private void 文件列表_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Save技能配置(FileManager.SelectedFile);
            if (FileManager.Select(文件列表.SelectedIndex))
            {
                Button删除.IsEnabled = true;
                配置面板.IsEnabled = true;
                配置面板.Visibility = Visibility.Visible;

                Load技能配置(FileManager.SelectedFile!);
            }
            else
            {
                Button删除.IsEnabled = false;
                配置面板.IsEnabled = false;
                配置面板.Visibility = Visibility.Hidden;
            }
        }

        private void 文件列表_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RenameSkFile(e.OriginalSource);
        }

        private void 右键重命名Click(object sender, RoutedEventArgs e)
        {
            RenameSkFile(文件列表.ItemContainerGenerator.ContainerFromItem(文件列表.SelectedItem));
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SwitchMode(false);
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SwitchMode(false);
                e.Handled = true;
            }
        }
    }
}
