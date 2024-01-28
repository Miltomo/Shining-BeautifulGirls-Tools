using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using static MHTools.数据工具;

namespace Shining_BeautifulGirls
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _jsonEmulator = Path.Combine(App.UserDataDir, "emulator.json");
        EmulatorItem? CurrentItem { get => (EmulatorItem)模拟器列表.SelectedItem; }

#pragma warning disable CA1822 // 禁用提示将成员标记为 static
        AdbHelper ADB => AdbHelper.Instance;
#pragma warning restore CA1822 // 启用提示将成员标记为 static

        int NotFindCount { get; set; } = 0;

        [SaveAll]
        private static class Config
        {
            public static bool AutoConnect { get; set; } = false;
            public static string? LastEmulator { get; set; }
        }

        //TODO 移动到Emulator.cs
        public class EmulatorItem
        {
            public string Name { get; set; }
            public string State { get; set; }
            public string Type { get; set; }
            public int[] Size { get; set; }
        }
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.CanMinimize;
            Title = "闪耀!优俊少女-工具";
            Height = 600;
            Width = 800;
            版本信息.Text = $"{App.Version}  byGithub@Miltomo";
            App.StartWindow = this;

            // ===定义文件路径===
            var path = App.ProgramDir;
            var resources = @$"{path}\resources";
            var cache = @$"{path}\cache";

            World.SymbolDir = @$"{resources}\symbol\";
            World.SkillDir = @$"{resources}\skill\";
            World.CardDir = @$"{resources}\card\";
            World.ScreenshotDir = @$"{path}\screenshot";
            World.CacheDir = cache;
            World.CreateLocInfo();

            ShiningGirl.RecordDir = @$"{path}\养成记录";

            Directory.CreateDirectory(cache);
            Directory.CreateDirectory(World.ScreenshotDir);
            Directory.CreateDirectory(ShiningGirl.RecordDir);

            AdbHelper.SetProgramPath(App.AdbPath);
            AdbHelper.SetWorkDir(World.CacheDir);
            // =======

            提示.Text = "";
            自动连接CheckBox.Visibility = Visibility.Collapsed;
            Load();
        }

        private List<EmulatorItem>? GetEmulators()
        {
            // 结束所有ADB进程
            AdbHelper.KillAll();

            // 寻找能被ADB发现的设备
            var r = ADB.SearchDevices();

            // 寻找其他可能的设备
            var others = Emulator.GetPotentialDevices();

            // 连接用户定义的端口
            var uPts = 端口配置.GetPortsInfo();

            foreach (var item in r)
                ADB.Connect(item[0]);
            foreach (var other in others)
                ADB.Connect(other);
            foreach (var port in uPts)
                ADB.Connect($"127.0.0.1:{port}");

            r = ADB.SearchDevices();
            if (r.Length > 0)
            {
                List<EmulatorItem> list = [];
                for (int i = 0; i < r.Length; i++)
                {
                    var dq = r[i];
                    var name = dq[0];
                    list.Add(new EmulatorItem
                    {
                        Name = name,
                        State = dq[1] switch
                        {
                            "device" => "已连接",
                            _ => "离线"
                        },
                        Size = (ADB.EmulatorName = name, ADB.GetSize()).Item2
                    });
                }
                return list;
            }
            return null;
        }

        private void Save()
        {
            Config.AutoConnect = 自动连接CheckBox.IsChecked ?? false;
            Config.LastEmulator = CurrentItem?.Name;
            SaveAllToSaveAsJSON(typeof(Config), _jsonEmulator);
        }

        private void Load()
        {
            // 隐藏读取数据
            Hide();
            LoadFromJSON(typeof(Config), _jsonEmulator);
            自动连接CheckBox.IsChecked = Config.AutoConnect;
            // 自动连接处理
            if (Config.AutoConnect && Config.LastEmulator != null)
            {
                var emulators = GetEmulators();
                if (emulators != null)
                {
                    模拟器列表.ItemsSource = emulators;
                    模拟器列表.SelectedIndex = ((IEnumerable<EmulatorItem>)模拟器列表.ItemsSource)
                        .ToList()
                        .FindIndex(x => x.Name == Config.LastEmulator);
                    // 直接显示用户界面
                    if (Button确定.IsEnabled)
                    {
                        new 用户界面(CurrentItem!).Show();
                        Close();
                        return;
                    }
                }
            }
            // 显示模拟器选择界面
            Show();
            刷新(Button刷新, new RoutedEventArgs());
        }

        private void 模拟器列表_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Button确定.IsEnabled = false;
            if (模拟器列表.SelectedIndex > -1)
            {
                if (CurrentItem!.State != "离线")
                {
                    if (CurrentItem.Size.Min() == World.STANDARD_WIDTH && CurrentItem.Size.Max() == World.STANDARD_HEIGHT)
                    {
                        提示.Text = $"选择操作设备 {CurrentItem.Name}";
                        Button确定.IsEnabled = true;
                        自动连接CheckBox.Visibility = Visibility.Visible;
                        return;
                    }
                    提示.Text = $"抱歉，目前仅支持分辨率 [{World.STANDARD_WIDTH}x{World.STANDARD_HEIGHT}]";
                }
                else
                    提示.Text = $"无法连接该设备，尝试刷新或重启设备";
            }
            自动连接CheckBox.Visibility = Visibility.Collapsed;
        }

        private void 确定(object sender, RoutedEventArgs e)
        {
            Save();
            提示.Text = "...正在加载界面...";
            IsEnabled = false;
            Thread thread = new(() =>
            {
                Thread.Sleep(100);
                Dispatcher.Invoke(() =>
                {
                    new 用户界面(CurrentItem!).Show();
                    Close();
                });
            });
            thread.Start();
        }

        private void 刷新(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            提示.Text = $"......正在搜索......";
            Thread thread = new(() =>
            {
                var emulators = GetEmulators();
                Dispatcher.Invoke(() =>
                {
                    if (emulators is not null)
                    {
                        NotFindCount = 0;
                        模拟器列表.ItemsSource = emulators;
                        模拟器列表.SelectedIndex = 0;
                    }
                    else
                    {
                        NotFindCount++;
                        模拟器列表.ItemsSource = default;
                        提示.Text = NotFindCount > 2 ?
                        $"找不到设备？试试配置端口" :
                        $"未检测到设备，尝试刷新或重启";
                    }
                    IsEnabled = true;
                });
            });
            thread.Start();
        }

        private void Button输入端口_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            new 端口配置().Show();
        }
    }
}
