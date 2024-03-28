using MHTools;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Shining_BeautifulGirls
{
    /// <summary>
    /// 启动窗口.xaml 的交互逻辑
    /// </summary>
    public partial class 启动窗口 : Window
    {
        private static 启动窗口? Instance { get; set; }
        public 启动窗口()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;
            Topmost = true;
            Instance = this;
            Task.Run(async () =>
            {
                await Task.Delay(200);
                await Dispatcher.BeginInvoke(() => new MainWindow());
            });

            背景图.ImageSource = new BitmapImage(new Uri(FileManagerHelper.SetDir(App.SystemLogosDir).Find("SBGT")!));
            版本信息.Text = App.Version;
        }

        protected override void OnClosed(EventArgs e)
        {
            Instance = null;
            base.OnClosed(e);
        }

        public static void OnCompleted(Window window)
        {
            window.Show();
            Instance?.Close();
        }
    }
}
