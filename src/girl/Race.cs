using System;
using System.Linq;
using System.Text.RegularExpressions;
using static Shining_BeautifulGirls.World.NP;
namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        private static Zone[][] RaceZoneTable { get; } = [
            [Zone.赛事Type1, Zone.赛事Name1, Zone.赛事Fans1],
            [Zone.赛事Type2, Zone.赛事Name2, Zone.赛事Fans2],
        ];

        private enum 比赛场地
        {
            草地,
            泥地
        }

        private enum 比赛距离
        {
            短距离,
            英里,
            中距离,
            长距离
        }

        class RaceInfo
        {
            public string Name { get; set; }
            public int Fans { get; set; }
            public 比赛场地 Ground { get; set; }
            public 比赛距离 Distance { get; set; }

            public override string ToString()
            {
                return $"{Name}：[{Ground}|{Distance}]、预计增粉 {Fans} 人";
            }
        }


        private void 前往日常赛事()
        {

        }

        private void 获取赛事信息()
        {
            // 赛事名
            // 粉丝增加量
            // 适应类型
            // ...

            // 加速：先检查类型

            foreach (var zones in RaceZoneTable)
            {
                var r = Mnt.ExtractZone(zones[0]);
                var 场地 = r.FirstIn(Enum.GetNames<比赛场地>());
                var 距离 = r.FirstIn(Enum.GetNames<比赛距离>());

                // 如果类型适合
                if (true)
                {
                    // 获取比赛名称
                    string name = Mnt.ExtractZoneText(zones[1]);

                    // 获取粉丝数
                    var fs = Mnt
                        .ExtractZoneLike(zones[2], RaceFansRegex())
                        .FirstOrDefault();
                    if (fs != null)
                    {
                        int fans = int.Parse(RaceFansTransRegex().Replace(fs, ""));
                    }
                }
            }

        }

        private void 选定赛事()
        {
            // bool函数
            // 选择后检测预约按键的"明亮度"，只有亮起才能选择
        }

        private void 赛事Scroll(double distance)
        {
            Mnt.Scroll([360, 1000, distance]);
            Mnt.Pause(500); // > 500
            Mnt.Refresh();
        }

        public void 测试()
        {
            赛事Scroll(230);
            获取赛事信息();
        }

        [GeneratedRegex(@"^\+\d+.+人$")]
        private static partial Regex RaceFansRegex();
        [GeneratedRegex("[\\+,人]")]
        private static partial Regex RaceFansTransRegex();
    }
}
