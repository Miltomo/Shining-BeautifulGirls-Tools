using System.Collections.Generic;
using System.IO;
using static MHTools.数据工具;

namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
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
        public int Turn { get; private set; } = 0;
        [ToSave]
        public string Location { get; private set; } = "主页";
        [ToSave]
        public bool EndTraining { get; private set; } = false;

        [ToSave]
        private int _lastHP = 100;
        [ToSave]
        private string _lastAction = "";

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
        private World Mnt { get; init; }
        public Config? UserConfig { get; set; } = default;

        public class Config
        {
            public int[]? TargetProperty { get; set; }
            public int ReChallenge { get; set; } = 0;
            public List<List<string>>? PrioritySkillList { get; set; }
        }

        public ShiningGirl(World world, Config? userConfig = default)
        {
            Mnt = world;
            UserConfig = userConfig;
        }

        public void Update()
        {
            Mnt.Refresh();
            Turn += 1;
            Vitality = GetHP();
            Vitality = Vitality > 95 && _lastHP < 45 && _lastAction != "休息" ? 0 : Vitality;
            Mood = GetMood();

            InSummer = Check("夏日");
            _lastHP = Vitality;

            // 读取当前属性值
            List<int> property = [];
            for (int i = 0; i < SubjectS.Count; i += 1)
                property.Add(GetPropertyValue(SubjectS[i]));

            _hproperty ??= [.. property];

            for (int i = 0; i < property.Count; i += 1)
            {
                var dq = property[i];
                var hdq = _hproperty[i];

                // 识别值正常时，更新历史记录
                if (hdq - dq < 100) _hproperty[i] = dq;
            }
        }

        public void Log(string logInfo)
        {
            Mnt.Log(logInfo);
        }

        public void Log(string[] logInfoS)
        {
            foreach (string logInfo in logInfoS) { Log(logInfo); }
        }
    }
}
