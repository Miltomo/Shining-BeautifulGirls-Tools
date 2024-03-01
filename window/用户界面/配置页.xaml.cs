using MHTools;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Shining_BeautifulGirls
{
    /// <summary>
    /// 配置页.xaml 的交互逻辑
    /// </summary>
    public partial class 配置页 : Page
    {
        static 配置DataModel VM => 配置DataModel.Get;

        public 配置页()
        {
            InitializeComponent();
            DataContext = VM;

            App.SetImage(主页图标, FileManagerHelper.SetDir(App.SystemIconsDir).Find("主页")!);

            // 绑定数据值
            OCR线程数滑条.SetBinding(Slider.ValueProperty, MakeBinding(nameof(VM.OCRthreads)));
            养成因子CheckBox.SetBinding(CheckBox.IsCheckedProperty, MakeBinding(nameof(VM.保存养成因子)));
            养成记录CheckBox.SetBinding(CheckBox.IsCheckedProperty, MakeBinding(nameof(VM.保存养成记录)));
            高光CheckBox.SetBinding(CheckBox.IsCheckedProperty, MakeBinding(nameof(VM.保存高光时刻)));
            传奇赛事CheckBox.SetBinding(CheckBox.IsCheckedProperty, MakeBinding(nameof(VM.Tip传奇赛事)));
            群英联赛CheckBox.SetBinding(CheckBox.IsCheckedProperty, MakeBinding(nameof(VM.Tip群英联赛)));
            群英允许宝石CheckBox.SetBinding(CheckBox.IsCheckedProperty, MakeBinding(nameof(VM.Is群英允许宝石)));
            // =========
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


        private void 主页图标MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (App.UserWindow is 用户界面 wd)
                wd.切换页面(用户界面.PageEnum.主页面);
        }
    }
}
