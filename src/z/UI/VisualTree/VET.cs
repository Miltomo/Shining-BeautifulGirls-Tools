using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MHTools.UI
{
    public static class VET
    {
        public static ScrollViewer? GetScrollViewer(DependencyObject element)
        {
            if (element is ScrollViewer scrollViewer)
            {
                return scrollViewer;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);

                var result = GetScrollViewer(child);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public static T? FindVisualParent<T>(DependencyObject? child) where T : DependencyObject
        {
            while (child != null)
            {
                if (child is T parent)
                {
                    return parent;
                }
                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }

        public static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is not null)
                {
                    if (child is T target)
                        return target;
                    else
                    {
                        var childOfChild = FindVisualChild<T>(child);
                        if (childOfChild != null)
                            return childOfChild;
                    }
                }
            }
            return null;
        }

        public static void SetImage(Image image, string? picturePath)
        {
            if (string.IsNullOrEmpty(picturePath))
                return;

            // 创建BitmapImage对象
            BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(picturePath, UriKind.RelativeOrAbsolute);
            bitmap.EndInit();

            // 将BitmapImage对象分配给Image控件的Source属性
            image.Source = bitmap;
        }

        // 递归函数，检查指定控件及其子控件是否包含指定文本的 TextBlock
        public static bool CheckForTextBlock(DependencyObject parent, string searchText)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child != null)
                {
                    if (child is TextBlock textBlock)
                    {
                        if (textBlock.Text == searchText)
                        {
                            // 找到了指定文本的 TextBlock，返回 true
                            return true;
                        }
                    }

                    // 递归调用，继续检查子控件
                    if (CheckForTextBlock(child, searchText))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // 获取指定控件及其子控件中所有 TextBlock 的文本
        public static string[] GetAllTextBlockTexts(DependencyObject parent)
        {
            List<string> texts = [];
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child != null)
                {
                    if (child is TextBlock textBlock)
                    {
                        texts.Add(textBlock.Text);
                    }

                    // 递归调用，继续检查子控件
                    string[] childTexts = GetAllTextBlockTexts(child);
                    texts.AddRange(childTexts);
                }
            }
            return [.. texts];
        }

        public static List<T> Items2List<T>(ItemCollection itemCollection)
        {
            List<T> itemList = [];

            foreach (var item in itemCollection)
            {
                if (item is T convertedItem)
                {
                    itemList.Add(convertedItem);
                }
            }

            return itemList;
        }
    }
}
