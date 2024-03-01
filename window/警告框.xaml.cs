using System.Windows;

namespace Shining_BeautifulGirls
{
    /// <summary>
    /// 警告框.xaml 的交互逻辑
    /// </summary>
    public partial class 警告框 : Window
    {
        public static bool DoNotDisplay { get; private set; } = false;
        public 警告框(string msg)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;

            提示信息.Text = msg;
            DoNotDisplay = false;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        private void 不再提示(object sender, RoutedEventArgs e)
        {
            DoNotDisplay = true;
            确认(sender, e);
        }

        private void 确认(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
