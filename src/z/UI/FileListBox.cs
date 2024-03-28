using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MHTools.UI
{
    public class FileListBox
    {
        public ListBox Main { get; init; }
        public SimpleFileManager FileManager { get; init; }
        Dispatcher Dispatcher => Main.Dispatcher;

        TextBlock? textBlockInEdit;
        TextBox? textBoxInEdit;

        //===============================================================
        private void SwitchMode(bool edit = true)
        {
            Dispatcher.Invoke(() =>
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
                            Main.ItemsSource = FileManager.Names;
                        textBlockInEdit = null;
                        textBoxInEdit = null;
                    }
                }
            });
        }

        public FileListBox(ListBox target, SimpleFileManager fileManager)
        {
            FileManager = fileManager;
            Main = target;
            Main.ItemsSource = FileManager.Names;
        }

        public void EnterRename(object? item = null)
        {
            if (FileManager.SelectedFile is null)
                return;

            object source = item ?? Main.ItemContainerGenerator.ContainerFromItem(Main.SelectedItem);

            // 获取目标项
            ListBoxItem? listBoxItem = VET.FindVisualParent<ListBoxItem>(source as DependencyObject);



            if (listBoxItem != null)
            {
                // 获取项中的元素
                textBlockInEdit = VET.FindVisualChild<TextBlock>(listBoxItem);
                textBoxInEdit = VET.FindVisualChild<TextBox>(listBoxItem);

                SwitchMode();
            }
        }

        public void EndRename() => SwitchMode(false);

        /// <summary>
        /// 新建列表项（新建文件）
        /// </summary>
        /// <param name="sender"></param>
        /// <returns>新文件的绝对路径</returns>
        public string Add(FrameworkElement? sender = null)
        {
            if (sender != null)
                sender.IsEnabled = false;
            string fileName = FileManager.Add();
            Dispatcher.Invoke(() =>
            {
                Main.ItemsSource = FileManager.Names;
                if (sender != null)
                    sender.IsEnabled = true;
            });
            return fileName;
        }

        /// <summary>
        /// 删除所选项(代表的文件)
        /// </summary>
        public void Delete()
        {
            if (FileManager.SelectedFile != null)
            {
                FileManager.Delete();
                Dispatcher.Invoke(() =>
                {
                    Main.ItemsSource = FileManager.Names;
                });
            }
        }

        public bool Select(string name) => FileManager.Select(name);
        public void Cancel() => FileManager.CancelSelect();
    }
}
