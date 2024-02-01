using System.Text.RegularExpressions;
namespace Shining_BeautifulGirls
{
    partial class ShiningGirl
    {
        class AdaptInfo
        {
            public string Item { get; set; }
            public string Rank { get; set; }
        }
        class RaceInfo
        {
            public string Name { get; set; }
            public int Fans { get; set; }
            public string Ground { get; set; }
            public string Distance { get; set; }

            public override string ToString()
            {
                return $"{Name} => [ {Ground} | {Distance} ]、预计增粉 {Fans} 人";
            }

            public override bool Equals(object? obj)
            {
                if (obj is RaceInfo another)
                {
                    return another.Name == Name;
                }
                return false;
            }
        }

        private static Zone[] 场地适应性 { get; } = [Zone.Rank草地, Zone.Rank泥地];
        private static Zone[] 距离适应性 { get; } = [Zone.Rank短距离, Zone.Rank英里, Zone.Rank中距离, Zone.Rank长距离];
        private static Zone[] 跑法适应性 { get; } = [Zone.Rank领跑, Zone.Rank跟前, Zone.Rank居中, Zone.Rank后追];
        private static string[] AllGround { get; } = 场地适应性.Select(x => x.ToString().Replace("Rank", "")).ToArray();
        private static string[] AllDistance { get; } = 距离适应性.Select(x => x.ToString().Replace("Rank", "")).ToArray();
        private static string[] AllRunMethod { get; } = 跑法适应性.Select(x => x.ToString().Replace("Rank", "")).ToArray();

        private static readonly string[] AcceptRank = ["S", "A", "B"];
        private List<AdaptInfo> AdaptionTable { get; } = [];

        private static Zone[][] RaceZoneTable { get; } = [
            [Zone.赛事Type1, Zone.赛事Name1, Zone.赛事Fans1],
            [Zone.赛事Type2, Zone.赛事Name2, Zone.赛事Fans2],
        ];

        private RaceInfo TargetRace { get; set; }

        //==============================================================

        private bool IsGoodAt(string? item)
        {
            var r = AdaptionTable
                    .Where(x => x.Item == item)
                    .Select(w => w.Rank)
                    .FirstOrDefault() ?? "G";
            return AcceptRank.Contains(r);
        }

        private bool IsSuitableRace(RaceInfo race)
        {
            return IsGoodAt(race.Ground) && IsGoodAt(race.Distance);
        }

        /// <summary>
        /// 前往并读取角色适应性信息，完毕后回到<b>赛事界面</b>
        /// </summary>
        private void UpdateAdpTable()
        {
            // 移至能力详情页
            MoveTo(AtMainPage, Button.返回);
            Click(Button.能力详情);
            PageDown(Zone.下部, PText.Cultivation.关闭);

            // 读取适应性等级值
            AdaptionTable.Clear();
            for (int i = 0; i < 场地适应性.Length; i++)
                AdaptionTable.Add(new AdaptInfo
                {
                    Item = AllGround[i],
                    Rank = GetAdaptability(场地适应性[i])
                });

            for (int i = 0; i < 距离适应性.Length; i++)
                AdaptionTable.Add(new AdaptInfo
                {
                    Item = AllDistance[i],
                    Rank = GetAdaptability(距离适应性[i])
                });

            for (int i = 0; i < 跑法适应性.Length; i++)
                AdaptionTable.Add(new AdaptInfo
                {
                    Item = AllRunMethod[i],
                    Rank = GetAdaptability(跑法适应性[i])
                });

            // 返回主界面
            MoveTo(AtMainPage, Button.比赛结束);
            GotoRacePage();
        }


        private RaceInfo[] BrowseAllRace()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 选择第一个擅长的比赛
        /// </summary>
        /// <returns>true, 选择成功; false, 选择失败|不存在比赛|比赛不可参加</returns>
        private bool SelectFirstSuitableRace()
        {
            if (AdaptionTable.Count == 0)
                UpdateAdpTable();
            else
                GotoRacePage();

            RaceInfo? last = default;

            while (true)
            {
                var races = GetRaceInfos();
                for (int i = 0; i < races.Length; i++)
                {
                    if (IsSuitableRace(races[i]))
                    {
                        Click(i switch
                        {
                            0 => Button.赛事位置1,
                            1 => Button.赛事位置2,
                            _ => throw new NotImplementedException()
                        }, 500);

                        if (IsDimmed(ZButton.通用参赛, 140))
                            return false;

                        TargetRace = races[i];
                        return true;
                    }
                }

                if (last?.Equals(races[^1]) ?? false)
                    break;
                last ??= races[^1];

                RaceScroll();
            }

            return false;
        }

        private bool SelectRace(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// (加速) 确保从任何位置移到赛事界面；确保处于赛事界面
        /// </summary>
        private void GotoRacePage()
        {
            _lastAction = "比赛";

            // 判断是否就在赛事界面
            if (AtRacePage())
                return;

            // 确保移至主界面
            if (!AtMainPage())
                MoveTo(AtMainPage, Button.返回);

            // 移至赛事界面以及处理相关弹窗
            while (true)
            {
                Click(ZButton.通用参赛, 1000);

                if (FastCheck(Symbol.连续参赛))
                    Click(Button.弹窗确认, 1000);
                if (FastCheck(Symbol.赛事推荐弹窗, sim: 0.8))
                {
                    Click(Button.不弹赛事推荐);
                    Click(Button.比赛结束);
                }

                if (AtRacePage())
                    break;
            }
        }

        private void RaceScroll(double distance = 230)
        {
            Mnt.Scroll([360, 1000, distance]);
            Pause(500);
        }



        //==============================================================

        [GeneratedRegex(@"^\+\d+.+人$")]
        private static partial Regex RaceFansRegex();
        [GeneratedRegex("[\\+,人]")]
        private static partial Regex RaceFansTransRegex();
    }
}
