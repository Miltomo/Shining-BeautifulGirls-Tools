using ComputerVision;
using MHTools;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

        CollectionView 仓库分类器 { get; init; }

        TextBlock? textBlockInEdit;
        TextBox? textBoxInEdit;

        class SkillItem
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public string[] Tag { get; set; }

            public override bool Equals(object? obj)
            {
                if (obj == null || GetType() != obj.GetType())
                    return false;

                var other = (SkillItem)obj;
                return Path == other.Path;
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode() ^ Path.GetHashCode();
            }
        }

        public static string[][] GetPrioritySkillList(string name)
        {
            List<string[]> skList = [];
            var wd = new 技能编辑();
            if (wd.FileManager.Select(name))
            {
                wd.Load技能配置(wd.FileManager.SelectedFile!);
                wd.技能组.ForEach(lb =>
                skList.Add(
                    App.Items2List<SkillItem>(lb.Items)
                    .Select(item => item.Name)
                    .ToArray()
                )
                );
                wd.FileManager.CancelSelect();
            }
            wd.Close();

            return [.. skList];
        }

        public static async Task<string[][]> GetPrioritySkillListAsync(string name)
        {
            List<string[]> pathList = [];
            var wd = new 技能编辑();
            //TODO 用OCR识别各文件
            if (wd.FileManager.Select(name))
            {
                wd.Load技能配置(wd.FileManager.SelectedFile!);
                wd.技能组.ForEach(lb =>
                pathList.Add(
                    App.Items2List<SkillItem>(lb.Items)
                    .Select(item => item.Path)
                    .ToArray()
                )
                );
                wd.FileManager.CancelSelect();
            }
            wd.Close();

            var skList = await Task.Run(() =>
            {
                return pathList
                .Select(ps =>
                ps.Select(
                    x =>
                    {
                        var r = PaddleOCR.SetImage(x).Extract().TextAsLines.First();
                        Trace.WriteLine(r);
                        Thread.Sleep(100);
                        return r;
                    }).ToArray());
            });

            return [.. skList];
        }
        public 技能编辑()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.CanResizeWithGrip;
            Height = 800;
            Width = 1200;
            技能组 = [级别1, 级别2, 级别3, 级别4];
            右侧面板.Visibility = Visibility.Collapsed;

            Array.ForEach(GenerateItems(World.SkillDir), DefaultList.Add);

            仓库分类器 = (CollectionView)CollectionViewSource.GetDefaultView(技能仓库.Items);
            仓库分类器.Filter = (item) =>
            {
                if (item is SkillItem skillItem)
                {
                    var sx = App.GetAllTextBlockTexts(筛选项集合);
                    return sx.Length == 0 || skillItem.Tag.Intersect(sx).Any();
                }
                return false;
            };

            筛选下拉列表.ItemsSource = DefaultList.Select(x => x.Tag).SelectMany(x => x).Distinct();

            文件列表.ItemsSource = FileManager.Names;
        }
        protected override void OnClosed(EventArgs e)
        {
            Save技能配置(FileManager.SelectedFile);
            用户界面.Refresh();

            if (App.UserWindow is not null)
            {
                App.UserWindow.IsEnabled = true;
                App.UserWindow.Show();
            }
            App.SkillWindow = null;
            base.OnClosed(e);
        }

        private static SkillItem[] GenerateItems(string dir)
        {
            List<SkillItem> r = [];
            Queue<string> subs = [];
            subs.Enqueue(dir);
            while (subs.Count > 0)
            {
                var dq = subs.Dequeue();
                var tag = dq.Replace(dir, string.Empty).Split("\\", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                Directory.GetFiles(dq).ToList().ForEach(
                    f => r.Add(new SkillItem
                    {
                        Name = Path.GetFileNameWithoutExtension(f),
                        Path = f,
                        Tag = tag,
                    }));
                Array.ForEach(Directory.GetDirectories(dq), subs.Enqueue);
            }
            return [.. r];
        }


        private void 刷新技能仓库()
        {
            技能仓库.Items.Clear();

            // 删去已经分配的技能
            var complement = DefaultList.Except(
                    技能组
                    .Select(x => App.Items2List<SkillItem>(x.Items))
                    .SelectMany(list => list)
                    .Distinct()
                    ).ToList();

            complement.ForEach(item => 技能仓库.Items.Add(item));

            仓库分类器.Refresh();
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
            技能组.ForEach(x => x.Items.Clear());

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
            }

            刷新技能仓库();
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
                右侧面板.Visibility = Visibility.Visible;
                /*配置面板.IsEnabled = true;
                配置面板.Visibility = Visibility.Visible;*/

                Load技能配置(FileManager.SelectedFile!);
            }
            else
            {
                Button删除.IsEnabled = false;
                右侧面板.Visibility = Visibility.Collapsed;
                /*配置面板.IsEnabled = false;
                配置面板.Visibility = Visibility.Hidden;*/
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


        private void 筛选按钮Click(object sender, RoutedEventArgs e)
        {
            筛选下拉列表.SelectedIndex = -1;

            popup.IsOpen = !popup.IsOpen; // 切换下拉列表的展开状态
        }

        private void 筛选下拉列表Selected(object sender, SelectionChangedEventArgs e)
        {
            if (筛选下拉列表.SelectedIndex > -1)
            {
                popup.IsOpen = false;

                var target = (string)筛选下拉列表.SelectedValue;

                if (App.CheckForTextBlock(筛选项集合, target))
                    return;

                筛选项集合.Children.Add(new TextBlock()
                {
                    Text = (string)筛选下拉列表.SelectedValue,
                    Style = (Style)Resources["筛选TextBlockStyle"]
                });

                刷新技能仓库();
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer && e.Delta != 0)
            {
                double newOffset = scrollViewer.HorizontalOffset - (e.Delta > 0 ? 20 : -20);
                if (newOffset < 0)
                {
                    scrollViewer.ScrollToHorizontalOffset(0);
                }
                else if (newOffset > scrollViewer.ScrollableWidth)
                {
                    scrollViewer.ScrollToHorizontalOffset(scrollViewer.ScrollableWidth);
                }
                else
                {
                    scrollViewer.ScrollToHorizontalOffset(newOffset);
                }

                e.Handled = true;
            }
        }

        private void 清除筛选(object sender, RoutedEventArgs e)
        {
            筛选项集合.Children.Clear();

            刷新技能仓库();
        }
    }
}
