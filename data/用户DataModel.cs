namespace Shining_BeautifulGirls
{
    internal class 用户DataModel
    {
        #region 定义
        public int[] 目标属性值 { get; set; } = [1100, 1000, 700, 360, 400];
        public string 协助卡名称 { get; set; } = "北部玄驹";
        public string 技能文件名 { get; set; } = "";
        public int 重赛逻辑 { get; set; } = 0;
        public int 选队Index { get; set; } = 0;
        public int 赛事名Index { get; set; } = 0;
        public int 赛事难度Index { get; set; } = 0;

        public bool 需要养成 { get; set; } = true;
        public int 养成次数设定 { get; set; } = -1;
        public bool 使用道具 { get; set; } = true;
        public bool 使用宝石 { get; set; } = false;

        public bool 需要竞技场 { get; set; } = true;
        public bool 需要日常赛事 { get; set; } = true;
        public bool 需要传奇赛事 { get; set; } = false;
        #endregion

    }
}
