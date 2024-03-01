using Shining_BeautifulGirls.data;
using System.IO;
using static MHTools.数据工具;
namespace Shining_BeautifulGirls
{
    [SaveAllWithout(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)]
    class 配置DataModel : ViewModelBase
    {
        public int OCRthreads { get; set; } = 10;
        public bool 保存养成因子 { get; set; } = true;
        public bool 保存养成记录 { get; set; } = true;
        public bool 保存高光时刻 { get; set; } = true;
        public bool Is群英允许宝石 { get; set; } = false;

        private bool _tipCQ = true;
        public bool Tip传奇赛事
        {
            get => _tipCQ;
            set
            {
                _tipCQ = value;
                OnPropertyChanged();
            }
        }
        private bool _tipQY = true;
        public bool Tip群英联赛
        {
            get => _tipQY;
            set
            {
                _tipQY = value;
                OnPropertyChanged();
            }
        }

        #region 系统定义
        private static string _json = Path.Combine(App.UserDataDir, "system.json");
        private static 配置DataModel? _instance;
        public static 配置DataModel Get
        {
            get
            {
                _instance ??= new 配置DataModel();
                return _instance;
            }
        }
        private 配置DataModel()
        {
            LoadFromJSON(this, _json);
        }
        public static void Save()
        {
            SaveAllToSaveAsJSON(Get, _json);
        }
        #endregion
    }
}
