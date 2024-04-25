﻿using ComputerVision;
using MHTools;
using MHTools.UI;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

//TODO 把核心Thread移到窗口去
namespace Shining_BeautifulGirls
{
    /// <summary>
    /// 核心页.xaml 的交互逻辑
    /// </summary>
    public partial class 核心页 : Page
    {
        #region 变量定义
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
                            // 异步获取所有要学习技能
                            var cfg = Monitor.UserConfig!.SBGConfig!;
                            if (cfg.NeedNewTask || cfg.SkillGetingTask == null)
                                cfg.SkillGetingTask = Task.Run(async () =>
                                {
#if DEBUG
                                    Toast("开始技能获取任务");
#endif
                                    var paths = cfg.SkillPaths;

                                    List<string[]> zz = [];
                                    foreach (var item in paths)
                                    {
                                        List<string> lin = [];
                                        for (int i = 0; i < item.Length; i++)
                                        {
                                            var ext = await PaddleOCR.SetImage(item[i]).ExtractAsync();
                                            lin.Add(ext.TextAsLines.First());
                                            await Task.Delay(200);
                                        }

                                        zz.Add([.. lin]);
                                    }

                                    string[][] r = [.. zz];
#if DEBUG
                                    Toast("已成功获取全部技能");
#endif
                                    return r;
                                });
                            else
                            {
#if DEBUG
                                Toast($"使用原来任务：{cfg.SkillGetingTask.Status}\n");
#endif
                            }

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
                                OutPut("⚠️游戏可能已闪退⚠️");
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                            Monitor.Stop();
                            Monitor.Girl?.Save();

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
                            App.UserWindow.顶层弹出显示();
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
        FileListBox 预设Manager { get; init; }
        static ShiningGirl.AlgorithmItem[] 算法集合 => ShiningGirl.Algorithms;
        class 协助卡Item
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }
        #endregion
        static 用户DataModel VM => 用户DataModel.Instance;
        static 用户DataModel.养成任务Info YC => VM.Cultivation;
        static 配置DataModel Config => 配置DataModel.Get;
        public 核心页(World mnt)
        {
            InitializeComponent();

            Monitor = mnt;
            Monitor.LogEvent += OutPut;
            Monitor.LogUpdateEvent += UpdateLogInfo;
            Monitor.LogDeleteEvent += DeleteLogInfo;
            DataContext = VM;

            预设Manager = new(预设下拉列表, new SimpleFileManager(Path.Combine(App.UserDataDir, "clt"), "预设", "json"));

            UpdateSource();
            //=== 绑定数据 ===
            MakeBinding(预设下拉列表, ListBox.SelectedValueProperty, nameof(VM.养成预设Value));

            MakeBinding(速度Input, TextBox.TextProperty, 用户DataModel.GetCultivationPath(nameof(YC.V1)));
            MakeBinding(耐力Input, TextBox.TextProperty, 用户DataModel.GetCultivationPath(nameof(YC.V2)));
            MakeBinding(力量Input, TextBox.TextProperty, 用户DataModel.GetCultivationPath(nameof(YC.V3)));
            MakeBinding(毅力Input, TextBox.TextProperty, 用户DataModel.GetCultivationPath(nameof(YC.V4)));
            MakeBinding(智力Input, TextBox.TextProperty, 用户DataModel.GetCultivationPath(nameof(YC.V5)));

            MakeBinding(技能ComboBox, ComboBox.SelectedValueProperty, 用户DataModel.GetCultivationPath(nameof(YC.技能文件Value)));

            MakeBinding(重赛逻辑ComboBox, ComboBox.SelectedIndexProperty, 用户DataModel.GetCultivationPath(nameof(YC.重赛逻辑Index)));
            MakeBinding(选队ComboBox, ComboBox.SelectedIndexProperty, nameof(VM.选队Index));
            MakeBinding(日常赛事ComboBox1, ComboBox.SelectedIndexProperty, nameof(VM.赛事名Index));
            MakeBinding(日常赛事ComboBox2, ComboBox.SelectedIndexProperty, nameof(VM.赛事难度Index));

            MakeBinding(养成循环_体力, RadioButton.IsCheckedProperty, nameof(VM.Is用尽体力));
            MakeBinding(养成循环_次数, RadioButton.IsCheckedProperty, nameof(VM.Is设置循环));
            MakeBinding(养成次数Input, TextBox.TextProperty, nameof(VM.养成次数Value));
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
            if (Monitor.UserConfig?.SBGConfig is ShiningGirl.Config cfg)
                cfg.SkillGetingTask = null;

            // 更新源集合
            协助卡ComboBox.ItemsSource = Directory.GetFiles(World.CardDir)
                .Select(f => new 协助卡Item { Name = Path.GetFileNameWithoutExtension(f), Path = f });

            技能ComboBox.ItemsSource = new SimpleFileManager(App.SkillStrategyDir).Names;

            养成算法ComboBox.ItemsSource = 算法集合;

            // 值还原
            协助卡ComboBox.SelectedIndex = ((IEnumerable<协助卡Item>)协助卡ComboBox.ItemsSource).ToList().FindIndex(x => x.Name.Contains(YC.协助卡名称));
            养成算法ComboBox.SelectedIndex = Array.FindIndex(算法集合, x => x.代号 == YC.养成算法代号);

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
            // 保存无法绑定的值
            YC.协助卡名称 = ((协助卡Item)协助卡ComboBox.SelectedItem)?.Name ?? "";
            YC.养成算法代号 = ((ShiningGirl.AlgorithmItem)养成算法ComboBox.SelectedItem).代号;

            // 存入文件
            用户DataModel.Save(
                clt: 预设Manager.FileManager.SelectedFile
                );
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
                if (Checker.IsKeyNumeric(e.Key) && length != limitLength)
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

        private void Button技能编辑_Click(object sender, RoutedEventArgs e) => App.UserWindow.打开技能窗口();
        #endregion

        #region 预设功能
        private void 预设下拉列表_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 保存旧值
            Save();

            // 选择新值
            string? value = 预设Manager.Main.SelectedValue as string;
            if (value is null)
            {
                预设Manager.Cancel();
                预设删除Button.Visibility = Visibility.Collapsed;
            }
            else
            {
                预设Manager.Select(value);
                预设删除Button.Visibility = Visibility.Visible;
            }

            预设显示TextBlock.Text = value ?? "默认预设";
            YC.ChangeValue(预设Manager.FileManager.SelectedFile);
            UpdateSource();
        }
        private void 预设展开Click(object sender, RoutedEventArgs e) => 预设Popup.IsOpen = true;
        private void 预设TextBox_LostFocus(object sender, RoutedEventArgs e) => 预设Manager.EndRename();
        private void 预设TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                预设Manager.EndRename();
                e.Handled = true;
            }
        }
        private void 预设添加(object sender, RoutedEventArgs e) => YC.Save(预设Manager.Add());
        private void 预设删除(object sender, RoutedEventArgs e) => 预设Manager.Delete();
        private void 预设重命名(object sender, RoutedEventArgs e) => 预设Manager.EnterRename();
        private void 预设下拉列表_MouseDoubleClick(object sender, MouseButtonEventArgs e) => 预设Manager.EnterRename();
        #endregion

        private void Button执行_Click(object sender, RoutedEventArgs e)
        {
            switch (CoreState)
            {
                case "未启动":
                    Save();

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

                    // 载入用户配置
                    Monitor.UserConfig ??= new World.Config();
                    var mcf = Monitor.UserConfig;
                    mcf.SupportCard = YC.协助卡名称;
                    mcf.DailyRaceNumber = VM.赛事名Index + 1;
                    mcf.DRDNumber = VM.赛事难度Index + 1;
                    mcf.TeamIndex = VM.选队Index;
                    mcf.CultivateExhaustTP = VM.Is用尽体力;
                    mcf.CultivateCount = VM.养成次数Value;
                    mcf.CultivateUseProp = VM.Is使用道具;
                    mcf.CultivateUseDiamond = VM.Is使用宝石;
                    mcf.ExtravaganzaUseDiamond = Config.Is群英允许宝石;

                    mcf.SBGConfig ??= new ShiningGirl.Config();
                    var scf = mcf.SBGConfig;
                    scf.TargetProperty = [YC.V1, YC.V2, YC.V3, YC.V4, YC.V5];
                    scf.ReChallenge = YC.重赛逻辑Index;
                    scf.AlgorithmCode = YC.养成算法代号;
                    scf.SaveFactor = Config.保存养成因子;
                    scf.SaveCultivationInfo = Config.保存养成记录;
                    scf.SaveHighLight = Config.保存高光时刻;
                    scf.UseGPU = false;

                    var dqF = YC.技能文件Value;
                    if (scf.SkillFile == dqF && scf.SkillGetingTask != null)
                    {
                        scf.NeedNewTask = false;
                    }
                    else
                    {
                        scf.NeedNewTask = true;
                        scf.SkillFile = dqF;
                        scf.SkillPaths = 技能编辑.GetSkillPaths(scf.SkillFile);
                    }


                    // 还原养成优俊少女
                    ShiningGirl girl = new(Monitor, scf);
                    girl.Load();
                    Monitor.Girl = girl;

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
    }
}
