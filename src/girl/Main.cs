using System.IO;
using static MHTools.数据工具;

namespace Shining_BeautifulGirls
{
    partial class ShiningGirl(World world, ShiningGirl.Config? userConfig = default)
    {
        public static string RecordDir { get; set; } = @"./record/";
        public static string HighLightDir
        {
            get
            {
                var path = Path.Combine(World.ScreenshotDir, "highlight");
                Directory.CreateDirectory(path);
                return path;
            }
        }

        [ToSave]
        private int[]? _hproperty;
        public int[] Property => _hproperty ?? [0, 0, 0, 0, 0];
        public int Vitality { get; private set; }
        public int Mood { get; private set; } = 3;//普通

        [ToSave]
        public int Turn { get; private set; } = 1;
        [ToSave]
        public string Stage { get; private set; } = string.Empty;
        [ToSave]
        public bool EndTraining { get; private set; } = false;

        [ToSave]
        private int _lastHP = 100;
        [ToSave]
        private string _lastAction = "休息";

        private string 回合开始 => $"###############第 {Turn} 回合###############";

        public string[] 基本信息 =>
                [
                $"心情: {Mood switch
                {
                    5 => "极佳",
                    4 => "上佳",
                    3 => "普通",
                    2 => "不佳",
                    1 => "极差",
                    _ => "未知",
                }}",
                    $"体力: {Vitality}",
                    $"属性值: {Property[0]} , {Property[1]} , {Property[2]} , {Property[3]} , {Property[4]}",
                ];
        public string[] 训练信息
        {
            get
            {
                List<string> list = [];
                for (int i = 0; i < T.Count; i++)
                {
                    var dq = T[i];
                    var t = dq.Subject + " => ";
                    t += $"协助数: {dq.HeadInfo.Count} ";
                    t += "增益值: ";
                    for (int k = 0; k < dq.UpS.Length; k++)
                        t += dq.UpS[k] + " ";
                    t += $"得分: {dq.Score:f3}";
                    list.Add(t);
                }
                return [.. list];
            }
        }

        private World Mnt { get; init; } = world;
        public Config? UserConfig { get; set; } = userConfig;

        public class Config
        {
            public int[]? TargetProperty { get; set; }
            public int ReChallenge { get; set; } = 0;
            public List<List<string>>? PrioritySkillList { get; set; }
        }

        public void Log(object logInfo)
        {
            Mnt.Log(logInfo.ToString()!);
        }

        public void Log(string[] logInfoS)
        {
            foreach (string logInfo in logInfoS) { Log(logInfo); }
        }

        public void 测试()
        {
            IsDimmed(ZButton.查看结果);
        }
    }
}
