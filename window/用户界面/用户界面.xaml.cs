using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Shining_BeautifulGirls
{
    /// <summary>
    /// 用户界面.xaml 的交互逻辑
    /// </summary>
    public partial class 用户界面 : Window
    {
        readonly BlockingCollection<Action> TipQueue = [];
        World Monitor { get; init; }

        public enum PageEnum
        {
            主页面,
            配置页面,
        }

        public 用户界面(Emulator.EmulatorItem emulator)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.CanMinimize;
            Title = $"操作设备：{emulator}";
            Height = 900;
            Width = 600;
            Monitor = new(emulator.ID);
            App.UserWindow = this;
            App.CorePage = new 核心页(Monitor);
            App.ConfigPage = new 配置页();
            frame.Navigate(App.CorePage);

            Switch即时消息框状态(false);

            // 启动处理消息队列的后台线程
            Task.Run(() =>
            {
                foreach (var task in TipQueue.GetConsumingEnumerable())
                    task();
            });
#if DEBUG
            右键测试.Visibility = Visibility.Visible;
#endif
            //TODO 更换增益值检测方法
            //TODO 思考如何处理文件空值问题？
            //TODO 异常未成功截获？

        }

        public void 切换页面(PageEnum page)
        {
            switch (page)
            {
                case PageEnum.主页面:
                    frame.Navigate(App.CorePage);
                    break;

                case PageEnum.配置页面:
                    frame.Navigate(App.ConfigPage);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            Save();
            base.OnClosed(e);
        }

        private void Switch即时消息框状态(bool open = true)
        {
            即时消息框.Opacity = open ? 1 : 0;
            即时消息框.Visibility = open ? Visibility.Visible : Visibility.Collapsed;
        }

        public static void Refresh()
        {
            if (App.CorePage is 核心页 cp)
            {
                cp.UpdateSource();
            }
        }

        public static void Save()
        {
            配置DataModel.Save();
            if (App.CorePage is 核心页 cp)
            {
                cp.Save();
            }
        }

        public void Toast(object msg)
        {
            var text = msg.ToString();

            TipQueue.Add(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    即时消息文本.Text = text;
                    Switch即时消息框状态();
                });
                Thread.Sleep(1000);

                int sum = 500, step = 20;
                int count = sum / step;
                var bias = 1d / count;
                for (int i = 0; i < count; i++)
                {
                    Dispatcher.Invoke(() => 即时消息框.Opacity -= bias);
                    Thread.Sleep(step);
                }

                Dispatcher.Invoke(() => Switch即时消息框状态(false));
            });
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
                //Monitor.Refresh();
                Monitor.Start();
                // 创建 Stopwatch 实例
                Stopwatch stopwatch = new();

                // 启动计时器
                stopwatch.Start();

                // 调用需要计时的函数

                //Toast($"亮度：{ComputerVision.ImageRecognition.AvgBrightness(Monitor.CropScreen(ZButton.养成日常赛事位, "测试"))}");

                var g = new ShiningGirl(Monitor, new ShiningGirl.Config());
                g.测试(Toast);

                /*Toast(Monitor.Match(out _, Symbol.G1));*/


                // 停止计时器
                stopwatch.Stop();
                Monitor.Stop();

                // 获取经过的时间（毫秒）
                long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                Debug.WriteLine($"Function took {elapsedMilliseconds} milliseconds to execute.");
                Toast("测试结束");
            });
            thread.Start();
        }
    }
}
