using ComputerVision;

namespace Shining_BeautifulGirls
{
    //TODO 完善FirstG1实现
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
            public string Introduction { get; set; } = string.Empty;
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
                    return another.Name == Name && another.Ground == Ground && another.Distance == Distance;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return
                    Name.GetHashCode() ^ Ground.GetHashCode() ^ Distance.GetHashCode();
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
            [Zone.赛事Type1, Zone.赛事Name1, Zone.赛事Intro1, Zone.赛事Fans1],
            [Zone.赛事Type2, Zone.赛事Name2, Zone.赛事Intro2, Zone.赛事Fans2],
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

        private bool IsSuitable(RaceInfo race)
        {
            return IsGoodAt(race.Ground) && IsGoodAt(race.Distance);
        }

        /// <summary>
        /// 前往并读取角色适应性信息，完毕后回到<b>养成主界面</b>
        /// </summary>
        private void UpdateAdpTable()
        {
            // 移至能力详情页
            MoveTo(AtMainPage, ZButton.返回);
            Click(Button.能力详情);
            PageDown(Zone.上部, PText.Cultivation.优俊少女详情);

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
        }


        private RaceInfo[] BrowseAllRace()
        {
            List<RaceInfo> list = [];
            while (true)
            {
                var lin = ReadRaceInfos();
                if (list.LastOrDefault() is RaceInfo last && last.Equals(lin.LastOrDefault()))
                    break;
                Array.ForEach(lin, list.Add);
                RaceScroll();
            }

            // 退回到开头
            RaceScroll(-5000);
            /*do
            {
                RaceScroll(-3000);
                if (list.First().Equals(ReadRaceInfos().First()))
                    break;
            } while (true);*/

            return [.. list.Distinct()];
        }

        private bool SelectRace(int index)
        {
            if (index > -1)
            {
                index += 1;
                while (index > 2)
                {
                    RaceScroll();
                    index -= 2;
                }

                Click(index switch
                {
                    1 => Button.赛事位置1,
                    2 => Button.赛事位置2,
                    _ => throw new NotImplementedException()
                }, 500);

                if (IsDimmed(ZButton.通用参赛, 140))
                    return false;

                TargetRace = ReadRaceInfos()[index - 1];

                return true;
            }

            return false;
        }


        /// <summary>
        /// 选择第一个擅长的比赛
        /// </summary>
        /// <returns>true, 选择成功; false, 选择失败|不存在比赛|比赛不可参加|无法进入赛事界面</returns>
        private bool SelectFirstSuitableRace()
        {
            if (AdaptionTable.Count == 0)
                UpdateAdpTable();

            if (GotoRacePage())
            {
                RaceInfo? last = default;

                while (true)
                {
                    var races = ReadRaceInfos();

                    for (int i = 0; i < races.Length; i++)
                    {
                        if (IsSuitable(races[i]))
                        {
                            if (SelectRace(i))
                            {
                                TargetRace = races[i];
                                return true;
                            }

                            return false;
                        }
                    }

                    if (last?.Equals(races[^1]) ?? false)
                        break;

                    last = races[^1];

                    RaceScroll();
                }
            }

            return false;
        }

        private bool SelectMaxFansSuitableRace()
        {
            if (AdaptionTable.Count == 0)
                UpdateAdpTable();

            if (GotoRacePage())
            {
                var infos = BrowseAllRace();
                var mf = infos
                    .Where(IsSuitable)
                    .MaxBy(s => s.Fans);
                return SelectRace(Array.IndexOf(infos, mf));
            }

            return false;
        }

        private bool SelectFirstG1orMaxFans()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// (加速) 确保从任何位置移到赛事界面；确保处于赛事界面
        /// </summary>
        /// <returns>true，成功移动到赛事界面；false，无法移至赛事界面|无比赛</returns>
        private bool GotoRacePage()
        {
            Mnt.Refresh();
            IOCRResult r;

            while (true)
            {
                if (AtRacePage())
                    return true;
                else if (AtMainPage())
                {
                    // 比赛日主页无需检测
                    if (FastCheck(Symbol.比赛日主页))
                        Click(ZButton.通用参赛, 1000);
                    else
                    {
                        if (IsDimmed(ZButton.养成日常赛事位, 140))
                            return false;
                        // 点击赛事按钮
                        Click(ZButton.养成日常赛事位, 1000);
                    }
                }
                // 检测赛事推荐弹窗
                else if (Extract上部().Contains(PText.Race.赛事推荐功能))
                {
                    Click(Button.不弹赛事推荐);
                    Click(Button.比赛结束, 1000);
                }
                // 检测提醒弹窗
                else if ((r = Extract中部(), r.Equals(PText.Race.前往赛事)).Item2)
                    Click(Button.大弹窗确认, 1000);
                else if (r.Contains(PText.Race.连续参赛))
                    Click(Button.弹窗确认, 1000);
                // 当没有比赛时
                else if (r.Equals(PText.Cultivation.暂无可以参加的比赛))
                    return false;
                else
                    Click(ZButton.返回);
            }
        }

        private void RaceScroll(double distance = 230)
        {
            Mnt.Scroll([360, 1000, distance]);
            Pause(500);
        }
    }
}
