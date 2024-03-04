using MHTools;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using static MHTools.数据工具;

namespace Shining_BeautifulGirls
{
    /// <summary>
    /// 核心页.xaml 的交互逻辑
    /// </summary>
    public partial class 核心页 : Page
    {
        #region 变量定义
        private readonly string _json养成优俊少女 = Path.Combine(App.UserDataDir, "girl.json");
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
                            //TODO 应该放在这里吗？
                            /*Monitor.UserConfig!.SBGConfig!.GetSkillsTask =
                            技能编辑.GetPrioritySkillListAsync("");*/

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
                                AdbHelper.KillAll();
                                OutPut("⚠️由于长时间未响应，程序已结束⚠️");
                                OutPut("⚠️请检查设备状态或网络连接⚠️");
                            }
                            catch (ResourcesNotFindException)
                            {
                                OutPut("⚠️图片或背景不存在⚠️");
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
                            App.UserWindow!.Topmost = true;
                            App.UserWindow!.Topmost = false;
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
        Queue<Action> PlanQueue { get; set; } = new();
        World Monitor { get; init; }
        class 协助卡Item
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }
        #endregion
        static 用户DataModel VM => 用户DataModel.Get;
        static 配置DataModel Config => 配置DataModel.Get;
        public 核心页(World mnt)
        {
            InitializeComponent();

            Monitor = mnt;
            Monitor.LogEvent += OutPut;
            Monitor.LogUpdateEvent += UpdateLogInfo;
            Monitor.LogDeleteEvent += DeleteLogInfo;
            UpdateSource();

            App.SetImage(设置图标, FileManagerHelper.SetDir(App.SystemIconsDir).Find("设置")!);

            DataContext = VM;

            //=== 绑定数据 ===
            MakeBinding(速度Input, TextBox.TextProperty, nameof(VM.V1));
            MakeBinding(耐力Input, TextBox.TextProperty, nameof(VM.V2));
            MakeBinding(力量Input, TextBox.TextProperty, nameof(VM.V3));
            MakeBinding(毅力Input, TextBox.TextProperty, nameof(VM.V4));
            MakeBinding(智力Input, TextBox.TextProperty, nameof(VM.V5));

            MakeBinding(技能ComboBox, ComboBox.SelectedValueProperty, nameof(VM.技能文件Value));

            MakeBinding(重赛逻辑ComboBox, ComboBox.SelectedIndexProperty, nameof(VM.重赛逻辑Index));
            MakeBinding(选队ComboBox, ComboBox.SelectedIndexProperty, nameof(VM.选队Index));
            MakeBinding(日常赛事ComboBox1, ComboBox.SelectedIndexProperty, nameof(VM.赛事名Index));
            MakeBinding(日常赛事ComboBox2, ComboBox.SelectedIndexProperty, nameof(VM.赛事难度Index));

            MakeBinding(养成循环_体力, RadioButton.IsCheckedProperty, nameof(VM.Is用尽体力));
            MakeBinding(养成次数Input, TextBox.TextProperty, nameof(VM.养成次数设定));
            MakeBinding(使用体力补剂CheckBox, CheckBox.IsCheckedProperty, nameof(VM.Is使用道具));
            MakeBinding(使用宝石CheckBox, CheckBox.IsCheckedProperty, nameof(VM.Is使用宝石));

            MakeBinding(养成启用CheckBox, CheckBox.IsCheckedProperty, nameof(VM.Is需要养成));
            MakeBinding(竞技场启用CheckBox, CheckBox.IsCheckedProperty, nameof(VM.Is需要竞技场));
            MakeBinding(日常赛事启用CheckBox, CheckBox.IsCheckedProperty, nameof(VM.Is需要日常赛事));
            MakeBinding(传奇赛事启用CheckBox, CheckBox.IsCheckedProperty, nameof(VM.Is需要传奇赛事));
            MakeBinding(群英联赛启用CheckBox, CheckBox.IsCheckedProperty, nameof(VM.Is需要群英联赛));
            //============

        }

        private static Binding MakeBinding(string propertyName)
        {
            return new(propertyName)
            {
                Source = VM,
                Mode = BindingMode.TwoWay
            };
        }
        private static void MakeBinding(FrameworkElement element, DependencyProperty dq, string propertyName) => element.SetBinding(dq, MakeBinding(propertyName));

        public void UpdateSource()
        {
            // 更新源集合
            协助卡ComboBox.ItemsSource = Directory.GetFiles(World.CardDir)
                .Select(f => new 协助卡Item { Name = Path.GetFileNameWithoutExtension(f), Path = f });

            技能ComboBox.ItemsSource = new SimpleFileManager(App.SkillStrategyDir).Names;

            // 值还原
            协助卡ComboBox.SelectedIndex = ((IEnumerable<协助卡Item>)协助卡ComboBox.ItemsSource).ToList().FindIndex(x => x.Name.Contains(VM.协助卡名称));
        }

        #region 信息输出
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

        private static void Toast(string msg)
        {
            if (App.UserWindow is 用户界面 wd)
            {
                wd.Toast(msg);
            }
        }

        /// <summary>
        /// 显示一个警告信息对话框
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>true，继续提示; false，不再提示</returns>
        private bool OpenWarningBox(string msg)
        {
            IsEnabled = false;
            new 警告框(msg).ShowDialog();
            IsEnabled = true;
            return 警告框.DoNotDisplay == false;
        }
        #endregion

        #region 数据保存
        public void Save()
        {
            Save养成优俊少女();
            Save用户设置();
        }
        private void Save养成优俊少女()
        {
            if (Monitor.Girl is not null)
                SaveAllToSaveAsJSON(Monitor.Girl, _json养成优俊少女);
        }
        private void Save用户设置()
        {
            // 保存无法绑定的值
            VM.协助卡名称 = ((协助卡Item)协助卡ComboBox.SelectedItem)?.Name ?? "";

            // 保存普通值
            用户DataModel.Save();
        }
        #endregion

        #region 基本交互
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
                App.SetImage(协助卡Image, imagePath);
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
                if (tb.Name == nameof(养成次数Input))
                    tb.Text = "1";
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

        private void Set养成次数(object sender, RoutedEventArgs e)
        {
            bool toSet = (sender as RadioButton)?.IsChecked ?? false;
            养成次数Input.IsEnabled = toSet;
            使用体力补剂CheckBox.Visibility = toSet ? Visibility.Visible : Visibility.Collapsed;
            使用宝石CheckBox.Visibility = toSet ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Button技能编辑_Click(object sender, RoutedEventArgs e)
        {
            Save用户设置();
            App.SkillWindow ??= new 技能编辑();
            App.SkillWindow.Show();
            App.UserWindow!.IsEnabled = false;
        }
        #endregion

        private void Button执行_Click(object sender, RoutedEventArgs e)
        {
            switch (CoreState)
            {
                case "未启动":
                    Save用户设置();

                    // 任务入队
                    PlanQueue.Clear();

                    if (VM.Is需要传奇赛事)
                    {
                        if (Config.Tip传奇赛事)
                            Config.Tip传奇赛事 =
                                OpenWarningBox(
                                    "你选择了「传奇赛事」任务。" +
                                    "请确保此场对决已自行选过角色，否则将使用默认角色。");

                        PlanQueue.Enqueue(Monitor.标准传奇赛事);
                    }
                    if (VM.Is需要竞技场)
                        PlanQueue.Enqueue(Monitor.标准竞技场);
                    if (VM.Is需要日常赛事)
                        PlanQueue.Enqueue(Monitor.标准日常赛事);
                    if (VM.Is需要群英联赛)
                    {
                        if (Config.Tip群英联赛)
                            Config.Tip群英联赛 =
                                OpenWarningBox(
                                    "你选择了「群英联赛」任务。\n" +
                                    "该任务只用于[小组赛]的自动执行，且默认不会使用宝石进行报名。" +
                                    "如需，请至设置界面勾选。");
                        PlanQueue.Enqueue(Monitor.标准群英联赛);
                    }
                    if (VM.Is需要养成)
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
                        DailyRaceNumber = VM.赛事名Index + 1,
                        DRDNumber = VM.赛事难度Index + 1,
                        TeamIndex = VM.选队Index,
                        SupportCard = VM.协助卡名称,
                        CultivateExhaustTP = VM.Is用尽体力,
                        CultivateCount = VM.养成次数设定,
                        CultivateUseProp = VM.Is使用道具,
                        CultivateUseDiamond = VM.Is使用宝石,
                        ExtravaganzaUseDiamond = Config.Is群英允许宝石,
                        SBGConfig = new ShiningGirl.Config
                        {
                            TargetProperty = [VM.V1, VM.V2, VM.V3, VM.V4, VM.V5],
                            ReChallenge = VM.重赛逻辑Index,
                            SaveFactor = Config.保存养成因子,
                            SaveCultivationInfo = Config.保存养成记录,
                            SaveHighLight = Config.保存高光时刻,
                            PrioritySkillList = 技能编辑.GetPrioritySkillList(VM.技能文件Value),
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

        private void 设置图标MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (App.UserWindow is 用户界面 wd)
                wd.切换页面(用户界面.PageEnum.配置页面);
        }
    }
}
