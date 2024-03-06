using System.IO;
using System.Threading.Tasks;
using static MHTools.数据工具;

namespace Shining_BeautifulGirls
{
    public partial class ShiningGirl
    {
        #region
        public static string SaveDir { get; set; } = @"./save/";
        private string 本体Json => Path.Combine(SaveDir, "girl.json");
        private string 技能Json => Path.Combine(SaveDir, "skill.json");

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

        private World Mnt { get; init; }
        public Config? UserConfig { get; set; }
        #endregion

        public class Config
        {
            public int[]? TargetProperty { get; set; }
            public int ReChallenge { get; set; } = 0;
            public bool SaveFactor { get; set; } = true;
            public bool SaveCultivationInfo { get; set; } = true;
            public bool SaveHighLight { get; set; } = true;
            public string SkillFile { get; set; } = string.Empty;
            public IEnumerable<string[]> SkillPaths { get; set; } = [];
            public bool NeedNewTask { get; set; } = true;
            public Task<string[][]>? SkillGetingTask { get; set; } = null;
        }

        public ShiningGirl(World world, Config? userConfig = default)
        {
            Mnt = world;
            UserConfig = userConfig;
            SkillManager = SkillCore.New(this);
        }


        public void Log(object logInfo)
        {
            Mnt.Log(logInfo.ToString()!);
        }

        public void Log(string[] logInfoS)
        {
            foreach (string logInfo in logInfoS) { Log(logInfo); }
        }

        public void Save()
        {
            SaveAllToSaveAsJSON(this, 本体Json);
            SaveAllToSaveAsJSON(SkillManager, 技能Json);
        }

        public void Load()
        {
            LoadFromJSON(this, 本体Json);
            if (!EndTraining)
                LoadFromJSON(SkillManager, 技能Json);
        }

        public void 测试()
        {
            SelectMaxFansSuitableRace();
        }
    }
}
