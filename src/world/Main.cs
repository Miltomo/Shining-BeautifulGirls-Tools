using MHTools;
using System.IO;

namespace Shining_BeautifulGirls
{
    public partial class World : Base
    {
        public static string SymbolDir { get; set; } = @"./symbol/";
        public static string SkillDir { get; set; } = @"./skill/";
        public static string CardDir { get; set; } = @"./card/";
        public static string CacheDir { get; set; } = @"./cache/";
        public static string ScreenshotDir { get; set; } = @"./screenshot/";

        public static readonly double STANDARD_WIDTH = 720d;
        public static readonly double STANDARD_HEIGHT = 1280d;

        public double Width { get; set; } = STANDARD_WIDTH;
        public double Height { get; set; } = STANDARD_HEIGHT;

        private readonly object _screenlock = new();
        private string? _screen;
        public string Screen
        {
            get
            {
                lock (_screenlock)
                {
                    return _screen!;
                }
            }
            init => _screen = value;
        }
        public string DeviceID { get; init; }

        public Action<string> Log { get; init; }
        public Action<string> UpdateLog { get; init; }
        public Action<int> DeleteLog { get; init; }

        public ShiningGirl? Girl { get; set; }

        public Config? UserConfig { get; set; }

        public class Config
        {
            public int DailyRaceNumber { get; set; } = 1;
            public int DRDNumber { get; set; } = 1;
            public int TeamIndex { get; set; } = 0;
            public bool CultivateExhaustTP { get; set; } = true;
            public int CultivateCount { get; set; } = 1;
            public bool CultivateUseProp { get; set; } = true;
            public bool CultivateUseDiamond { get; set; } = false;
            public bool ExtravaganzaUseDiamond { get; set; } = false;
            public string SupportCard { get; set; } = "北部玄驹";
            public ShiningGirl.Config? SBGConfig { get; set; }
        }

        private static AdbHelper ADB => AdbHelper.Instance;

        public World(string emulator)
        {
            ADB.EmulatorName = emulator;
            DeviceID = FileManagerHelper.SanitizeFileName(emulator);
            Screen = Path.Combine(CacheDir, $"{DeviceID}.png");
            Log = OnLog;
            UpdateLog = OnUpdateLog;
            DeleteLog = OnDeleteLog;
        }

        /// <summary>
        /// 通过简单文件名制作独属于设备的cache文件路径
        /// </summary>
        /// <param name="name"></param>
        /// <returns>文件绝对路径</returns>
        public string MakeUniqueCacheFile(string name)
        {
            return Path.Combine(CacheDir, $"{DeviceID}_{FileManagerHelper.SanitizeFileName(name)}.png");
        }

        public bool AtStartPage() => ExtractZoneAndContains(ZButton.养成, PText.Main.养成);
    }
}
