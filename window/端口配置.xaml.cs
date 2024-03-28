using MHTools.UI;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static MHTools.数据工具;

namespace Shining_BeautifulGirls
{
    /// <summary>
    /// 端口配置.xaml 的交互逻辑
    /// </summary>
    public partial class 端口配置 : Window
    {
        private static string _jsonPorts = Path.Combine(App.UserDataDir, "port.json");
        [ToSave]
        private static List<string> Ports { get; set; } = [];
        public 端口配置()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;
            LoadFromJSON(this, _jsonPorts);
            Ports.ForEach(x => theListBox.Items.Add(x));
        }

        public static string[] GetPortsInfo()
        {
            LoadFromJSON(typeof(端口配置), _jsonPorts);
            return [.. Ports];
        }

        protected override void OnClosed(EventArgs e)
        {
            Ports.Clear();

            foreach (var item in theListBox.Items)
                Ports.Add(item.ToString()!);

            if (theInput.Text.Length > 0)
                Ports.Add(theInput.Text);

            if (App.MWindow is MainWindow mw)
            {
                mw.IsEnabled = true;
                mw.Show();
            }

            SaveAllToSaveAsJSON(this, _jsonPorts);

            base.OnClosed(e);
        }

        private void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (theInput.Text.Length > 0)
                {
                    theListBox.Items.Add(theInput.Text);
                    theInput.Text = null;
                }

                e.Handled = true;
            }
            else if (Checker.IsKeyNumeric(e.Key) == false)
            {
                e.Handled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button bt = (System.Windows.Controls.Button)sender;
            bt.IsEnabled = false;
            var item = App.FindVisualParent<ListBoxItem>(bt);
            var index = theListBox.ItemContainerGenerator.IndexFromContainer(item);
            theListBox.Items.RemoveAt(index);
        }
    }
}
