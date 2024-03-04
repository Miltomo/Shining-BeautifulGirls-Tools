using System.IO;
using static MHTools.数据工具;
namespace Shining_BeautifulGirls
{
    [SaveAllWithout(System.Reflection.BindingFlags.Static)]
    class 用户DataModel : ViewModelBase
    {
        public int V1 { get; set; } = 1100;
        public int V2 { get; set; } = 1100;
        public int V3 { get; set; } = 700;
        public int V4 { get; set; } = 360;
        public int V5 { get; set; } = 400;
        public string 协助卡名称 { get; set; } = "北部玄驹";
        public string 技能文件Value { get; set; } = "";
        public int 重赛逻辑Index { get; set; } = 0;
        public int 选队Index { get; set; } = 0;
        public int 赛事名Index { get; set; } = 0;
        public int 赛事难度Index { get; set; } = 0;

        public bool Is用尽体力 { get; set; } = true;
        public int 养成次数设定 { get; set; } = 1;
        public bool Is使用道具 { get; set; } = true;
        public bool Is使用宝石 { get; set; } = false;

        public bool Is需要养成 { get; set; } = true;
        public bool Is需要竞技场 { get; set; } = true;
        public bool Is需要日常赛事 { get; set; } = true;
        public bool Is需要传奇赛事 { get; set; } = false;
        public bool Is需要群英联赛 { get; set; } = false;

        #region 系统定义
        private static readonly string _json = Path.Combine(App.UserDataDir, "userX.json");
        private static 用户DataModel? _instance;
        public static 用户DataModel Get
        {
            get
            {
                _instance ??= new 用户DataModel();
                return _instance;
            }
        }
        private 用户DataModel()
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
