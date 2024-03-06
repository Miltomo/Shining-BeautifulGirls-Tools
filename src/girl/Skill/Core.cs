using System.Threading.Tasks;
using static MHTools.数据工具;
namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        public class SkillCore
        {
            private ShiningGirl Girl { get; init; }
            private Task<string[][]> SkillWaitingTask => Girl.UserConfig?.SkillGetingTask ?? throw new Exception();
            public bool IsLastL { get; set; } = false;
            public bool IsEndL { get; set; } = false;

            [ToSave]
            public int SkTurns { get; private set; } = 1;

            [ToSave]
            private List<string> Completed { get; set; } = [];


            public class CheckInfo
            {
                public bool IsTarget { get; set; } = false;
                public int Cost { get; set; } = int.MaxValue;
                public string Name { get; set; } = string.Empty;

                public Zone Zone { get; set; }
                public int ButtonX { get; set; } = 0;
                public int ButtonY { get; set; } = 0;
            }

            public static SkillCore New(ShiningGirl girl)
            {
                return new SkillCore(girl);
            }

            private SkillCore(ShiningGirl girl)
            {
                Girl = girl;
            }

            private async Task<string[]> GetTargetsAsync()
            {
                var all = await SkillWaitingTask;

                if (all.Length == 0)
                    return [];

                Index mIndex;
                // 养成结束技能学习
                if (IsEndL)
                    mIndex = ^0;
                // 最后一次技能学习
                else if (IsLastL)
                    mIndex = ^1;
                else
                {
                    if (SkTurns >= 6)
                        mIndex = ^1;
                    else if (SkTurns >= 3)
                        mIndex = ^2;
                    else
                        mIndex = ^3;
                }

                return all[0..mIndex]
                    .SelectMany(x => x)
                    .Except(Completed)
                    .ToArray();
            }

            public async Task<CheckInfo> CheckTheSkillAsync(Zone zone)
            {
                var info = new CheckInfo();
                var mask = Girl.MaskScreen(zone, $"{zone}_mask");

                // 加速：若已获得则跳过
                if (Girl.IsHadSkill(mask))
                    return info;

                var result = await Girl.ExtractInfoAsync(zone);
                var tgs = await GetTargetsAsync();

                for (int i = 0; i < tgs.Length; i++)
                {
                    var skill = tgs[i];
                    if (result.Contains(skill))
                    {
                        info.IsTarget = true;
                        info.Cost = (int)result.NumericLines.FirstOrDefault();
                        info.Name = skill;
                        info.Zone = zone;

                        // 找到学习按钮位置
                        Girl.Match(out OpenCvSharp.Point pt, Symbol.技能加, mask);
                        info.ButtonX = pt.X + 20;
                        info.ButtonY = pt.Y + 20;
                        break;
                    }
                }
                return info;
            }

            public void AcceptLearned(IEnumerable<string> learned)
            {
                SkTurns++;
                Completed.AddRange(learned.Where(s => !string.IsNullOrEmpty(s)));
            }
        }
    }
}
