using MHTools;
using System;
using System.IO;

namespace Shining_BeautifulGirls
{
    partial class World : Base
    {
        public static string SymbolDir { get; set; } = @"./symbol/";
        public static string CardDir { get; set; } = @"./card/";
        public static string CacheDir { get; set; } = @"./cache/";
        public static string ScreenshotDir { get; set; } = @"./screenshot/";

        public static readonly double STANDARD_WIDTH = 720d;
        public static readonly double STANDARD_HEIGHT = 1280d;

        public double Width { get; set; } = STANDARD_WIDTH;
        public double Height { get; set; } = STANDARD_HEIGHT;
        public string Screen { get; init; }
        public string DeviceID { get; init; }

        public Action<string> Log { get; init; }
        public Action<string> UpdateLog { get; init; }
        public Action<int> DeleteLog { get; init; }

        AdbHelper ADB { get; init; }

        public ShiningGirl? Girl { get; set; }

        public Config? UserConfig { get; set; }

        public class Config
        {
            public int DailyRaceNumber { get; set; } = 1;
            public int DRDNumber { get; set; } = 1;
            public int TeamNumber { get; set; } = 1;
            public int CultivateCount { get; set; } = -1;
            public bool CultivateUseProp { get; set; } = true;
            public bool CultivateUseMoney { get; set; } = false;
            public string SupportCard { get; set; } = "北部玄驹";
            public ShiningGirl.Config? SBGConfig { get; set; }
        }

        public World(AdbHelper adb)
        {
            ADB = adb;
            DeviceID = ADB.EmulatorName;
            Screen = Path.Combine(CacheDir, $"{DeviceID}.png");
            Log = OnLog;
            UpdateLog = OnUpdateLog;
            DeleteLog = OnDeleteLog;
        }

        public string MakeUniqueCacheFile(string name)
        {
            return Path.Combine(CacheDir, $"{DeviceID}_{name}.png");
        }
    }
}
