using MHTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
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

        private int _state = 0;
        public int RunningState
        {
            get => _state;
            private set
            {
                switch (value)
                {
                    //未开始
                    case 0:
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
                    //执行中
                    case 1:
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
                    //成功结束
                    case 2:
                        Dispatcher.Invoke(() =>
                        {
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
                }
                _state = value;
            }
        }

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
            App.用户界面 = this;

            Monitor = new(
                new AdbHelper(App.AdbPath, World.CacheDir)
                .Connect(emulator.Name),
                OutPut
                );

            Refresh();
            //TODO Monitor位置检测
        }

        public void Refresh()
        {
            协助卡ComboBox.ItemsSource = Directory.GetFiles(World.CardDir)
                .Select(f => new 协助卡Item { Name = Path.GetFileNameWithoutExtension(f), Path = f });

            技能ComboBox.ItemsSource = new SimpleFileManager(App.SkillStrategyDir).Names;

            Load用户设置(_json用户设置);
        }

        void OutPut(object logInfo)
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
        protected override void OnClosed(EventArgs e)
        {
            Monitor.Stop();
            Save养成优俊少女();
            Save用户设置();
            base.OnClosed(e);
        }

        private void Save养成优俊少女()
        {
            if (Monitor.Girl is not null)
                SaveAllToSaveAsJSON(Monitor.Girl, _json养成优俊少女);
        }
        private void Load养成优俊少女()
        {
            ShiningGirl girl = new(Monitor);
            LoadFromJSON(girl, _json养成优俊少女);
            Monitor.Girl = girl;
        }
        private void Save用户设置()
        {
            User.目标属性值 = [
                int.Parse(速度Input.Text),
                int.Parse(耐力Input.Text),
                int.Parse(力量Input.Text),
                int.Parse(毅力Input.Text),
                int.Parse(智力Input.Text),
            ];
            User.协助卡名称 = ((协助卡Item)协助卡ComboBox.SelectedItem).Name;
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


        /// <summary>
        /// 程序运行的中心控制处
        /// </summary>
        /// <param name="stopIt"></param>
        private void 运行程序(bool stopIt = false)
        {
            Button执行.IsEnabled = false;
            //终止程序
            if (stopIt)
            {
                Monitor.Stop();
                Thread thread = new(() =>
                {
                    while (true)
                    {
                        // 如果程序线程已退出
                        if (RunningState > 1)
                        {
                            Animation面板移动(true);
                            break;
                        }
                    }
                });
                thread.Start();
                return;
            }
            运行记录.Blocks.Clear();
            //==============
            //★操作UI的线程★
            //==============
            Thread threadUI = new(() =>
            {
                Animation面板移动();
            });
            //=============
            //★主程序线程★
            //=============
            Thread thread核心程序 = new(() =>
            {
                try
                {
                    Monitor.Start();

                    while (PlanQueue.Count > 0)
                        PlanQueue.Dequeue()();

                    Monitor.Stop();
                    //线程成功结束
                    RunningState = 2;
                    return;
                }
                catch (UserStopException)
                {
                    Debug.WriteLine("用户终止");
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
                Save养成优俊少女();
                //线程异常结束
                RunningState = 5;
            });
            //===========================================
            threadUI.Start();
            thread核心程序.Start();
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

            RunningState = reverse ? 0 : 1;
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
            switch (RunningState)
            {
                //未开始
                case 0:
                    Save用户设置();

                    PlanQueue.Clear();
                    if (User.需要竞技场)
                        PlanQueue.Enqueue(Monitor.标准竞技场);
                    if (User.需要日常赛事)
                        PlanQueue.Enqueue(Monitor.标准日常赛事);
                    if (User.需要养成)
                        PlanQueue.Enqueue(Monitor.自定义养成);

                    if (PlanQueue.Count == 0)
                    {
                        return;
                    }

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

                    Load养成优俊少女();

                    运行程序();
                    break;
                default:
                    运行程序(true);
                    break;
            }
        }

        private void 启用CheckBox_CheckChanged(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            switch (cb.Name)
            {
                case "养成启用CheckBox":
                    /*养成任务.IsEnabled = cb.IsChecked ?? false;
                    cb.IsEnabled = true;*/
                    break;
            }
        }

        private void Button技能编辑_Click(object sender, RoutedEventArgs e)
        {
            Save用户设置();
            App.技能编辑界面 ??= new 技能编辑();
            App.技能编辑界面.Show();
            IsEnabled = false;
        }

        private void Set养成次数(object sender, RoutedEventArgs e)
        {
            bool toSet = (sender as RadioButton)?.IsChecked ?? false;
            养成次数Input.IsEnabled = toSet;
            使用体力补剂CheckBox.Visibility = toSet ? Visibility.Visible : Visibility.Collapsed;
            使用宝石CheckBox.Visibility = toSet ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
