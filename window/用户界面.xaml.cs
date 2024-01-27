using MHTools;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static MHTools.数据工具;

namespace Shining_BeautifulGirls
{
    /// <summary>
    /// 用户界面.xaml 的交互逻辑
    /// </summary>
    public partial class 用户界面 : Window
    {
        private readonly string _json养成优俊少女 = Path.Combine(App.UserDataDir, "girl.json");
        private readonly string _json用户设置 = Path.Combine(App.UserDataDir, "user.json");

        private string _corestate = "未启动";
        public string CoreState
        {
            get => _corestate;
            private set
            {
                switch (value)
                {
                    case "未启动":
                        // UI变动
                        Dispatcher.Invoke(() =>
                        {
                            Button执行.IsEnabled = true;
                            Button执行.Content = "开始执行";

                            LinearGradientBrush brush = new()
                            {
                                StartPoint = new Point(0.5, 0),
                                EndPoint = new Point(0.5, 1)
                            };
                            brush.GradientStops.Add(new GradientStop(Color.FromRgb(101, 188, 243), 0));
                            brush.GradientStops.Add(new GradientStop(Color.FromRgb(245, 245, 223), 1));

                            Button执行.Background = brush;
                        });
                        break;

                    case "准备开始":
                        // UI变动
                        Dispatcher.Invoke(() =>
                        {
                            Button执行.IsEnabled = false;
                        });
                        Thread threadUI = new(() =>
                        {
                            Animation面板移动();
                        });

                        // 核程序变动
                        Thread thread核心程序 = new(() =>
                        {
                            Monitor.Start();
                            try
                            {
                                if (Monitor.位置检测())
                                    while (PlanQueue.Count > 0)
                                        PlanQueue.Dequeue()();
                                else
                                    OutPut("⚠️请先回到游戏主界面⚠️");
                            }
                            catch (StopException)
                            {
                                Debug.WriteLine("程序终止");
                            }
                            catch (LongTimeNoOperationException)
                            {
                                OutPut("⚠️由于长时间未响应，程序已结束⚠️");
                                OutPut("请检查网络或ADB连接");
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                            Monitor.Stop();
                            Save养成优俊少女();

#pragma warning disable CA2011 // 禁用无限递归警告
                            if (CoreState == "终止")
                                CoreState = "重置";
                            else
                                CoreState = "已结束";
#pragma warning restore CA2011 // 恢复无限递归警告
                        });
                        //===========================================
                        threadUI.Start();
                        thread核心程序.Start();
                        break;

                    case "运行中":
                        // UI变动
                        Dispatcher.Invoke(() =>
                        {
                            Button执行.IsEnabled = true;
                            Button执行.Content = "停止";

                            LinearGradientBrush brush = new()
                            {
                                StartPoint = new Point(0.5, 0),
                                EndPoint = new Point(0.5, 1)
                            };
                            brush.GradientStops.Add(new GradientStop(Color.FromRgb(237, 59, 59), 0));
                            brush.GradientStops.Add(new GradientStop(Color.FromRgb(245, 245, 223), 1));

                            Button执行.Background = brush;
                        });
                        break;

                    // 用户主动停止
                    case "终止":
                        // UI变动
                        Dispatcher.Invoke(() =>
                        {
                            Button执行.IsEnabled = false;
                        });
                        Monitor.Stop();
                        break;

                    case "已结束":
                        // UI变动
                        Dispatcher.Invoke(() =>
                        {
                            Topmost = true;
                            Topmost = false;
                            LinearGradientBrush brush = new()
                            {
                                StartPoint = new Point(0.5, 0),
                                EndPoint = new Point(0.5, 1)
                            };
                            brush.GradientStops.Add(new GradientStop(Color.FromRgb(232, 192, 84), 0));
                            brush.GradientStops.Add(new GradientStop(Color.FromRgb(245, 245, 223), 1));

                            Button执行.IsEnabled = true;
                            Button执行.Content = "返回";
                            Button执行.Background = brush;
                        });
                        break;

                    // 回到初始状态
                    case "重置":
                        // UI变动
                        Dispatcher.Invoke(() =>
                        {
                            Button执行.IsEnabled = false;
                            运行记录.Blocks.Clear();
                        });
                        Thread thread = new(() =>
                        {
                            Animation面板移动(true);
                        });
                        thread.Start();
                        break;

                    default:
                        value = "未知状态";
                        break;
                }
                _corestate = value;
            }
        }

        readonly BlockingCollection<Action> TipQueue = [];
        Queue<Action> PlanQueue { get; set; } = new();
        World Monitor { get; init; }
        class 协助卡Item
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }

        [SaveAll]
        private static class User
        {
            public static int[] 目标属性值 { get; set; } = [1100, 1000, 700, 360, 400];
            public static string 协助卡名称 { get; set; } = "北部玄驹";
            public static string 技能文件名 { get; set; } = "";
            public static int 重赛逻辑 { get; set; } = 0;
            public static int 选队Index { get; set; } = 0;
            public static int 赛事名Index { get; set; } = 0;
            public static int 赛事难度Index { get; set; } = 0;

            public static bool 需要养成 { get; set; } = true;
            public static int 养成次数设定 { get; set; } = -1;
            public static bool 使用道具 { get; set; } = true;
            public static bool 使用宝石 { get; set; } = false;

            public static bool 需要竞技场 { get; set; } = true;
            public static bool 需要日常赛事 { get; set; } = true;
        }

        public 用户界面(MainWindow.EmulatorItem emulator)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.CanMinimize;
            Title = $"操作设备：（{emulator.Name}）";
            Height = 800;
            Width = 600;
            App.UserWindow = this;

            // 启动处理消息队列的后台线程
            Task.Run(() =>
            {
                foreach (var task in TipQueue.GetConsumingEnumerable())
                    task();
            });

            Monitor = new(
                new AdbHelper(App.AdbPath, World.CacheDir)
                .Connect(emulator.Name)
                );
            Monitor.LogEvent += OutPut;
            Monitor.LogUpdateEvent += UpdateLogInfo;
            Monitor.LogDeleteEvent += DeleteLogInfo;

            Refresh();
#if DEBUG
            右键测试.Visibility = Visibility.Visible;
#endif
            //TODO 使用异步更新技能名列表
        }

        public void Refresh()
        {
            协助卡ComboBox.ItemsSource = Directory.GetFiles(World.CardDir)
                .Select(f => new 协助卡Item { Name = Path.GetFileNameWithoutExtension(f), Path = f });

            技能ComboBox.ItemsSource = new SimpleFileManager(App.SkillStrategyDir).Names;

            Load用户设置(_json用户设置);
        }

        private void OutPut(object logInfo)
        {
            var text = logInfo.ToString();
            Dispatcher.Invoke(() =>
            {
                var p = new Paragraph
                {
                    Margin = new Thickness(0),
                    Padding = new Thickness(0)
                };
                p.Inlines.Add(text);
                运行记录.Blocks.Add(p);
                App.GetScrollViewer(滚动包装器)?.ScrollToEnd();
            });
        }

        private void UpdateLogInfo(object logInfo)
        {
            // 更新最后一个 Paragraph 的内容
            Dispatcher.Invoke(() =>
            {
                if (运行记录.Blocks.Count > 0 && 运行记录.Blocks.LastBlock is Paragraph p)
                {
                    // 清空原有的内容
                    p.Inlines.Clear();

                    // 添加新的内容
                    Run newRun = new(logInfo.ToString());
                    p.Inlines.Add(newRun);
                }
            });
        }

        private void DeleteLogInfo(int count = 1)
        {
            Dispatcher.Invoke(() =>
            {
                var B = 运行记录.Blocks;

                for (int i = 0; i < count; i++)
                {
                    // 删除最后一个LogInfo
                    if (B.Count > 0)
                        B.Remove(B.LastBlock);
                    else
                        break;
                }
            });
        }

        private void Toast(string msg)
        {
            TipQueue.Add(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    提示信息.Text = msg;
                    提示信息.Opacity = 1;
                });
                Thread.Sleep(1000);

                int sum = 500, step = 20;
                int count = sum / step;
                var bias = 1d / count;
                for (int i = 0; i < count; i++)
                {
                    Dispatcher.Invoke(() => 提示信息.Opacity -= bias);
                    Thread.Sleep(step);
                }

                Dispatcher.Invoke(() => 提示信息.Opacity = 0);
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            Monitor.Stop();
            Save养成优俊少女();
            Save用户设置();
            base.OnClosed(e);
        }

        public void Save养成优俊少女()
        {
            if (Monitor.Girl is not null)
                SaveAllToSaveAsJSON(Monitor.Girl, _json养成优俊少女);
        }

        public void Save用户设置()
        {
            User.目标属性值 = [
                int.Parse(速度Input.Text),
                int.Parse(耐力Input.Text),
                int.Parse(力量Input.Text),
                int.Parse(毅力Input.Text),
                int.Parse(智力Input.Text),
            ];
            User.协助卡名称 = ((协助卡Item)协助卡ComboBox.SelectedItem)?.Name ?? "";
            User.技能文件名 = 技能ComboBox.SelectedItem as string ?? "";
            User.重赛逻辑 = 重赛逻辑ComboBox.SelectedIndex;
            User.选队Index = 选队ComboBox.SelectedIndex;
            User.赛事名Index = 日常赛事ComboBox1.SelectedIndex;
            User.赛事难度Index = 日常赛事ComboBox2.SelectedIndex;


            User.需要养成 = 养成启用CheckBox.IsChecked ?? false;
            User.养成次数设定 = (养成循环_次数.IsChecked ?? false) ?
                int.Parse(养成次数Input.Text) : -1;
            User.使用道具 = 使用体力补剂CheckBox.IsChecked ?? false;
            User.使用宝石 = 使用宝石CheckBox.IsChecked ?? false;

            User.需要竞技场 = 竞技场启用CheckBox.IsChecked ?? false;
            User.需要日常赛事 = 日常赛事启用CheckBox.IsChecked ?? false;

            SaveAllToSaveAsJSON(typeof(User), _json用户设置);
        }
        private void Load用户设置(string file)
        {
            LoadFromJSON(typeof(User), file);
            速度Input.Text = User.目标属性值[0].ToString();
            耐力Input.Text = User.目标属性值[1].ToString();
            力量Input.Text = User.目标属性值[2].ToString();
            毅力Input.Text = User.目标属性值[3].ToString();
            智力Input.Text = User.目标属性值[4].ToString();

            协助卡ComboBox.SelectedIndex = ((IEnumerable<协助卡Item>)协助卡ComboBox.ItemsSource).ToList().FindIndex(x => x.Name.Contains(User.协助卡名称));
            重赛逻辑ComboBox.SelectedIndex = User.重赛逻辑;
            技能ComboBox.SelectedIndex = ((IEnumerable<string>)技能ComboBox.ItemsSource).ToList().FindIndex(x => x == User.技能文件名);
            选队ComboBox.SelectedIndex = User.选队Index;
            日常赛事ComboBox1.SelectedIndex = User.赛事名Index;
            日常赛事ComboBox2.SelectedIndex = User.赛事难度Index;

            养成启用CheckBox.IsChecked = User.需要养成;
            if (User.养成次数设定 > 0)
            {
                养成循环_次数.IsChecked = true;
                养成次数Input.Text = User.养成次数设定.ToString();
                使用体力补剂CheckBox.IsChecked = User.使用道具;
                使用宝石CheckBox.IsChecked = User.使用宝石;
            }

            竞技场启用CheckBox.IsChecked = User.需要竞技场;
            日常赛事启用CheckBox.IsChecked = User.需要日常赛事;
        }

        private void Animation面板移动(bool reverse = false)
        {
            const double W = 600;
            const int step = 5;
            var max = (int)Math.Ceiling(300d / step);
            var dO = 1d / max;
            var dX = W / max;

            for (int i = 0; i < max; i++)
            {
                Dispatcher.Invoke(() =>
                {
                    选项面板.Opacity -= reverse ? -dO : dO;
                    选项面板.SetValue(Canvas.LeftProperty, reverse ? -W + dX * i : -dX * i);

                    输出面板.Opacity += reverse ? -dO : dO;
                    输出面板.SetValue(Canvas.LeftProperty, reverse ? dX * i : W - dX * i);
                });
                Thread.Sleep(step);
            }

            Dispatcher.Invoke(() =>
            {
                选项面板.Opacity = reverse ? 1 : 0;
                选项面板.SetValue(Canvas.LeftProperty, reverse ? 0d : -W);
                输出面板.Opacity = reverse ? 0 : 1;
                输出面板.SetValue(Canvas.LeftProperty, reverse ? W : 0d);
            });

            CoreState = reverse ? "未启动" : "运行中";
        }


        private void 协助卡ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (协助卡ComboBox.SelectedIndex > -1)
            {
                var imagePath = ((协助卡Item)协助卡ComboBox.SelectedItem).Path;
                // 创建BitmapImage对象
                BitmapImage bitmap = new();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
                bitmap.EndInit();

                // 将BitmapImage对象分配给Image控件的Source属性
                协助卡Image.Source = bitmap;
            }
        }

        private void NumberInput_KeyDown(object sender, KeyEventArgs e)
        {
            string name = (sender as TextBox).Name;
            int length = (sender as TextBox).Text.Length;
            int limitLength = name switch
            {
                "养成次数Input" => 2,
                _ => 4
            };

            if (e.Key != Key.Back)
            {
                if (UI.IsKeyNumeric(e.Key) && length != limitLength)
                    return;
                e.Handled = true;
            }
        }

        private void NumberInput_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            int length = tb.Text.Length;
            if (length == 0)
            {
                tb.Text = "0";
            }
            else
            {
                var text = tb.Text;
                while (text.Length > 1)
                {
                    if (text[0] == '0')
                        text = text[1..];
                    else
                        break;
                }
                tb.Text = text;
            }
        }

        private void Button执行_Click(object sender, RoutedEventArgs e)
        {
            switch (CoreState)
            {
                case "未启动":
                    Save用户设置();

                    // 任务入队
                    PlanQueue.Clear();
                    if (User.需要竞技场)
                        PlanQueue.Enqueue(Monitor.标准竞技场);
                    if (User.需要日常赛事)
                        PlanQueue.Enqueue(Monitor.标准日常赛事);
                    if (User.需要养成)
                        PlanQueue.Enqueue(Monitor.自定义养成);

                    if (PlanQueue.Count == 0)
                    {
                        Toast("!未选择任何任务");
                        return;
                    }

                    // 还原养成优俊少女
                    ShiningGirl girl = new(Monitor);
                    LoadFromJSON(girl, _json养成优俊少女);
                    Monitor.Girl = girl;

                    // 载入用户配置
                    Monitor.UserConfig = new World.Config
                    {
                        DailyRaceNumber = User.赛事名Index + 1,
                        DRDNumber = User.赛事难度Index + 1,
                        TeamNumber = User.选队Index + 1,
                        SupportCard = User.协助卡名称,
                        CultivateCount = User.养成次数设定,
                        CultivateUseProp = User.使用道具,
                        CultivateUseMoney = User.使用宝石,
                        SBGConfig = new ShiningGirl.Config
                        {
                            TargetProperty = User.目标属性值,
                            ReChallenge = User.重赛逻辑,
                            PrioritySkillList =
                            string.IsNullOrWhiteSpace(User.技能文件名) ?
                            null :
                            技能编辑.GetPrioritySkillList(User.技能文件名)
                        }
                    };

                    CoreState = "准备开始";
                    break;

                case "运行中":
                    CoreState = "终止";
                    break;

                case "已结束":
                    CoreState = "重置";
                    break;
                default:
                    break;
            }
        }

        private void Button技能编辑_Click(object sender, RoutedEventArgs e)
        {
            Save用户设置();
            App.SkillWindow ??= new 技能编辑();
            App.SkillWindow.Show();
            IsEnabled = false;
        }

        private void Set养成次数(object sender, RoutedEventArgs e)
        {
            bool toSet = (sender as RadioButton)?.IsChecked ?? false;
            养成次数Input.IsEnabled = toSet;
            使用体力补剂CheckBox.Visibility = toSet ? Visibility.Visible : Visibility.Collapsed;
            使用宝石CheckBox.Visibility = toSet ? Visibility.Visible : Visibility.Collapsed;
        }

        private void 右键截图_Click(object sender, RoutedEventArgs e)
        {
            Monitor.SaveScreen();
            Toast($"已保存截图：前往 {World.ScreenshotDir} 查看");
        }

        private void 右键测试_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new(() =>
            {
                var files = Directory.GetFiles(@"Z:\C#练习\Shining BeautifulGirls\resources\skill");
                List<string[]> list = [];

                // 创建 Stopwatch 实例
                Stopwatch stopwatch = new();

                // 启动计时器
                stopwatch.Start();

                // 调用需要计时的函数

                var v = Emulator.CheckConnection(port: 16384);

                Trace.WriteLine(v);

                /*var dir = @"C:\Users\Administrator\Desktop\mlData\data\Rank";
                Monitor.Refresh();
                Func<object, OpenCvSharp.Mat> func = Monitor.CropScreen;
                func(World.NP.Zone.Rank草地).SaveImage(Path.Combine(dir, $"{TimeTool.RandomLetters(10)}.png"));
                func(World.NP.Zone.Rank泥地).SaveImage(Path.Combine(dir, $"{TimeTool.RandomLetters(10)}.png"));
                func(World.NP.Zone.Rank短距离).SaveImage(Path.Combine(dir, $"{TimeTool.RandomLetters(10)}.png"));
                func(World.NP.Zone.Rank英里).SaveImage(Path.Combine(dir, $"{TimeTool.RandomLetters(10)}.png"));
                func(World.NP.Zone.Rank中距离).SaveImage(Path.Combine(dir, $"{TimeTool.RandomLetters(10)}.png"));
                func(World.NP.Zone.Rank长距离).SaveImage(Path.Combine(dir, $"{TimeTool.RandomLetters(10)}.png"));
                func(World.NP.Zone.Rank领跑).SaveImage(Path.Combine(dir, $"{TimeTool.RandomLetters(10)}.png"));
                func(World.NP.Zone.Rank跟前).SaveImage(Path.Combine(dir, $"{TimeTool.RandomLetters(10)}.png"));
                func(World.NP.Zone.Rank居中).SaveImage(Path.Combine(dir, $"{TimeTool.RandomLetters(10)}.png"));
                func(World.NP.Zone.Rank后追).SaveImage(Path.Combine(dir, $"{TimeTool.RandomLetters(10)}.png"));*/


                // 停止计时器
                stopwatch.Stop();

                // 获取经过的时间（毫秒）
                long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                var one = (double)elapsedMilliseconds / files.Length;

                Debug.WriteLine($"Function took {elapsedMilliseconds} milliseconds to execute.");
                //Debug.WriteLine($"一个图片平均 {one} 毫秒\n一个训练6次判断，需要用 {one * 6} 毫秒");


                //new ShiningGirl(Monitor).测试();
                ;
            });
            thread.Start();
        }
    }
}
