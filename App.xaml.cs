global using System;
global using System.Collections.Generic;
global using System.Linq;
global using static Shining_BeautifulGirls.World.NP;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace Shining_BeautifulGirls
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Window? StartWindow { get; set; }
        public static Window? UserWindow { get; set; }
        public static Window? SkillWindow { get; set; }

        public static readonly string Version = "v0.10.1-repair2";

        public static string AdbPath => Path.Combine(ProgramDir, @"adb/adb.exe");
        public static string ProgramDir { get; private set; } = Environment.CurrentDirectory;
        public static string UserDataDir
        {
            get
            {
                var dir = Path.Combine(ProgramDir, "userdata");
                Directory.CreateDirectory(dir);
                return dir;
            }
        }
        public static string SkillStrategyDir
        {
            get
            {
                var dir = Path.Combine(UserDataDir, "sks");
                Directory.CreateDirectory(dir);
                return dir;
            }
        }



        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 订阅未处理异常事件
            // 非UI线程上的异常
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                // 保存数据
                if (UserWindow is 用户界面 wd)
                {
                    wd.Save养成优俊少女();
                    wd.Save用户设置();
                }

                Exception ex = e.ExceptionObject as Exception;
                MessageBox.Show($"出现了一个未经处理的异常: {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            };

            // UI线程上的异常
            Current.DispatcherUnhandledException += (s, e) =>
            {
                // 防止应用程序终止
                e.Handled = true;

                // 显示错误对话框或者记录日志
                MessageBox.Show($"An unhandled exception occurred: {e.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                Shutdown();
            };
        }

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
